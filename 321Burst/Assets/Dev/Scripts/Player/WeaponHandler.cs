using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHandler : MonoBehaviour
{
    public event Action OnPick;
    [SerializeField] Weapon _currentWeapon;
    [SerializeField] Transform _holder;

    private bool _pick;
    private Vector2 _weaponOffset;

    public Transform Holder=> _holder;

    public Weapon GetWeapon()
    {
        return _currentWeapon;
    }

    public void SetWeapon(Weapon weapon)
    {
        _currentWeapon = weapon;
        _weaponOffset = weapon.WeaponOffset;
        _holder.localPosition = new Vector3(_weaponOffset.x, _weaponOffset.y, 0);
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


}
