using UnityEngine;

public class PlayerStaminaAbility : PlayerAbility
{
    private const float STAMINA_REGEN_DELAY = 3f;

    public ConsumableStat Stamina { get; private set; }

    private float _staminaRegenCooldown;

    protected override void Awake()
    {
        base.Awake();
        InitializeResources();
    }

    private void Update()
    {
        if (_owner.IsDead || _owner.PhotonView == null || !_owner.PhotonView.IsMine || Stamina == null)
        {
            return;
        }

        if (_staminaRegenCooldown > 0f)
        {
            _staminaRegenCooldown -= Time.deltaTime;
            return;
        }

        Stamina.TryRegenerate(Time.deltaTime);
    }
    
    private void InitializeResources()
    {
        if (_owner == null || _owner.Stat == null)
        {
            return;
        }

        Stamina = new ConsumableStat(_owner.Stat.MaxStamina, _owner.Stat.MaxStamina, _owner.Stat.RegenerateStamina);
        _staminaRegenCooldown = 0f;
    }
    
    public void ApplyNetworkState(float staminaCurrent)
    {
        Stamina?.SetCurrent(staminaCurrent);
    }
    
    public void ResetState()
    {
        Stamina.SetCurrent(_owner.Stat.MaxStamina);
        _staminaRegenCooldown = 0f;
    }

    public bool TryUseStamina(float amount)
    {
        if (_owner.IsDead || Stamina == null)
        {
            return false;
        }

        if (!Stamina.TryConsume(amount))
        {
            return false;
        }

        _staminaRegenCooldown = STAMINA_REGEN_DELAY;

        return true;
    }
}
