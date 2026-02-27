using UnityEngine;

public class EnemyStateBase : IEnemyState
{
    public EnemyStateId Id { get; }
    public void Enter()
    {
        Debug.Log($"{Id} state 실행");
    }

    public void Tick(float dt)
    {
        
    }

    public void Exit()
    {
        Debug.Log($"{Id} state 종료");
    }
}
