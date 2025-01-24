using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHanlder : MonoBehaviour
{

    public Action JumpEvent;
    public Action AttackEvent;

    [SerializeField] int HP;

    private Vector2 _movementInput;
    private bool _jump;
    private bool _attack;
    private bool _pick;
    public Vector2 MovmentInput => _movementInput;


    void OnFire(InputValue input)
    {
        if (input.isPressed && _attack == false)
        {
            AttackEvent?.Invoke();
        }

        _attack = input.isPressed;
    }

    void OnJump(InputValue input)
    {
        if (input.isPressed && _jump == false)
        {
            JumpEvent?.Invoke();
        }

        _jump = input.isPressed;
    }

}
