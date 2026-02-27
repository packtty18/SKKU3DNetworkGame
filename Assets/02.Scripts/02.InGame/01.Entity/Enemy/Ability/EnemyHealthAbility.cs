using UnityEngine;

public class EnemyHealthAbility : EnemyAbility
{
    public ConsumableStat Health { get; private set; }
    
    public bool IsDead => Health.IsEmpty;
    protected override void Awake()
    {
        base.Awake();
        InitializeHealth();
    }
    
    private void InitializeHealth()
    {
        if (_owner == null || _owner.Stat == null)
        {
            return;
        }

        Health = new ConsumableStat(_owner.Stat.MaxHealth, _owner.Stat.MaxHealth, _owner.Stat.RegenerateHealth);
    }
    
    public bool TryDecreaseHealth(float damage, int attackerActorNumber)
    {
        if (!Health.TryConsume(damage, true))
        {
            return false;
        }
        
        if (Health.IsEmpty)
        {
            OnDead();
        }
        else
        {
            OnHit();
        }

        return true;
    }
    
    private void OnDead()
    {
        _owner.FSM.Change(EnemyStateId.Dead);
    }
    

    private void OnHit()
    {
        _owner.FSM.Change(EnemyStateId.Hit);
    }
}
