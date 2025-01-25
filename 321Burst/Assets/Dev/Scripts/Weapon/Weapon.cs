using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected string _weaponName;
    [SerializeField] protected float _movingSpeed;
    [SerializeField] protected float _attackDuration;
    [SerializeField] protected float _attackCooldown = 1.5f;

    [SerializeField] protected MMF_Player _attackFeedback;
    [SerializeField] protected MMF_Player _hitFeedback;
    [SerializeField] protected MMF_Player _pickUpFeedBack;


    protected Vector3 _startScale;
    protected WeaponHandler _weaponHandler;
    protected PlayerHanlder _owner;
    protected Collider2D _target;
    protected Transform _spawnPoint;
    protected bool _isPicked;
    protected bool _isAttacking = false;
    protected float _cooldownTimer = 0f;


    protected bool _CooldownReady => _cooldownTimer <= 0f;

    public string WeaponName=> _weaponName;
    public PlayerHanlder Owner { get { return _owner; } }
    public Transform SpawnPoint => _spawnPoint;

    protected virtual void Start()
    {
        _startScale = transform.localScale;
    }

    protected virtual void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;

        if (!_isAttacking)
        {
            SetFatherPosition();
            SetFatherDirection();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _isPicked == false)
        {
            _weaponHandler = collision.GetComponent<WeaponHandler>();
            _target = collision;
            _weaponHandler.OnPick += PickOrDrop;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _isPicked == false)
        {
            _weaponHandler = collision.GetComponent<WeaponHandler>();
            _target = null;
            _weaponHandler.OnPick -= PickOrDrop;
        }
    }

    public void SetSpawnPoint(Transform sp)
    {
        _spawnPoint = sp;
    }

    protected virtual void PickOrDrop()
    {
        if (_isAttacking)
            return;
        if (!_isPicked && _target != null)
        {
            _weaponHandler = _target.GetComponent<WeaponHandler>();
            if (_weaponHandler != null)
            {
                Pick();
                _pickUpFeedBack.PlayFeedbacks();
            }
        }
        else if (_isPicked)
        {

            Drop();
        }
    }

    protected virtual void Pick()
    {
        if (_spawnPoint != null)
        {
            LevelManager.Instance.MarkSpawnPointAsAvailable(_spawnPoint);
            _spawnPoint = null;
        }
        _weaponHandler.SetWeapon(this);
        _owner = _weaponHandler.Player;
        _isPicked = true;
        _pickUpFeedBack.PlayFeedbacks();
        _weaponHandler.AttackEvent += Attack;
    }

    protected virtual void Drop()
    {
        _owner = null;
        _weaponHandler.DisableWeapon();
        transform.SetParent(null);
        _weaponHandler.AttackEvent -= Attack;
        _weaponHandler = null;
        _isPicked = false;
    }

    public void ForceDropWeapon()
    {
        if(!_isPicked) return;
        Drop();
    }
    public virtual void Attack()
    {
        if (_weaponHandler.Player.dead) return;
        if (_isAttacking) return; // prevent from attacking while mid attack
        if (!_CooldownReady) return; // if attack is on cooldown, cancel attack

        _owner.SetAnimation(_weaponName);
        _cooldownTimer = _attackCooldown;
        _isAttacking = true;
        _weaponHandler.Player.PlayerMovement.CanMove = false;
        transform.position = _weaponHandler.Holder.position;
        //_attackFeedback.PlayFeedbacks();
    }

    public virtual void PlayerAttack()
    {
        _attackFeedback.PlayFeedbacks();
        _isAttacking = false;
    }

    public void SetAttack()
    {
        _isAttacking = false;
    }

    public virtual void HitFeedBack()
    {
        _hitFeedback.PlayFeedbacks();
    }

    protected virtual void SetFatherPosition()
    {
        if (_weaponHandler != null && _isPicked)
        {
            transform.position = Vector3.Slerp(transform.position, _weaponHandler.Holder.position, _movingSpeed * Time.deltaTime);
        }
    }

    protected virtual void SetFatherDirection()
    {
        if (_weaponHandler != null && _isPicked)
        {
            if (_weaponHandler.transform.localScale.x < 0)
                transform.localScale = new Vector3(-_startScale.x, _startScale.y, _startScale.z);
            else if (_weaponHandler.transform.localScale.x > 0)
                transform.localScale = new Vector3(_startScale.x, _startScale.y, _startScale.z);
        }
    }

}
