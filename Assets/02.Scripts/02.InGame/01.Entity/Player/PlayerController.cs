using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(PhotonView))]
public class PlayerController : MonoBehaviour, IDamageable
{
    private const float RESPAWN_DELAY = 5f;

    public PhotonView PhotonView { get; private set; }
    public Animator Animator { get; private set; }
    
    public EntityStat Stat;
    public PlayerInputs Inputs => GetAbility<PlayerInputAbility>()?.Inputs;

    public int Score = 0;
    public bool IsDead => _healthAbility.Health.IsEmpty;
    public bool Exhausted => _staminaAbility.Exhausted;
    
    
    private PlayerHealthAbility _healthAbility => GetAbility<PlayerHealthAbility>();
    private PlayerStaminaAbility _staminaAbility => GetAbility<PlayerStaminaAbility>();
    
    private readonly Dictionary<Type, PlayerAbility> _abilitiesCache = new();
    private bool _isRespawning;

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
                SpawnCoins();
                StartCoroutine(RespawnAfterDelay());
            }
        }
    }

    private void SpawnCoins()
    {
        int counts = UnityEngine.Random.Range(1, 10);

        for (int i = 0; i < counts; i++)
        {
            PhotonNetwork.Instantiate("Coin", transform.position, transform.rotation);
        }
    }

    public void GetCoins(ScoreItem item)
    {
        Score += 10;
        PhotonNetwork.Destroy(item.gameObject);
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
