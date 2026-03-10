using UnityEngine;

public class EnemyIdleState : EnemyStateBase
{
    private float _waitTimer;

    public EnemyIdleState(EnemyController owner) : base(owner)
    {
    }

    public override EnemyStateId Id => EnemyStateId.Idle;

    public override void Enter()
    {
        _waitTimer = Owner.ConsumeIdleDelay();
        Owner.MoveAbility.StopMove();
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

        _waitTimer -= deltaTime;
        if (_waitTimer > 0f)
        {
            return;
        }

        Owner.FSM.Change(EnemyStateId.Patrol);
    }
}
