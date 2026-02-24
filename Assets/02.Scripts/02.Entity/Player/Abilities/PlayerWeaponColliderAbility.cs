using System;
using UnityEngine;

public class PlayerWeaponColliderAbility : PlayerAbility
{
    [SerializeField] private Collider _collider;

    private void Start()
    {
        DeactiveCollider();
    }

    //애니메이션 이벤트
    public void ActiveCollider()
    {
        _collider.enabled = true;
    }
    
    //애니메이션 이벤트
    public void DeactiveCollider()
    {
        _collider.enabled = false;
    }
}