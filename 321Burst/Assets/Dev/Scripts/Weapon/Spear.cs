using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon
{
    [Header("Attack Sequence")]
    [SerializeField] Transform _pos;
    [SerializeField] Collider2D _spearCollider;
    [SerializeField] float _attackDistance = 12f;
    [SerializeField] float _colliderExtraDuration = 0.3f;
    [SerializeField] float _returnDuration = 0.2f;

    protected bool FacingRight => transform.localScale.x > 0;
    public override void Attack()
    {
        if (_isAttacking)
        {
            print("already mid attack");
            return;
        }
        if (!_CooldownReady)
        {
            print("cooldown not ready");
            return;
        }

        _owner.SetAnimation(_weaponName);
    }

    public override void PlayerAttack()
    {
        _cooldownTimer = _attackCooldown;
        _isAttacking = true;
        _weaponHandler.Player.PlayerMovement.CanMove = false;
        transform.position = _weaponHandler.Holder.position;

        _spearCollider.enabled = true;
        Vector3 targetPosition = FacingRight ? transform.position + Vector3.right * _attackDistance : transform.position + Vector3.left * _attackDistance;
        transform.DOMove(targetPosition, _attackDuration).OnComplete(ColliderDelay);
    }

    void ColliderDelay()
    {
        StartCoroutine(ResetCollider());
    }

    IEnumerator ResetCollider()
    {
        yield return new WaitForSeconds(_colliderExtraDuration);
        print("spear collider off");
        _spearCollider.enabled = false;
        transform.DOMove(_weaponHandler.Holder.position, _returnDuration).OnComplete(CompleteAttack);
    }

    void CompleteAttack()
    {
        _isAttacking = false;
        _weaponHandler.Player.PlayerMovement.CanMove = true;
    }


}
