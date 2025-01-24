using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public static Action JumpEvent;
    private Vector2 _movemetInput;
    [SerializeField] private bool _jump;

    public Vector2 MovmentInput=> _movemetInput;

    private void Awake()
    {
        Instance = this;
    }

    void OnMove(InputValue input)
    {
        _movemetInput= input.Get<Vector2>();
    }

    public void OnJump(InputValue input)
    {
        if (input.isPressed && _jump == false)
        {
            JumpEvent?.Invoke();
        }

        _jump = input.isPressed;
    }
}
