using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] MMF_Player _attackFeedback;
    [SerializeField] MMF_Player _hitFeedback;
    [SerializeField] MMF_Player _pickUpFeedBack;
    [SerializeField] MMF_Player _dropFeedBack;
    [SerializeField] Vector2 _weaponOffset;

    public Vector2 WeaponOffset => _weaponOffset;

    private WeaponHandler _weaponHandler;
    private Collider2D _target;
    private bool _isPicked;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _weaponHandler = collision.GetComponent<WeaponHandler>();
            _target = collision;
            _weaponHandler.OnPick += PickOrDrop;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _isPicked==false)
        {
            _weaponHandler = collision.GetComponent<WeaponHandler>();
            _target = null;
            _weaponHandler.OnPick -= PickOrDrop;
        }
    }

    void PickOrDrop()
    {
        if(!_isPicked && _target!=null)
        {
            _weaponHandler = _target.GetComponent<WeaponHandler>();
            if (_weaponHandler != null)
            {
                Pick();
                _pickUpFeedBack.PlayFeedbacks();
            }
        }
        else if(_isPicked) 
        {
            Drop();
            _dropFeedBack.PlayFeedbacks();
        }
    }

    void Pick()
    {
        transform.SetParent(_weaponHandler.Holder);
        transform.localPosition = Vector3.zero;
        _weaponHandler.SetWeapon(this);
        _isPicked = true;
        _pickUpFeedBack.PlayFeedbacks();
    }

    void Drop()
    {
        _weaponHandler.DisableWeapon();
        transform.SetParent(null);
        _weaponHandler = null;
        _isPicked=false;
    }

    public void Attack()
    {
        _attackFeedback.PlayFeedbacks();
    }

    public void HitFeedBack()
    {
        _hitFeedback.PlayFeedbacks();
    }
}
