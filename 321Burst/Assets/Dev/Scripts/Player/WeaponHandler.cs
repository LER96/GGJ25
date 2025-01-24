using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHandler : MonoBehaviour
{
    public event Action OnPick;
    public event Action AttackEvent;
    [SerializeField] Weapon _currentWeapon;
    [SerializeField] Transform _holder;

    private bool _pick;
    private bool _attackPress;
    private bool _canAttack;
    private Vector2 _weaponOffset;

    public bool CanAttack { get => _canAttack; set => _canAttack = value; }
    public Transform Holder=> _holder;

    public Weapon GetWeapon()
    {
        return _currentWeapon;
    }

    public void SetWeapon(Weapon weapon)
    {
        _currentWeapon = weapon;

    }

    public void DisableWeapon()
    {
        _currentWeapon = null;
        _weaponOffset = Vector3.zero;
        _holder.localPosition = Vector3.zero;
    }

    void OnPickUp(InputValue input)
    {
        if (input.isPressed && _pick == false)
        {
            OnPick.Invoke();
        }

        _pick = input.isPressed;
    }

    void OnFire(InputValue input)
    {
        if (_canAttack)
        {
            if (input.isPressed && _attackPress == false)
            {
                AttackEvent?.Invoke();
            }
            _attackPress = input.isPressed;
        }
    }


}
