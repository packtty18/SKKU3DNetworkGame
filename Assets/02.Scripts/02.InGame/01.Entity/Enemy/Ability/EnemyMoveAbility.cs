using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMoveAbility : EnemyAbility
{
    private const float MinDirectionSqrMagnitude = 0.0001f;
    private const float PathReachEpsilon = 0.05f;
    private const float RandomPositionSampleRadius = 3f;

    private NavMeshAgent _agent;

    protected override void Awake()
    {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (_agent == null || _owner == null || _owner.Stat == null)
        {
            return;
        }

        _agent.speed = _owner.Stat.MoveSpeed;
        _agent.updateRotation = false;
    }

    private void Update()
    {
        if (_agent == null || _owner == null || _owner.Stat == null)
        {
            return;
        }

        if (_agent.isStopped)
        {
            return;
        }

        UpdateRotation(_agent.destination, Time.deltaTime);
    }

    public bool SetDestination(Vector3 worldPosition)
    {
        if (_agent == null || !_agent.enabled)
        {
            return false;
        }

        _agent.isStopped = false;
        return _agent.SetDestination(worldPosition);
    }

    public bool Warp(Vector3 worldPosition)
    {
        if (_agent == null || !_agent.enabled)
        {
            return false;
        }

        return _agent.Warp(worldPosition);
    }

    public void StopMove()
    {
        if (_agent == null || !_agent.enabled)
        {
            return;
        }

        _agent.isStopped = true;
        _agent.ResetPath();
    }

    public bool HasReachedDestination(float extraDistance = 0f)
    {
        if (_agent == null || !_agent.enabled)
        {
            return true;
        }

        if (_agent.pathPending)
        {
            return false;
        }

        float requiredDistance = Mathf.Max(_agent.stoppingDistance + extraDistance, PathReachEpsilon);
        if (_agent.remainingDistance > requiredDistance)
        {
            return false;
        }

        if (_agent.hasPath && _agent.velocity.sqrMagnitude > MinDirectionSqrMagnitude)
        {
            return false;
        }

        return true;
    }

    public void RotateToward(Vector3 worldPosition, float deltaTime)
    {
        if (_owner == null || _owner.Stat == null)
        {
            return;
        }

        UpdateRotation(worldPosition, deltaTime);
    }

    public bool TryGetRandomReachablePoint(Vector3 center, float radius, out Vector3 point)
    {
        point = center;

        if (!TrySampleNavMeshPosition(center, radius, out Vector3 sampledPoint))
        {
            return false;
        }

        if (!HasPath(sampledPoint))
        {
            return false;
        }

        point = sampledPoint;
        return true;
    }

    private void UpdateRotation(Vector3 worldPosition, float deltaTime)
    {
        Vector3 direction = worldPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= MinDirectionSqrMagnitude)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            _owner.Stat.RotateSpeed * deltaTime
        );
    }

    private static bool TrySampleNavMeshPosition(Vector3 center, float radius, out Vector3 sampledPoint)
    {
        Vector3 randomOffset = Random.insideUnitSphere * Mathf.Max(radius, 0.1f);
        randomOffset.y = 0f;
        Vector3 candidate = center + randomOffset;

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, RandomPositionSampleRadius, NavMesh.AllAreas))
        {
            sampledPoint = hit.position;
            return true;
        }

        sampledPoint = center;
        return false;
    }

    private bool HasPath(Vector3 destination)
    {
        if (_agent == null || !_agent.enabled)
        {
            return false;
        }

        NavMeshPath path = new NavMeshPath();
        bool isPathCalculated = _agent.CalculatePath(destination, path);
        return isPathCalculated && path.status == NavMeshPathStatus.PathComplete;
    }
}
