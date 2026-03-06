using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerSpawnManager : MonoSingleton<PlayerSpawnManager>
{
    [SerializeField] private List<Transform> _spawnPoints = new();
    
    protected override void OnInitialize()
    {
        
    }

    protected override void OnShutdown()
    {
        
    }

    public void PlayerRandomPointSpawn()
    {
        int spawnId = Random.Range(0, _spawnPoints.Count);
        Transform spawnPoint = _spawnPoints[spawnId];

        PhotonNetwork.Instantiate("Player",spawnPoint.position, spawnPoint.rotation);
    }

    public bool TryGetRandomSpawnPoint(out Vector3 position, out Quaternion rotation)
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;

        if (_spawnPoints == null || _spawnPoints.Count == 0)
        {
            return false;
        }

        int spawnId = Random.Range(0, _spawnPoints.Count);
        Transform spawnPoint = _spawnPoints[spawnId];
        if (spawnPoint == null)
        {
            return false;
        }

        position = spawnPoint.position;
        rotation = spawnPoint.rotation;
        return true;
    }
}
