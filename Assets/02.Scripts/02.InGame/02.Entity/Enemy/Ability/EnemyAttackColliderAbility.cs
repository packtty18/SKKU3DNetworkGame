using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemyAttackColliderAbility : EnemyAbility
{
    [Header("Hitboxes")]
    [SerializeField] private Collider _leftFootCollider;
    [SerializeField] private Collider _rightFootCollider;
    [SerializeField] private Collider _biteCollider;
    [SerializeField] private Collider _smashCollider;

    private readonly HashSet<int> _hitTargets = new();
    private readonly Dictionary<int, Collider> _attackColliderMap = new();

    private int _currentAttackNumber;

    protected override void Awake()
    {
        base.Awake();
        CacheAttackColliders();
        DeactivateAllColliders();
    }

    public void BeginAttack(int attackNumber)
    {
        _currentAttackNumber = attackNumber;
        _hitTargets.Clear();
    }

    public void EndAttack()
    {
        _currentAttackNumber = 0;
        _hitTargets.Clear();
        DeactivateAllColliders();
    }

    public void ActivateAttackCollider(int attackNumber)
    {
        BeginAttack(attackNumber);
        DeactivateAllColliders();

        if (_attackColliderMap.TryGetValue(attackNumber, out Collider collider))
        {
            collider.enabled = true;
        }
    }

    // Animation event.
    public void DeactivateAllColliders()
    {
        SetColliderEnabled(_leftFootCollider, false);
        SetColliderEnabled(_rightFootCollider, false);
        SetColliderEnabled(_biteCollider, false);
        SetColliderEnabled(_smashCollider, false);
    }

    // Animation event.
    public void ActivateLeftFootCollider()
    {
        ActivateAttackCollider(1);
    }

    // Animation event.
    public void ActivateRightFootCollider()
    {
        ActivateAttackCollider(2);
    }

    // Animation event.
    public void ActivateBiteCollider()
    {
        ActivateAttackCollider(3);
    }

    // Animation event.
    public void ActivateSmashCollider()
    {
        ActivateAttackCollider(4);
    }

    public void HandleHitboxTriggerEnter(Collider other)
    {
        if (_owner == null || _owner.IsDead || _currentAttackNumber <= 0 || other == null)
        {
            return;
        }

        if (other.transform.IsChildOf(_owner.transform))
        {
            return;
        }

        if (!other.TryGetComponent(out IDamageable damageable))
        {
            return;
        }

        if (other.TryGetComponent(out PlayerController playerController) && playerController.IsDead)
        {
            return;
        }

        int instanceId = other.transform.root.GetInstanceID();
        if (_hitTargets.Contains(instanceId))
        {
            return;
        }

        _hitTargets.Add(instanceId);
        ApplyDamage(damageable, _owner.Stat.AttackDamage);
    }

    private void CacheAttackColliders()
    {
        _attackColliderMap.Clear();

        CacheAttackCollider(1, _leftFootCollider);
        CacheAttackCollider(2, _rightFootCollider);
        CacheAttackCollider(3, _biteCollider);
        CacheAttackCollider(4, _smashCollider);
    }

    private void CacheAttackCollider(int attackNumber, Collider collider)
    {
        if (collider == null)
        {
            return;
        }

        collider.isTrigger = true;
        _attackColliderMap[attackNumber] = collider;

        EnemyAttackHitbox hitbox = collider.GetComponent<EnemyAttackHitbox>();
        if (hitbox == null)
        {
            hitbox = collider.gameObject.AddComponent<EnemyAttackHitbox>();
        }

        hitbox.Initialize(this);
    }

    private static void SetColliderEnabled(Collider collider, bool isEnabled)
    {
        if (collider == null)
        {
            return;
        }

        collider.enabled = isEnabled;
    }

    private static void ApplyDamage(IDamageable target, float damage)
    {
        if (target is PlayerController playerController && playerController.PhotonView != null)
        {
            playerController.PhotonView.RPC(
                nameof(PlayerController.TakeDamage),
                RpcTarget.All,
                damage,
                -1
            );
            return;
        }

        target.TakeDamage(damage, -1);
    }
}
