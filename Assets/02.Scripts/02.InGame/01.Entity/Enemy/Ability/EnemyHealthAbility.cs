using UnityEngine;

public class EnemyHealthAbility : EnemyAbility
{
    public ConsumableStat Health { get; private set; }

    public bool IsDead => Health != null && Health.IsEmpty;

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

    public void ResetStat()
    {
        if (Health == null || _owner == null || _owner.Stat == null)
        {
            return;
        }

        Health.SetMax(_owner.Stat.MaxHealth);
        Health.SetCurrent(_owner.Stat.MaxHealth);
    }

    public bool TryDecreaseHealth(float damage, int attackerActorNumber)
    {
        if (Health == null)
        {
            return false;
        }

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
        _owner.NotifyDead();
    }

    private void OnHit()
    {
        _owner.NotifyHit();
    }
}
