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

    [Header("Jump")]
    [SerializeField] int _numOfjumps;
    [SerializeField] float _jumpForce;

    [Header("FallForce")]
    [SerializeField] float _dogeGravity;
    [SerializeField] float _fastFallStartTimer;

    [Header("Gravity")]
    [SerializeField] float _normalGravity;
    [SerializeField] float _fallGravity;


    private int _currentJumps;
    private float _currentTime;

    private Vector2 _movementInput;
    private Vector2 _moveDir;

    private bool _canAirDodge;
    private bool _isAir;

    private void Start()
    {
        InputManager.JumpEvent += Jump;
    }

    private void Update()
    {
        _movementInput = InputManager.Instance.MovmentInput;
        Move();
        SetGravirty();
    }


    private void Move()
    {
        Run();
        IsGrounded();
    }

    void Jump()
    {
        if (_currentJumps < _numOfjumps)
        {
            _currentJumps++;
            _playerBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    void Run()
    {
        _moveDir.x = _movementInput.x * _runForce;
        _playerBody.AddForce(_moveDir);

        if (_playerBody.velocity.x >= _maxSpeed)
        {
            _playerBody.velocity = new Vector2(_maxSpeed, _playerBody.velocity.y);
        }
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

    void IsGrounded()
    {

    }
}
