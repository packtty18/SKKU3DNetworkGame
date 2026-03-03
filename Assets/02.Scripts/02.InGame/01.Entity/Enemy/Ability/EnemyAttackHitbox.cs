using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private EnemyAttackColliderAbility _owner;

    public void Initialize(EnemyAttackColliderAbility owner)
    {
        _owner = owner;
    }

    private void OnTriggerEnter(Collider other)
    {
        _owner?.HandleHitboxTriggerEnter(other);
    }
}
