using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    private readonly EnemyController _controller;
    private readonly Dictionary<EnemyStateId, IEnemyState> states;
    private IEnemyState current;

    public EnemyStateId CurrentId => current.Id;

    public EnemyStateMachine(EnemyController controller)
    {
        _controller = controller;
    }

    public void Change(EnemyStateId next)
    {
        //current를 재호출 한것이라면 중복시행이니 안함
        if (current != null && current.Id == next)
        {
            return;
        }
        
        //State 캐싱
        IEnemyState nextState = null;
        if (states.ContainsKey(next))
        {
            nextState = states[next];
        }
        else
        {
            switch (next)
            {
                //case EnemyStateId.Idle : nextState = new EnemyIdleState(_controller); break;
                
            }
        }
        
        current?.Exit();
        current = nextState;
        current.Enter();
    }

    public void Tick(float dt)
    {
        current?.Tick(dt);
    }
}
