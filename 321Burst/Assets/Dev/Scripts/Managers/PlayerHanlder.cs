using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHanlder : MonoBehaviour
{
    public event Action JumpEvent;

    [SerializeField] int HP;
    [SerializeField] WeaponHandler _weaponHandler;
    [SerializeField] PlayerMovement _playerMovement;
    private bool _jump;

    private void Start()
    {
        //LevelManager.Instance.AddPlayer(this);
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
