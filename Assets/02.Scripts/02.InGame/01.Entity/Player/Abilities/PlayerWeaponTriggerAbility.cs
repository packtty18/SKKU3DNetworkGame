using Photon.Pun;
using UnityEngine;

public class PlayerWeaponTriggerAbility : PlayerAbility
{
    private void OnTriggerEnter(Collider other)
    {
        if (_owner.IsDead || !_owner.PhotonView.IsMine) return;
        
        if (other.transform == _owner.transform) return;

        if (other.TryGetComponent(out IDamageable damageable))
        {
            Debug.Log("칼에 맞음");
            //damageable.TakeDamage(_owner.Stat.AttackDamage);
            //내가 아닌 상대방에게 데미지 적용
            PlayerController target = other.GetComponent<PlayerController>();
            target.PhotonView.RPC(nameof(damageable.TakeDamage), RpcTarget.All, _owner.Stat.AttackDamage);
            
            _owner.GetAbility<PlayerWeaponColliderAbility>().DeactiveCollider();
        }
    }
}
