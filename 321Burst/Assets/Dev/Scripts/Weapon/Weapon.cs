using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] float _movingSpeed;
    [SerializeField] MMF_Player _attackFeedback;
    [SerializeField] MMF_Player _hitFeedback;
    [SerializeField] MMF_Player _pickUpFeedBack;

    private WeaponHandler _weaponHandler;
    private Collider2D _target;
    private bool _isPicked;

    private void Update()
    {
        if (_weaponHandler != null && _isPicked)
        {
            Vector3 scale = _weaponHandler.Holder.localScale;
            Vector3 localScale = transform.localScale;
            transform.position = Vector3.Slerp(transform.position, _weaponHandler.Holder.position, _movingSpeed * Time.deltaTime);
            transform.localScale = new Vector3(localScale.x * scale.x, localScale.y, localScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && _isPicked == false)
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
        }
    }

    void Pick()
    {
        _weaponHandler.SetWeapon(this);
        _isPicked = true;
        _pickUpFeedBack.PlayFeedbacks();
        _weaponHandler.AttackEvent += Attack;
    }

    void Drop()
    {
        _weaponHandler.DisableWeapon();
        transform.SetParent(null);
        _weaponHandler.AttackEvent -= Attack;
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
