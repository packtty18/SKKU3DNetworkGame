public class EnemyChaseState : EnemyStateBase
{
    public EnemyChaseState(EnemyController owner) : base(owner)
    {
    }

    public override EnemyStateId Id => EnemyStateId.Chase;

    public override void Enter()
    {
        Owner.SetChaseAnimation(true);
        Owner.MoveAbility.StopMove();
    }

    public override void Tick(float deltaTime)
    {
        if (Owner.CurrentTarget == null || Owner.CurrentTarget.IsDead)
        {
            if (!Owner.TryAcquireTarget(Owner.Stat.ChaseDistance))
            {
                Owner.ClearCurrentTarget();
                Owner.FSM.Change(EnemyStateId.Idle);
                return;
            }
        }

        if (Owner.IsCurrentTargetInRange(Owner.Stat.AttackDistance))
        {
            Owner.FSM.Change(EnemyStateId.Attack);
            return;
        }

        if (!Owner.IsCurrentTargetInRange(Owner.Stat.ChaseDistance))
        {
            if (!Owner.TryAcquireTarget(Owner.Stat.ChaseDistance))
            {
                Owner.ClearCurrentTarget();
                Owner.FSM.Change(EnemyStateId.Idle);
                return;
            }
        }

        Owner.MoveAbility.SetDestination(Owner.CurrentTarget.transform.position);
    }

    public override void Exit()
    {
        Owner.SetChaseAnimation(false);
    }
}
