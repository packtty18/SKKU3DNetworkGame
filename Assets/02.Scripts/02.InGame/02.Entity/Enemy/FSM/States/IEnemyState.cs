public interface IEnemyState
{
    EnemyStateId Id { get; }
    void Enter();
    void Tick(float dt);
    void Exit();
}
