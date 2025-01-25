using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    [SerializeField] WeaponHandler weaponHand;

    public void Attack()
    {
        Weapon weapon = weaponHand.CurrentWeapon;
        if(weapon != null )
        {
            weapon.PlayerAttack();
        }

    }
}
