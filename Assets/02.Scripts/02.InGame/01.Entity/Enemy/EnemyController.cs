using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    //적의 중심.
    private readonly Dictionary<Type, EnemyAbility> _abilitiesCache = new();

    [SerializeField] private GameObject _hitbox;

    public EnemyStat Stat;
    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public Collider HitCollider { get; private set; }

    public EnemyStateMachine FSM { get; private set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        HitCollider = _hitbox.GetComponent<Collider>();
    }

    private void Start()
    {
        FSM = new EnemyStateMachine(this);
    }

    public T GetAbility<T>() where T : EnemyAbility
    {
        var type = typeof(T);

        if (_abilitiesCache.TryGetValue(type, out EnemyAbility ability))
        {
            return ability as T;
        }

        ability = GetComponent<T>();

        if (ability != null)
        {
            _abilitiesCache[ability.GetType()] = ability;
            return ability as T;
        }

        throw new Exception($"Ability {type.Name} not found on {gameObject.name}.");
    }
    
    public void TakeDamage(float damage, int attackerActorNumber)
    {
        EnemyHealthAbility health = GetAbility<EnemyHealthAbility>();
        if (health.IsDead)
        {
            return;
        }

        health.TryDecreaseHealth(damage, attackerActorNumber);
    }
}
