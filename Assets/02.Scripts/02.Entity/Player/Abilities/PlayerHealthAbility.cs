using UnityEngine;

public class PlayerHealthAbility : PlayerAbility
{
    public ConsumableStat Health { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InitializeHealth();
    }

    public void ApplyNetworkState(float healthCurrent)
    {
        Health?.SetCurrent(healthCurrent);
    }

    private void InitializeHealth()
    {
        if (_owner == null || _owner.Stat == null)
        {
            return;
        }

        Health = new ConsumableStat(_owner.Stat.MaxHealth, _owner.Stat.MaxHealth, _owner.Stat.RegenerateHealth);
    }

    public bool TryTakeDamage(float damage)
    {
        if (Health.TryConsume(damage))
        {
            Debug.Log("HP감소");
            if (Health.IsEmpty)
            {
                //사망 처리
            }
            return true;
        }

        return false;
    }
}
