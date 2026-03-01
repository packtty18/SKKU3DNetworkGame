using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance { get; private set; }

    [SerializeField] private List<Transform> _spawnPoints = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RequestRespawn(EnemyController enemy, float delay)
    {
        if (enemy == null)
        {
            return;
        }

        StartCoroutine(RespawnAfterDelay(enemy, Mathf.Max(0f, delay)));
    }

    public bool TryGetRandomSpawnPoint(out Vector3 position, out Quaternion rotation)
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;

        if (_spawnPoints == null || _spawnPoints.Count == 0)
        {
            return false;
        }

        int spawnIndex = Random.Range(0, _spawnPoints.Count);
        Transform spawnPoint = _spawnPoints[spawnIndex];

        if (spawnPoint == null)
        {
            return false;
        }

        position = spawnPoint.position;
        rotation = spawnPoint.rotation;
        return true;
    }

    private IEnumerator RespawnAfterDelay(EnemyController enemy, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemy == null)
        {
            yield break;
        }

        Vector3 position = enemy.transform.position;
        Quaternion rotation = enemy.transform.rotation;

        if (TryGetRandomSpawnPoint(out Vector3 randomPosition, out Quaternion randomRotation))
        {
            position = randomPosition;
            rotation = randomRotation;
        }

        enemy.Respawn(position, rotation);
    }
}
