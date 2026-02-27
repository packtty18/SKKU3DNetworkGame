
using System;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMoveAbility : EnemyAbility
{
    private NavMeshAgent _agent;
    
    protected override void Awake()
    {
        base.Awake();   
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        SetAgentStat();
        _agent.updateRotation = false; // Agent 자동 회전 금지, 우리가 직접 회전
    }

    private void Update()
    {
        UpdateRotation();
    }

    private void SetAgentStat()
    {
        _agent.speed = _owner.Stat.MoveSpeed;
    }
    
    
    private void UpdateRotation()
    {
        if (_agent == null)
        {
            return;
        }
        Vector3 dir = _agent.destination  - transform.position;
        dir.y = 0.0f; //y는 신경쓰지 않음
        
        //이미 해당 방향을 보고 있는 경우
        if (dir.sqrMagnitude <= 0.0001f)
        {
            return;
        }
        
        //회전  수행
        Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            _owner.Stat.RotateSpeed * Time.deltaTime
        );
    }
    
    [Button]
    public bool SetDestination(Vector3 worldPos)
    {
        if (_agent == null || _agent.enabled == false)
        {
            return false;
        }

        _agent.isStopped = false;
        return _agent.SetDestination(worldPos);
    }
    
    [Button]
    public bool Warp(Vector3 worldPos)
    {
        if (_agent == null || _agent.enabled == false)
        {
            return false;
        }

        return _agent.Warp(worldPos);
    }
    
    [Button]
    public void StopAgentMove()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
    }
}