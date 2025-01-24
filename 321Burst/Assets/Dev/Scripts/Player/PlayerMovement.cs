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
    [SerializeField] float _jumpForce;
    [SerializeField] float _jumpTimer;

    [Header("FallForce")]
    [SerializeField] float _fallForce;

    private bool _jumpInput;
    private Vector2 _movementInput;
    private Vector2 _moveDir;

    private bool _canAirDodge;
    private bool _isJumping;
    private float _currentTime;

    private void Start()
    {
        InputManager.JumpEvent += Jump;
    }

    private void Update()
    {
        _movementInput = InputManager.Instance.MovmentInput;
        Move();
    }


    private void Move()
    {
        _moveDir.x = _movementInput.x * _runForce;
        _playerBody.AddForce(_moveDir);

        if (_playerBody.velocity.x >= _maxSpeed)
        {
            _playerBody.velocity = new Vector2(_maxSpeed, _playerBody.velocity.y);
        }

    }

    void Jump()
    {
            _playerBody.AddForce(Vector2.up* _jumpForce, ForceMode2D.Impulse);
    }

    void AirTimer()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime >= _jumpTimer)
        {
            _currentTime = 0;
            _canAirDodge = true;
        }
    }
}
