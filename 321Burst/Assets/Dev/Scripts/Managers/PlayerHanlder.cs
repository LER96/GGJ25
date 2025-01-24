using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHanlder : MonoBehaviour
{
    public event Action JumpEvent;

    [SerializeField] int _hp;
    [SerializeField] WeaponHandler _weaponHandler;
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] MMF_Player _deathFeedBack;
    private bool _jump;

    public int HP => _hp;

    public WeaponHandler WeaponHandler=> _weaponHandler;
    public PlayerMovement PlayerMovement => _playerMovement;

    private void Start()
    {
        LevelManager.Instance.AddPlayer(this);
    }

    public void StartRound()
    {
        _weaponHandler.CanAttack = true;
        _playerMovement.CanMove = true;
    }

    void OnJump(InputValue input)
    {
        if (input.isPressed && _jump == false)
        {
            JumpEvent?.Invoke();
        }

        _jump = input.isPressed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Weapon"))
        {
            _deathFeedBack.PlayFeedbacks();
            _hp--;
        }
    }


}
