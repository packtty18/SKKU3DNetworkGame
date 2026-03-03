using UnityEngine;

public class EnemyPatrolState : EnemyStateBase
{
    private const int MaxPatrolSampleCount = 6;

    private Vector3 _destination;

    public EnemyPatrolState(EnemyController owner) : base(owner)
    {
    }

    public override EnemyStateId Id => EnemyStateId.Patrol;

    public override void Enter()
    {
        Owner.SetPatrolAnimation(true);

        bool hasDestination = TryFindPatrolDestination(out _destination);

        if (!hasDestination)
        {
            Owner.FSM.Change(EnemyStateId.Idle);
            return;
        }

        Owner.MoveAbility.SetDestination(_destination);
    }

    public override void Tick(float deltaTime)
    {
        if (Owner.TryAcquireTarget(Owner.Stat.AttackDistance))
        {
            Owner.FSM.Change(EnemyStateId.Attack);
            return;
        }

        if (Owner.TryAcquireTarget(Owner.Stat.ChaseDistance))
        {
            Owner.FSM.Change(EnemyStateId.Chase);
            return;
        }

        if (Owner.MoveAbility.HasReachedDestination())
        {
            Owner.FSM.Change(EnemyStateId.Idle);
        }
    }

    public override void Exit()
    {
        Owner.SetPatrolAnimation(false);
    }

    private bool TryFindPatrolDestination(out Vector3 destination)
    {
        destination = Owner.transform.position;

        for (int index = 0; index < MaxPatrolSampleCount; index++)
        {
            bool isSuccess = Owner.MoveAbility.TryGetRandomReachablePoint(
                Owner.transform.position,
                Owner.Stat.PatrolRadius,
                out destination
            );

            if (isSuccess)
            {
                return true;
            }
        }

        return false;
    }
}
