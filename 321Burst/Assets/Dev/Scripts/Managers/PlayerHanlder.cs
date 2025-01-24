using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHanlder : MonoBehaviour
{
    public event Action JumpEvent;

    [SerializeField] int HP;

    private Vector2 _movementInput;
    private bool _jump;
    private bool _attack;
    private bool _pick;

    void OnJump(InputValue input)
    {
        if (input.isPressed && _jump == false)
        {
            JumpEvent?.Invoke();
        }

        _jump = input.isPressed;
    }

}
