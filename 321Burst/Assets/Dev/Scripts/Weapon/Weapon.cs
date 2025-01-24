using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected float _movingSpeed;
    [SerializeField] protected MMF_Player _attackFeedback;
    [SerializeField] protected MMF_Player _hitFeedback;
    [SerializeField] protected MMF_Player _pickUpFeedBack;

    protected Vector3 _startScale;
    protected WeaponHandler _weaponHandler;
    protected Collider2D _target;
    protected bool _isPicked;

    protected virtual void Start()
    {
        _startScale = transform.localScale;
    }

    protected virtual void Update()
    {
        SetFatherBehavior();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && _isPicked == false)
        {
            _weaponHandler = collision.GetComponent<WeaponHandler>();
            _target = collision;
            _weaponHandler.OnPick += PickOrDrop;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _isPicked==false)
        {
            _weaponHandler = collision.GetComponent<WeaponHandler>();
            _target = null;
            _weaponHandler.OnPick -= PickOrDrop;
        }
    }

    protected virtual void PickOrDrop()
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

    protected virtual void Pick()
    {
        _weaponHandler.SetWeapon(this);
        _isPicked = true;
        _pickUpFeedBack.PlayFeedbacks();
        _weaponHandler.AttackEvent += Attack;
    }

    protected virtual void Drop()
    {
        _weaponHandler.DisableWeapon();
        transform.SetParent(null);
        _weaponHandler.AttackEvent -= Attack;
        _weaponHandler = null;
        _isPicked=false;
    }

    public virtual void Attack()
    {
        _attackFeedback.PlayFeedbacks();
    }

    public virtual void HitFeedBack()
    {
        _hitFeedback.PlayFeedbacks();
    }

    protected virtual void SetFatherBehavior()
    {
        if (_weaponHandler != null && _isPicked)
        {
            transform.position = Vector3.Slerp(transform.position, _weaponHandler.Holder.position, _movingSpeed * Time.deltaTime);

            if (_weaponHandler.transform.localScale.x < 0)
                transform.localScale = new Vector3(-_startScale.x, _startScale.y, _startScale.z);
            else if (_weaponHandler.transform.localScale.x > 0)
                transform.localScale = new Vector3(_startScale.x, _startScale.y, _startScale.z);
        }
    }
}
