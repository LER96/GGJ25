using MoreMountains.Feedbacks;
using System;
using System.Buffers;
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


    public bool dead = false;

    private void Start()
    {
        LevelManager.Instance.AddPlayer(this);
        dead = false;
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

    public void SetAnimation(string name)
    {
        _playerMovement.PlayAnimation(name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.transform.CompareTag("Weapon"))
        {
            //check if owner is this
            Weapon weapon = collision.GetComponentInParent<Weapon>();
            if (weapon == null)
                return;

            if(weapon.Owner == null) return;

            if (weapon.Owner == this)
            {
                print("hit by own spear");
                return;
            }
            CameraManager.Instance.Shake(.5f);
            PlayerMovement.StopMovement();
            dead = true;
            _deathFeedBack.PlayFeedbacks();
            _hp--;
            LevelManager.Instance.EndRound();
        }
    }


}
