using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    private static readonly int IsPatrolHash = Animator.StringToHash("IsPatrol");
    private static readonly int IsChaseHash = Animator.StringToHash("IsChase");

    [SerializeField] private GameObject _hitbox;
    [SerializeField] private EnemySpawnManager _spawnManager;

    private readonly Dictionary<Type, EnemyAbility> _abilitiesCache = new();
    private bool _isFirstIdleCycle = true;

    public EnemyStat Stat;

    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public Collider HitCollider { get; private set; }
    public EnemyStateMachine FSM { get; private set; }
    public PlayerController CurrentTarget { get; private set; }

    public bool IsDead => HealthAbility != null && HealthAbility.IsDead;

    public EnemyMoveAbility MoveAbility => GetAbility<EnemyMoveAbility>();
    public EnemyAttackAbility AttackAbility => GetAbility<EnemyAttackAbility>();
    public EnemyHealthAbility HealthAbility => GetAbility<EnemyHealthAbility>();
    public EnemyTargetingAbility TargetingAbility => GetAbility<EnemyTargetingAbility>();

    private void Awake()
    {
        if (Stat == null)
        {
            Stat = new EnemyStat();
        }

        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();

        if (_hitbox != null)
        {
            HitCollider = _hitbox.GetComponent<Collider>();
        }

        if (_spawnManager == null)
        {
            _spawnManager = EnemySpawnManager.Instance;
        }
    }

    private void Start()
    {
        FSM = new EnemyStateMachine(this);
        ResetLocomotionFlags();
        FSM.Change(EnemyStateId.Idle);
    }

    private void Update()
    {
        if (FSM == null)
        {
            return;
        }

        FSM.Tick(Time.deltaTime);
    }

    public T GetAbility<T>() where T : EnemyAbility
    {
        Type type = typeof(T);

        if (_abilitiesCache.TryGetValue(type, out EnemyAbility cachedAbility))
        {
            return cachedAbility as T;
        }

        T ability = GetComponent<T>();

        if (ability != null)
        {
            _abilitiesCache[type] = ability;
            return ability;
        }

        throw new Exception($"Ability {type.Name} not found on {gameObject.name}.");
    }

    public void TakeDamage(float damage, int attackerActorNumber)
    {
        if (IsDead)
        {
            return;
        }

        HealthAbility.TryDecreaseHealth(damage, attackerActorNumber);
    }

    public void NotifyHit()
    {
        if (IsDead || FSM == null)
        {
            return;
        }

        FSM.Change(EnemyStateId.Hit);
    }

    public void NotifyDead()
    {
        if (FSM == null)
        {
            return;
        }

        FSM.Change(EnemyStateId.Dead);
    }

    public void SetCurrentTarget(PlayerController target)
    {
        CurrentTarget = target;
    }

    public void ClearCurrentTarget()
    {
        CurrentTarget = null;
    }

    public bool TryAcquireTarget(float searchRange)
    {
        if (!TargetingAbility.TryFindClosestTarget(searchRange, out PlayerController target))
        {
            return false;
        }

        CurrentTarget = target;
        return true;
    }

    public bool IsCurrentTargetInRange(float range)
    {
        if (CurrentTarget == null || CurrentTarget.IsDead)
        {
            return false;
        }

        return TargetingAbility.IsInRange(CurrentTarget, range);
    }

    public void SetPatrolAnimation(bool isPatrol)
    {
        if (Animator == null)
        {
            return;
        }

        Animator.SetBool(IsPatrolHash, isPatrol);
    }

    public void SetChaseAnimation(bool isChase)
    {
        if (Animator == null)
        {
            return;
        }

        Animator.SetBool(IsChaseHash, isChase);
    }

    public void ResetLocomotionFlags()
    {
        SetPatrolAnimation(false);
        SetChaseAnimation(false);
    }

    public void SetCollisionEnabled(bool enabled)
    {
        Collider ownerCollider = GetComponent<Collider>();

        if (ownerCollider != null)
        {
            ownerCollider.enabled = enabled;
        }

        if (HitCollider != null)
        {
            HitCollider.enabled = enabled;
        }
    }

    public void RequestRespawn()
    {
        float respawnDelay = Stat != null ? Mathf.Max(0f, Stat.RespawnDelay) : 0f;

        if (_spawnManager != null)
        {
            _spawnManager.RequestRespawn(this, respawnDelay);
            return;
        }

        StartCoroutine(RespawnFallback());
    }

    public void Respawn(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);

        MoveAbility.Warp(position);
        MoveAbility.StopMove();

        HealthAbility.ResetStat();
        SetCollisionEnabled(true);
        ClearCurrentTarget();
        ResetLocomotionFlags();
        _isFirstIdleCycle = true;

        FSM.Change(EnemyStateId.Idle);
    }

    public float ConsumeIdleDelay()
    {
        if (Stat == null)
        {
            return 0f;
        }

        if (_isFirstIdleCycle)
        {
            _isFirstIdleCycle = false;
            return Mathf.Max(0f, Stat.PatrolStartDelay);
        }

        return Mathf.Max(0f, Stat.PatrolWaitDelay);
    }

    private IEnumerator RespawnFallback()
    {
        float respawnDelay = Stat != null ? Mathf.Max(0f, Stat.RespawnDelay) : 0f;
        yield return new WaitForSeconds(respawnDelay);

        Vector3 respawnPosition = transform.position;
        Quaternion respawnRotation = transform.rotation;

        float patrolRadius = Stat != null ? Mathf.Max(2f, Stat.PatrolRadius) : 2f;
        if (MoveAbility.TryGetRandomReachablePoint(transform.position, patrolRadius, out Vector3 randomPoint))
        {
            respawnPosition = randomPoint;
        }

        Respawn(respawnPosition, respawnRotation);
    }
}
