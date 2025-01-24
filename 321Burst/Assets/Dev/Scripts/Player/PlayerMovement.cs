using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody2D _playerBody;

    [Header("Movment Variables")]
    [Header("Run")]
    [SerializeField] float _maxSpeed;
    [SerializeField] float _runForce;
    [SerializeField] float _disselerationForce;

    [Header("Jump")]
    [SerializeField] int _numOfjumps;
    [SerializeField] float _jumpForce;

    [Header("FallForce")]
    [SerializeField] float _dogeGravity;
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

    private bool _canAirDodge;
    [SerializeField] private bool _isGrounded;

    private void Start()
    {
        InputManager.JumpEvent += Jump;
    }

    private void Update()
    {
        _movementInput = InputManager.Instance.MovmentInput;
        Move();
        Debug.Log(_playerBody.velocity.x);
    }

    private void Move()
    {
        if (IsGrounded())
        {
            Run();
            if (_movementInput.x == 0)
            {
                Desslerate();
            }
        }
        SetGravirty();
    }

    void Run()
    {
        _moveDir.x = _movementInput.x * _runForce;
        _playerBody.AddForce(_moveDir);

        float dir = _playerBody.velocity.x;
        if (Mathf.Abs(dir) >= _maxSpeed)
        {
            if (dir > 0)
                _playerBody.velocity = new Vector2(_maxSpeed, _playerBody.velocity.y);
            else if (dir < 0)
                _playerBody.velocity = new Vector2(-_maxSpeed, _playerBody.velocity.y);
        }
    }
    void Jump()
    {
        if (_currentJumps < _numOfjumps)
        {
            _currentJumps++;
            _playerBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    void Desslerate()
    {
        if (_playerBody.velocity.x > 0)
            _playerBody.AddForce(Vector2.left * _disselerationForce);
        else if(_playerBody.velocity.x < 0)
            _playerBody.AddForce(Vector2.right * _disselerationForce);
    }

    void SetGravirty()
    {
        if (_playerBody.velocity.y < 0)
            _playerBody.gravityScale = _fallGravity;
        else
            _playerBody.gravityScale = _normalGravity;
    }

    void AirTimer()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime >= _fastFallStartTimer)
        {
            _currentTime = 0;
            _canAirDodge = true;
        }
    }

    bool IsGrounded()
    {
        Collider2D collider= Physics2D.OverlapCircle(_checkFloor.transform.position, _radius, _groundLayer);
        if (collider != null)
            _isGrounded = true;
        else
            _isGrounded = false;

        return _isGrounded;
    }
}
