using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(PhotonView))]
public class PlayerController : MonoBehaviour, IDamageable
{
    private const float RESPAWN_DELAY = 3f;

    [Header("reference")]
    public PhotonView PhotonView { get; private set; }
    public Animator Animator { get; private set; }
    
    [Header("memeber")]
    public EntityStat Stat;
    public int Score = 0;
    private readonly Dictionary<Type, PlayerAbility> _abilitiesCache = new();
    private bool _isRespawning;
    
    [Header("Property")]
    public bool IsDead => _healthAbility.Health.IsEmpty;
    public bool Exhausted => _staminaAbility.Exhausted;
    
    public PlayerInputs Inputs => GetAbility<PlayerInputAbility>()?.Inputs;
    private PlayerHealthAbility _healthAbility => GetAbility<PlayerHealthAbility>();
    private PlayerStaminaAbility _staminaAbility => GetAbility<PlayerStaminaAbility>();
    
    

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
        Animator = GetComponent<Animator>();
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        GetAbility<PlayerNetworkSyncAbility>()?.OnPhotonSerializeView(stream, info);
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

        throw new Exception($"Ability {type.Name} not found on {gameObject.name}.");
    }

    [PunRPC]
    public void TakeDamage(float damage, int attackerActorNumber)
    {
        if (IsDead || _isRespawning)
        {
            return;
        }

        if (_healthAbility.TryDecreaseHealth(damage, attackerActorNumber))
        {
            if (IsDead && PhotonView != null && PhotonView.IsMine && !_isRespawning)
            {
                ItemObjectFactory.Instance.RequestSpawnCoins(transform.position + new Vector3(0,1,0));
                
                StartCoroutine(RespawnAfterDelay());
            }
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        _isRespawning = true;
        yield return new WaitForSeconds(RESPAWN_DELAY);

        if (PhotonView == null || !PhotonView.IsMine)
        {
            _isRespawning = false;
            yield break;
        }

        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = transform.rotation;

        if (PlayerSpawnManager.Instance != null &&
            PlayerSpawnManager.Instance.TryGetRandomSpawnPoint(out Vector3 point, out Quaternion rotation))
        {
            spawnPosition = point;
            spawnRotation = rotation;
        }

        PhotonView.RPC(nameof(RpcRespawn), RpcTarget.All, spawnPosition, spawnRotation);
    }

    [PunRPC]
    private void RpcRespawn(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        if (characterController != null)
        {
            characterController.enabled = true;
        }

        Animator?.Rebind();
        Animator?.Update(0f);
        _healthAbility?.ApplyNetworkState(Stat.MaxHealth);
        _staminaAbility?.ResetState();
        _isRespawning = false;
    }
}
