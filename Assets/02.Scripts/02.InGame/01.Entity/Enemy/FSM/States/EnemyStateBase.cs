public abstract class EnemyStateBase : IEnemyState
{
    protected EnemyStateBase(EnemyController owner)
    {
        Owner = owner;
    }

    protected EnemyController Owner { get; }

    public abstract EnemyStateId Id { get; }

    public virtual void Enter()
    {
    }

    public virtual void Tick(float deltaTime)
    {
    }

    public virtual void Exit()
    {
    }
}
