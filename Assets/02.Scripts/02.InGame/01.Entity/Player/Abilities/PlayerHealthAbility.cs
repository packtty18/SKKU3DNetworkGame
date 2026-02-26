using Photon.Pun;
using UnityEngine;

public class PlayerHealthAbility : PlayerAbility
{
    private static readonly int OnDeadHash = Animator.StringToHash("OnDead");
    private static readonly int OnHitHash = Animator.StringToHash("OnHit");

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
    
    public void ResetState()
    {
        Health.SetCurrent(_owner.Stat.MaxStamina);
    }


    public bool TryDecreaseHealth(float damage, int attackerActorNumber)
    {
        if (!_owner.PhotonView.IsMine)
        {
            return false;
        }
        
        if (Health == null || damage <= 0f || Health.IsEmpty)
        {
            return false;
        }
        
        Health.SetCurrent(Health.Current - damage);

        if (Health.IsEmpty)
        {
            PhotonRoomManager.Instance.TryOnPlayerDeath(attackerActorNumber, PhotonNetwork.LocalPlayer.ActorNumber);
            TryPlayerDead();
        }
        else
        {
            TryPlayerHit();
        }

        return true;
    }

    private void TryPlayerDead()
    {
        _owner?.TryCollisionEnabled(false);
        PlayDead();

        if (_owner == null || _owner.PhotonView == null || !_owner.PhotonView.IsMine)
        {
            return;
        }

        _owner.PhotonView.RPC(nameof(RpcPlayDead), RpcTarget.Others);
    }

    [PunRPC]
    private void RpcPlayDead()
    {
        PlayDead();
    }

    private void PlayDead()
    {
        if (_owner.Animator == null)
        {
            return;
        }

        _owner.Animator.SetTrigger(OnDeadHash);
    }

    private void TryPlayerHit()
    {
        PlayHit();

        if (_owner == null || _owner.PhotonView == null || !_owner.PhotonView.IsMine)
        {
            return;
        }

        _owner.PhotonView.RPC(nameof(RpcPlayHit), RpcTarget.Others);
    }

    [PunRPC]
    private void RpcPlayHit()
    {
        PlayHit();
    }

    private void PlayHit()
    {
        if (_owner.Animator == null)
        {
            return;
        }

        _owner.Animator.SetTrigger(OnHitHash);
    }
}
