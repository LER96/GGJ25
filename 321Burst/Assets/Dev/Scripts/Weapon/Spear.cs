using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon
{
    [Header("Attack Sequence")]
    [SerializeField] Transform _pos;
    [SerializeField] Collider2D _spearCollider;
    [SerializeField] float _attackDuration;
    [SerializeField] float _waitDuration;
    protected bool _goBack=true;
    public override void Attack()
    {
        if (_goBack)
        {
            _goBack = false;
            _spearCollider.enabled = true;
            transform.DOMove(_pos.position, _attackDuration).OnComplete(ColliderDelay);
        }
    }

    void ColliderDelay()
    {
        StartCoroutine(ResetCollider());

    }

    IEnumerator ResetCollider()
    {
        yield return new WaitForSeconds(_waitDuration);
        _spearCollider.enabled = false;
        _goBack = true;
    }

    protected override void SetFatherBehavior()
    {
        if (_weaponHandler != null && _isPicked && _goBack)
        {
            transform.position = Vector3.Slerp(transform.position, _weaponHandler.Holder.position, _movingSpeed * Time.deltaTime);

            if (_weaponHandler.transform.localScale.x < 0)
                transform.localScale = new Vector3(-_startScale.x, _startScale.y, _startScale.z);
            else if (_weaponHandler.transform.localScale.x > 0)
                transform.localScale = new Vector3(_startScale.x, _startScale.y, _startScale.z);
        }
    }
}
