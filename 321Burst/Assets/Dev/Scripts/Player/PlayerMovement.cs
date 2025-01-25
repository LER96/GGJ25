using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D _playerBody;

    [Header("Movment Variables")]
    [Header("Ground Movement")]
    [SerializeField] float _groundMaxSpeed;
    [SerializeField] float _groundAcceleration;
    [SerializeField] float _groundDecceleration;

    [Header("Air Movement")]
    [SerializeField] float _airMaxSpeed;
    [SerializeField] float _airAcceleration;
    [SerializeField] float _airDecceleration;

    [Header("Jump")]
    [SerializeField] int _numOfjumps;
    [SerializeField] float _jumpForce;
    [SerializeField] float _maxJumpSpeed;

    [Header("FallForce")]
    [SerializeField] float _fastFallGravity;
    [SerializeField] float _fastFallStartTimer;

    [Header("Gravity")]
    [SerializeField] float _normalGravity;
    [SerializeField] float _fallGravity;

    [Header("CheckFloor")]
    [SerializeField] Transform _checkFloor;
    [SerializeField] float _radius;
    [SerializeField] LayerMask _groundLayer;

    [Header("Effects")]
    [SerializeField] Animator _animator;
    [SerializeField] MMF_Player _jumpFeedBack;
    [SerializeField] ParticleSystem _jumpVFX;
    [SerializeField] MMF_Player _midAirFallFeedBack;
    [SerializeField] MMF_Player _doubleJumpFeedBack;
    [SerializeField] MMF_Player _landFeedback;
    [SerializeField] MMF_Player _runFeedBack;
    [SerializeField] MMF_Player _idleFeedBack;

    
    private float _delayMovement;
    private PlayerHanlder _playerHandler;

    private int _currentJumps;
    private float _currentTime;
    private float _currentDiableTime;

    private Vector2 _movementInput;
    private Vector2 _moveDir;
    private Vector3 _scale;

    private bool _canFastFall;
    private bool _jump;
    [SerializeField] private bool _isGrounded;

    private bool _moveDelay;
    private bool _canMove;
    private bool _inKnockback;

    public bool CanMove { get=> _canMove; set=> _canMove = value; }
    public float StopMovementTimer { get=> _delayMovement; set=> _delayMovement = value; }
    public Animator PlayerAnimator => _animator;
    private void Start()
    {
        _playerHandler = GetComponent<PlayerHanlder>();
        _scale = transform.localScale;
        _playerHandler.JumpEvent += Jump;
        _playerHandler.WeaponHandler.AttackEvent += DelayMovement;
    }

    private void Update()
    {

        if (_playerHandler.dead)
            return;

        CheckGround();

        if (_moveDelay == false)
            DisableMovementTimer();
    }

    private void FixedUpdate()
    {
        if (_playerHandler.dead)
            return;

        if (_inKnockback)
            return;

        if (_canMove)
            Move();
    }

    private void Move()
    {
        if (_isGrounded)
        {
            _currentJumps = 0;
            _canFastFall = false;
            if (_movementInput.x == 0 && Mathf.Abs(_playerBody.velocity.x) > 0.5f)
            {
                _animator.SetBool("IsRunning", false);
                GroundDeccelerate();
            }
            if (_movementInput.x != 0)
                Run();

            if (_movementInput.x == 0 && Mathf.Abs(_playerBody.velocity.x) < 0.5f)
                _playerBody.velocity = new Vector2(0, _playerBody.velocity.y);
        }
        else
        {
            _animator.SetBool("IsRunning", false);
            CheckFall();
            AirRun();
            if (_movementInput.x == 0 || _movementInput.x * _playerBody.velocity.x < 0)
            {
                AirDeccelerate();
            }

            if (_canFastFall && _movementInput.y < 0)
                SetGravity(_fastFallGravity);
        }

    }

    void Run()
    {
        _animator.Play("Run");
        _animator.SetBool("IsRunning", true);
        _moveDir.x = _movementInput.x * _groundAcceleration;
        _runFeedBack.PlayFeedbacks();
        _playerBody.AddForce(_moveDir);

        float dir = _playerBody.velocity.x;
        if (Mathf.Abs(dir) >= _groundMaxSpeed)
        {
            if (dir > 0)
                _playerBody.velocity = new Vector2(_groundMaxSpeed, _playerBody.velocity.y);
            else if (dir < 0)
                _playerBody.velocity = new Vector2(-_groundMaxSpeed, _playerBody.velocity.y);
        }

    }

    void AirRun()
    {
        _moveDir.x = _movementInput.x * _airAcceleration;
        _playerBody.AddForce(_moveDir);

        float dir = _playerBody.velocity.x;
        if (Mathf.Abs(dir) >= _airMaxSpeed)
        {
            if (dir > 0)
                _playerBody.velocity = new Vector2(_airMaxSpeed, _playerBody.velocity.y);
            else if (dir < 0)
                _playerBody.velocity = new Vector2(-_airMaxSpeed, _playerBody.velocity.y);
        }
    }

    void Jump()
    {
        if (_playerHandler.dead)
            return;

        if(_isGrounded)
            _jumpFeedBack.PlayFeedbacks();


        if (_currentJumps < _numOfjumps)
        {
            _currentJumps++;
            if (_isGrounded)
            {
                _jumpVFX.Play();
                _animator.Play("JumpAnticipation");
                Push();
            }
            else
            {
                _animator.Play("DoubleJump");
                _doubleJumpFeedBack.PlayFeedbacks();
                Push();
            }

        }
    }

    IEnumerator JumpAnticipation()
    {
        yield return new WaitForSeconds(0.3f);
        Push();
    }

    void Push()
    {
        SetGravity(_normalGravity);
        _playerBody.velocity = new Vector2(_playerBody.velocity.x, 0);
        _playerBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }

    void GroundDeccelerate()
    {
        if (_playerBody.velocity.x > 0.1f)
            _playerBody.AddForce(Vector2.left * _groundDecceleration);
        else if (_playerBody.velocity.x < -0.1f)
            _playerBody.AddForce(Vector2.right * _groundDecceleration);
        else
            _playerBody.velocity = Vector2.zero;
    }

    void AirDeccelerate()
    {
        if (_playerBody.velocity.x > 0)
            _playerBody.AddForce(Vector2.left * _airDecceleration);
        else if (_playerBody.velocity.x < 0)
            _playerBody.AddForce(Vector2.right * _airDecceleration);
    }

    public void PlayAnimation(string name)
    {
        _animator.Play(name);
    }

    void CheckFall()
    {
        if (_playerBody.velocity.y < 0)
        {
            _animator.SetBool("Down", true);
            _animator.SetBool("Up", false);
            SetGravity(_fallGravity);
            AirTimer();
        }
        else
        {
            _animator.SetBool("Up", true);
            _animator.SetBool("Down", false);
            SetGravity(_normalGravity);
        }
    }

    void SetGravity(float gravity)
    {
        _playerBody.gravityScale = gravity;
    }

    void AirTimer()
    {
        if (_canFastFall == false)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= _fastFallStartTimer)
            {
                _currentTime = 0;
                _canFastFall = true;
            }
        }
    }

    void DisableMovementTimer()
    {
        _currentDiableTime += Time.deltaTime;
        if (_currentDiableTime >= _delayMovement)
        {
            _currentDiableTime = 0;
            _moveDelay = true;
        }
    }    

    bool IsGrounded()
    {
        return _isGrounded;
    }

    void CheckGround()
    {
        Collider2D[] co = Physics2D.OverlapCircleAll(_checkFloor.transform.position, _radius, _groundLayer);
        Debug.Log(co.Length);
        if (co.Length > 0)
        {
            if (_isGrounded == false)
            {
                _landFeedback.PlayFeedbacks();
                _animator.Play("Landing");
            }
            _isGrounded = true;
            _animator.SetBool("Up", false);
            _animator.SetBool("Down", false);
        }
        else
            _isGrounded = false;
    }

    void DelayMovement()
    {
        _moveDelay = false;
    }

    void OnMove(InputValue input)
    {
        if (_moveDelay)
        {
            _movementInput = input.Get<Vector2>();
            _runFeedBack.PlayFeedbacks();
            if (_movementInput.x < 0)
                transform.localScale = new Vector3(-_scale.x, _scale.y, _scale.z);
            else if (_movementInput.x > 0)
                transform.localScale = new Vector3(_scale.x, _scale.y, _scale.z);
            else
                _idleFeedBack.PlayFeedbacks();
        }
    }

    public void Knockback(float amount, Vector2 direction)
    {
        _playerBody.velocity = Vector2.zero;
        _playerBody.AddForce(direction * amount, ForceMode2D.Impulse);
    }

    public void StopMovement()
    {
        _playerBody.velocity = Vector2.zero;
    }

}
