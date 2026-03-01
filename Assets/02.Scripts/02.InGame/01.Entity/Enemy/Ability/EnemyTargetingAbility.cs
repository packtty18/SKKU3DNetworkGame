using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetingAbility : EnemyAbility
{
    public bool TryFindClosestTarget(float searchRange, out PlayerController target)
    {
        target = null;

        if (searchRange <= 0f)
        {
            return false;
        }

        float bestDistanceSqr = searchRange * searchRange;
        IReadOnlyList<PlayerController> players = PlayerRegistryManager.Instance.Players;

        for (int index = 0; index < players.Count; index++)
        {
            PlayerController player = players[index];
            if (player == null || player.IsDead)
            {
                continue;
            }

            Vector3 offset = player.transform.position - transform.position;
            offset.y = 0f;

            float distanceSqr = offset.sqrMagnitude;
            if (distanceSqr > bestDistanceSqr)
            {
                continue;
            }

            bestDistanceSqr = distanceSqr;
            target = player;
        }

        return target != null;
    }

    public bool IsInRange(PlayerController target, float range)
    {
        if (target == null)
        {
            return false;
        }

        Vector3 offset = target.transform.position - transform.position;
        offset.y = 0f;
        return offset.sqrMagnitude <= range * range;
    }
}
