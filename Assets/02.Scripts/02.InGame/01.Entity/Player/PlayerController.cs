using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerController : MonoBehaviour, IDamageable
{
    private readonly Dictionary<Type, PlayerAbility> _abilitiesCache = new();

    public PhotonView PhotonView { get; private set; }
    public Animator Animator { get; private set; }
    public EntityStat Stat;
    
    private PlayerHealthAbility _healthAbility => GetAbility<PlayerHealthAbility>();
    private PlayerStaminaAbility _staminaAbility => GetAbility<PlayerStaminaAbility>();
    public PlayerInputs Inputs => GetAbility<PlayerInputAbility>()?.Inputs;

    public bool IsDead => _healthAbility.Health.IsEmpty;
    public bool Exhausted=>  _staminaAbility.Exhausted;

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
        Animator = GetComponent<Animator>();
    }

    public bool TryUseStamina(float amount)
    {
        return _staminaAbility != null && _staminaAbility.TryUseStamina(amount);
    }

    public bool TryGetNetworkResourceState(out float health, out float stamina)
    {
        health = 0f;
        stamina = 0f;

        if (_healthAbility == null || _staminaAbility == null)
        {
            return false;
        }

        health = _healthAbility.Health != null ? _healthAbility.Health.Current : 0f;
        stamina = _staminaAbility.Stamina != null ? _staminaAbility.Stamina.Current : 0f;
        return true;
    }

    public void ApplyNetworkResourceState(float health, float stamina)
    {
        _healthAbility?.ApplyNetworkState(health);
        _staminaAbility?.ApplyNetworkState(stamina);
    }

    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);

        if (_abilitiesCache.TryGetValue(type, out PlayerAbility ability))
        {
            return ability as T;
        }

        ability = GetComponent<T>();

        if (ability != null)
        {
            _abilitiesCache[ability.GetType()] = ability;
            return ability as T;
        }

        throw new Exception($"어빌리티 {type.Name}를 {gameObject.name}에서 찾을 수 없습니다.");
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        GetAbility<PlayerNetworkSyncAbility>()?.OnPhotonSerializeView(stream, info);
    }
    
    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (_healthAbility.TryTakeDamage(damage))
        {
            Debug.Log("데미지처리 성공");
        }
        else
        {
            Debug.Log("데미지처리 실패");
        }
        
    }
}
