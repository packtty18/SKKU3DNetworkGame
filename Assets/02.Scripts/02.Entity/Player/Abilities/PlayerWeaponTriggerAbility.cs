using UnityEngine;

public class PlayerWeaponTriggerAbility : PlayerAbility
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == _owner.transform) return;

        if (other.TryGetComponent(out IDamageable damageable))
        {
            Debug.Log("칼에 맞음");
            damageable.TakeDamage(_owner.Stat.AttackDamage);
        }
    }
}