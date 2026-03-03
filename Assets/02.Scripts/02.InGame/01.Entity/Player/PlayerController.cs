using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PhotonView))]
public class PlayerController : MonoBehaviour, IDamageable
{
    private const float RESPAWN_DELAY = 3f;

    [Header("reference")]
    public PhotonView PhotonView { get; private set; }
    public Animator Animator { get; private set; }
    public Collider Collision { get; private set; }
    public CharacterController CharacterController { get; private set; }
    [SerializeField] private GameObject _weaponGameObject;

    [Header("memeber")]
    public EntityStat Stat;
    private readonly Dictionary<Type, PlayerAbility> _abilitiesCache = new();
    private bool _isRespawning;

    [Header("Property")]
    public bool IsDead => _healthAbility.Health.IsEmpty;

    public PlayerInputs Inputs => GetAbility<PlayerInputAbility>()?.Inputs;
    private PlayerHealthAbility _healthAbility => GetAbility<PlayerHealthAbility>();
    private PlayerStaminaAbility _staminaAbility => GetAbility<PlayerStaminaAbility>();


    private void OnEnable()
    {
        PlayerRegistryManager.Instance.RegisterPlayer(this);
        ScoreManager.OnMyScoreChanged += TrySetWeaponScale;
    }

    private void OnDisable()
    {
        PlayerRegistryManager.Instance.UnregisterPlayer(this);
        ScoreManager.OnMyScoreChanged -= TrySetWeaponScale;
    }

    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
        Animator = GetComponent<Animator>();
        Collision = GetComponent<Collider>();
        CharacterController = GetComponent<CharacterController>();
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
                int counts = Random.Range(1, 10);

                ItemObjectFactory.Instance.RequestSpawnCoins(transform.position + new Vector3(0, 1, 0), counts);
                ScoreManager.Instance.SubtractScore((int)(ScoreManager.Instance.MyScore * 0.5f));
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

        Respawn(spawnPosition, spawnRotation);
        PhotonView.RPC(nameof(RpcRespawn), RpcTarget.Others, spawnPosition, spawnRotation);
    }

    [PunRPC]
    private void RpcRespawn(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        Respawn(spawnPosition, spawnRotation);
    }

    private void Respawn(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        Animator?.Rebind();
        Animator?.Update(0f);

        _healthAbility?.ResetStat();
        _staminaAbility?.ResetStat();

        SetCollisionEnabled(true);

        _isRespawning = false;
    }

    public void TryCollisionEnabled(bool enabled)
    {
        if (!PhotonView.IsMine)
        {
            return;
        }

        SetCollisionEnabled(enabled);
        PhotonView.RPC(nameof(RpcSetCollisionEnabled), RpcTarget.Others, enabled);
    }

    [PunRPC]
    private void RpcSetCollisionEnabled(bool enabled)
    {
        SetCollisionEnabled(enabled);
    }

    private void SetCollisionEnabled(bool enabled)
    {
        if (Collision == null)
        {
            return;
        }

        Collision.enabled = enabled;
    }

    public void TrySetWeaponScale(int level)
    {
        if (!PhotonView.IsMine)
        {
            return;
        }
    
        SetWeaponScale(level);
        PhotonView.RPC(nameof(RpcSetWeaponScale), RpcTarget.Others, level);
    }
    
    [PunRPC]
    private void RpcSetWeaponScale(int level)
    {
        SetWeaponScale(level);
    }
    
    private void SetWeaponScale(int level)
    {
        if (_weaponGameObject == null)
        {
            return;
        }
    
        float scale = 1 + level * 1f; //임시로 1, 원래는 0.1
        Debug.Log($"무기의 크기 변경 : {scale}");
        _weaponGameObject.transform.localScale = new Vector3(scale, scale, scale);
    }
}
