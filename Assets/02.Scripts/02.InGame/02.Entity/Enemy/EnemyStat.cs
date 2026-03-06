using System;

[Serializable]
public class EnemyStat
{
    public float MaxHealth = 100f;
    public float RegenerateHealth = 0f;
    public float MoveSpeed = 3.5f;
    public float AttackDelay = 1.25f;
    public float AttackDamage = 5f;
    public float RotateSpeed = 120f;

    public float PatrolStartDelay = 1.5f;
    public float PatrolWaitDelay = 2f;
    public float PatrolRadius = 12f;
    public float ChaseDistance = 10f;
    public float AttackDistance = 2f;

    public float HitDuration = 0.4f;
    public float RespawnDelay = 5f;
}
