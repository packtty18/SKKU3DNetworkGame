using System.Collections.Generic;

public class EnemyStateMachine
{
    private readonly Dictionary<EnemyStateId, IEnemyState> _states;
    private IEnemyState _current;

    public EnemyStateId CurrentId => _current != null ? _current.Id : EnemyStateId.Idle;

    public EnemyStateMachine(EnemyController controller)
    {
        _states = new Dictionary<EnemyStateId, IEnemyState>
        {
            { EnemyStateId.Idle, new EnemyIdleState(controller) },
            { EnemyStateId.Patrol, new EnemyPatrolState(controller) },
            { EnemyStateId.Chase, new EnemyChaseState(controller) },
            { EnemyStateId.Attack, new EnemyAttackState(controller) },
            { EnemyStateId.Hit, new EnemyHitState(controller) },
            { EnemyStateId.Dead, new EnemyDeadState(controller) },
        };
    }

    public void Change(EnemyStateId next)
    {
        if (_current != null && _current.Id == next)
        {
            return;
        }

        if (!_states.TryGetValue(next, out IEnemyState nextState))
        {
            return;
        }

        _current?.Exit();
        _current = nextState;
        _current.Enter();
    }

    public void Tick(float deltaTime)
    {
        _current?.Tick(deltaTime);
    }
}
