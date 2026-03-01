using UnityEngine;

public class EnemyAttackState : EnemyStateBase
{
    public EnemyAttackState(EnemyController owner) : base(owner)
    {
    }

    public override EnemyStateId Id => EnemyStateId.Attack;

    public override void Enter()
    {
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

        if (!Owner.IsCurrentTargetInRange(Owner.Stat.AttackDistance))
        {
            if (Owner.IsCurrentTargetInRange(Owner.Stat.ChaseDistance))
            {
                Owner.FSM.Change(EnemyStateId.Chase);
                return;
            }

            if (!Owner.TryAcquireTarget(Owner.Stat.ChaseDistance))
            {
                Owner.ClearCurrentTarget();
                Owner.FSM.Change(EnemyStateId.Idle);
                return;
            }

            if (!Owner.IsCurrentTargetInRange(Owner.Stat.AttackDistance))
            {
                Owner.FSM.Change(EnemyStateId.Chase);
                return;
            }
        }

        Owner.MoveAbility.RotateToward(Owner.CurrentTarget.transform.position, deltaTime);
        Owner.AttackAbility.TryAttack();
    }

    public override void Exit()
    {
        Owner.AttackAbility.CancelAttack();
    }
}
