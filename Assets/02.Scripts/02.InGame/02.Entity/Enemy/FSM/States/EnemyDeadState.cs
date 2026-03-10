public class EnemyDeadState : EnemyStateBase
{
    public EnemyDeadState(EnemyController owner) : base(owner)
    {
    }

    public override EnemyStateId Id => EnemyStateId.Dead;

    public override void Enter()
    {
        Owner.MoveAbility.StopMove();
        Owner.SetCollisionEnabled(false);
        Owner.ClearCurrentTarget();
        Owner.RequestRespawn();
    }
}
