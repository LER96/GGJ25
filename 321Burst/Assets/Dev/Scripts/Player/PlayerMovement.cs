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


    private int _currentJumps;
    private float _currentTime;

    private Vector2 _movementInput;
    private Vector2 _moveDir;

    private bool _canFastFall;
    private bool _jump;
    [SerializeField] private bool _isGrounded;


    private void FixedUpdate()
    {
        Move();
        Debug.Log("player X vel: " + _playerBody.velocity.x + "\nplayer Y vel: " + _playerBody.velocity.y);
    }

    private void Move()
    {
        
        if (IsGrounded())
        {
            _currentJumps = 0;
            _canFastFall = false;
            Run();
            if (_movementInput.x == 0 || _movementInput.x * _playerBody.velocity.x < 0)
            {
                GroundDeccelerate();
            }
        }
        else
        {
            AirRun();
            if (_movementInput.x == 0 || _movementInput.x * _playerBody.velocity.x < 0)
            {
                AirDeccelerate();
            }
        }

        
        CheckFall();

        if (_canFastFall && _movementInput.y < 0)
            SetGravity(_fastFallGravity);
    }

    void Run()
    {
        _moveDir.x = _movementInput.x * _groundAcceleration;
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
        if (_currentJumps < _numOfjumps)
        {
            _currentJumps++;
            _playerBody.gravityScale = _normalGravity;
            _playerBody.velocity = new Vector2(_playerBody.velocity.x, 0);
            _playerBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    void GroundDeccelerate()
    {
        if (_playerBody.velocity.x > 0)
            _playerBody.AddForce(Vector2.left * _groundDecceleration);
        else if (_playerBody.velocity.x < 0)
            _playerBody.AddForce(Vector2.right * _groundDecceleration);
    }

    void AirDeccelerate()
    {
        if (_playerBody.velocity.x > 0)
            _playerBody.AddForce(Vector2.left * _airDecceleration);
        else if (_playerBody.velocity.x < 0)
            _playerBody.AddForce(Vector2.right * _airDecceleration);
    }

    void CheckFall()
    {
        if (_playerBody.velocity.y < 0)
        {
            SetGravity(_fallGravity);
            AirTimer();
        }
        else
            SetGravity(_normalGravity);
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

    bool IsGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(_checkFloor.transform.position, _radius, _groundLayer);
        if (collider != null)
            _isGrounded = true;
        else
            _isGrounded = false;

        return _isGrounded;
    }

    void OnMove(InputValue input)
    {
        _movementInput = input.Get<Vector2>();
    }

    public void OnJump(InputValue input)
    {
        if (input.isPressed && _jump == false)
        {
            Jump();
        }

        _jump = input.isPressed;
    }
}
