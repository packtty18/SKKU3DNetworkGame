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
    
    [Header("memeber")]
    public EntityStat Stat;
    public int Score = 0;
    private readonly Dictionary<Type, PlayerAbility> _abilitiesCache = new();
    private bool _isRespawning;
    
    [Header("Property")]
    public bool IsDead => _healthAbility.Health.IsEmpty;
    
    public PlayerInputs Inputs => GetAbility<PlayerInputAbility>()?.Inputs;
    private PlayerHealthAbility _healthAbility => GetAbility<PlayerHealthAbility>();
    private PlayerStaminaAbility _staminaAbility => GetAbility<PlayerStaminaAbility>();

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

                ItemObjectFactory.Instance.RequestSpawnCoins(transform.position + new Vector3(0,1,0),counts);
                
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
        //랜덤위치로 이동
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        
        //애니메이션 초기화
        Animator?.Rebind();
        Animator?.Update(0f);
        
        //체력,스태미너 초기화
        _healthAbility?.ResetStat();
        _staminaAbility?.ResetStat();
        
        //충돌활성화
        SetCollisionEnabled(true);
        
        _isRespawning = false;
    }

    public void TryCollisionEnabled(bool enabled)
    {
        if (!PhotonView.IsMine)
        {
            return;
        }
        
        //내 상태를 바꾸고 모든 사람에게 전달함
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
}
