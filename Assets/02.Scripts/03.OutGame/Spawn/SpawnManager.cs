using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void PlayerRandomPointSpawn()
    {
        int spawnId = Random.Range(0, _spawnPoints.Count);
        Transform spawnPoint = _spawnPoints[spawnId];

        PhotonNetwork.Instantiate("Player",spawnPoint.position, spawnPoint.rotation);
    }
}