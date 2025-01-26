using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D _playerBody;
    [SerializeField] protected PlayerHanlder _playerHandler;

    [Header("Movment Variables")]
    [Header("Ground Movement")]
    [SerializeField] protected float _groundMaxSpeed;
    [SerializeField] protected float _groundAcceleration;
    [SerializeField] protected float _groundDecceleration;

    [Header("Air Movement")]
    [SerializeField] protected float _airMaxSpeed;
    [SerializeField] protected float _airAcceleration;
    [SerializeField] protected float _airDecceleration;

    [Header("Jump")]
    [SerializeField] protected int _numOfjumps;
    [SerializeField] protected float _jumpForce;
    [SerializeField] protected float _maxJumpSpeed;

    [Header("FallForce")]
    [SerializeField] protected float _fastFallGravity;
    [SerializeField] protected float _fastFallStartTimer;
                     
    [Header("Gravity")]
    [SerializeField] protected float _normalGravity;
    [SerializeField] protected float _fallGravity;

    [Header("CheckFloor")]
    [SerializeField] protected Transform _checkFloor;
    [SerializeField] protected float _radius;
    [SerializeField] protected LayerMask _groundLayer;

    [Header("Effects")]
    [SerializeField] protected Animator _animator;
    [SerializeField] protected MMF_Player _knockFeedBack;
    [SerializeField] protected MMF_Player _jumpFeedBack;
    [SerializeField] protected ParticleSystem _jumpVFX;
    [SerializeField] protected MMF_Player _midAirFallFeedBack;
    [SerializeField] protected MMF_Player _doubleJumpFeedBack;
    [SerializeField] protected MMF_Player _landFeedback;
    [SerializeField] protected MMF_Player _runFeedBack;
    [SerializeField] protected MMF_Player _idleFeedBack;


    protected float _delayMovement;
    protected int _currentJumps;
    protected float _currentTime;
    protected float _currentDiableTime;

    [SerializeField] protected Vector2 _movementInput;
    protected Vector2 _moveDir;
    protected Vector3 _scale;

    protected bool _canFastFall;
    protected bool _jump;
    [SerializeField] protected bool _isGrounded;

    protected bool _moveDelay;
    [SerializeField] protected bool _canMove;
    protected bool _inKnockback;

    public bool CanMove { get => _canMove; set => _canMove = value; }
    public float StopMovementTimer { get => _delayMovement; set => _delayMovement = value; }
    public Animator PlayerAnimator => _animator;


    protected virtual void Start()
    {
        _scale = transform.localScale;
        //_playerHandler.JumpEvent += Jump;
        _playerHandler.WeaponHandler.AttackEvent += DelayMovement;
    }

    protected virtual void Update()
    {
        if (_playerHandler.dead)
            return;

        CheckGround();

        if (_moveDelay == false)
            DisableMovementTimer();
    }

    protected virtual void FixedUpdate()
    {
        if (_playerHandler.dead)
            return;

        if (_inKnockback)
            return;

        if (_canMove)
            Move();
    }

    protected virtual void Move()
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

    protected virtual void Run()
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

    protected virtual void AirRun()
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

    protected virtual void Jump()
    {
        if (_playerHandler.dead)
            return;

        if (_isGrounded)
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

    protected virtual IEnumerator JumpAnticipation()
    {
        yield return new WaitForSeconds(0.3f);
        Push();
    }

    protected virtual void Push()
    {
        SetGravity(_normalGravity);
        _playerBody.velocity = new Vector2(_playerBody.velocity.x, 0);
        _playerBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }

    protected virtual void GroundDeccelerate()
    {
        if (_playerBody.velocity.x > 0.1f)
            _playerBody.AddForce(Vector2.left * _groundDecceleration);
        else if (_playerBody.velocity.x < -0.1f)
            _playerBody.AddForce(Vector2.right * _groundDecceleration);
        else
            _playerBody.velocity = Vector2.zero;
    }

    protected virtual void AirDeccelerate()
    {
        if (_playerBody.velocity.x > 0)
            _playerBody.AddForce(Vector2.left * _airDecceleration);
        else if (_playerBody.velocity.x < 0)
            _playerBody.AddForce(Vector2.right * _airDecceleration);
    }

    public virtual void PlayAnimation(string name)
    {
        _animator.Play(name);
    }

    protected virtual void CheckFall()
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

    protected virtual void SetGravity(float gravity)
    {
        _playerBody.gravityScale = gravity;
    }

    protected virtual void AirTimer()
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

    protected virtual void DisableMovementTimer()
    {
        _currentDiableTime += Time.deltaTime;
        if (_currentDiableTime >= _delayMovement)
        {
            _currentDiableTime = 0;
            _moveDelay = true;
        }
    }

    protected bool IsGrounded()
    {
        return _isGrounded;
    }

    protected virtual void CheckGround()
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

    protected virtual void DelayMovement()
    {
        _moveDelay = false;
    }

    public virtual void Knockback(float amount, Vector2 direction)
    {
        _knockFeedBack.PlayFeedbacks();
        _playerBody.velocity = Vector2.zero;
        _animator.Play("KnockBack");
        _playerBody.AddForce(direction * amount, ForceMode2D.Impulse);
    }

    public virtual void StopMovement()
    {
        _playerBody.velocity = Vector2.zero;
    }
}
