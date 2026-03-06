using UnityEngine;

public class EnemyHitState : EnemyStateBase
{
    private float _recoverTimer;

    public EnemyHitState(EnemyController owner) : base(owner)
    {
    }

    public override EnemyStateId Id => EnemyStateId.Hit;

    public override void Enter()
    {
        float hitDuration = Owner.Stat != null ? Owner.Stat.HitDuration : 0f;
        _recoverTimer = Mathf.Max(0f, hitDuration);
        Owner.MoveAbility.StopMove();
    }

    public override void Tick(float deltaTime)
    {
        _recoverTimer -= deltaTime;
        if (_recoverTimer > 0f)
        {
            return;
        }

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

        Owner.ClearCurrentTarget();
        Owner.FSM.Change(EnemyStateId.Idle);
    }
}
