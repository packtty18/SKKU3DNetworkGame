using Photon.Pun;
using UnityEngine;

public class PlayerWeaponHitAbility : PlayerAbility
{
    private void OnTriggerEnter(Collider other)
    {
        if (_owner.IsDead || !_owner.PhotonView.IsMine) return;
        
        if (other.transform == _owner.transform) return;

        if (other.TryGetComponent(out IDamageable damageable))
        {
            
            //or _onwer.PhotonView.Owner.ActorNumber
            //damageable.TakeDamage(_owner.Stat.AttackDamage);
            //내가 아닌 상대방에게 데미지 적용
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            if (other.TryGetComponent(out PlayerController player))
            {
                
                player.PhotonView.RPC(nameof(damageable.TakeDamage), RpcTarget.All, _owner.Stat.AttackDamage,actorNumber);
                _owner.GetAbility<PlayerWeaponColliderAbility>().DeactiveCollider();
            }
            else
            {
                damageable.TakeDamage(_owner.Stat.AttackDamage,actorNumber);
            }
        }
    }
}
