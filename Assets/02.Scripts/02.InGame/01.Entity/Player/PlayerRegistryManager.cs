using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class PlayerRegistryManager : MonoBehaviour
{
    private static PlayerRegistryManager _instance;
    public static PlayerRegistryManager Instance => _instance;
    
    private readonly List<PlayerController> _players = new();
    public IReadOnlyList<PlayerController> Players => _players;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterPlayer(PlayerController player)
    {
        if (player == null || _players.Contains(player))
        {
            return;
        }

        _players.Add(player);
    }

    public void UnregisterPlayer(PlayerController player)
    {
        if (player == null)
        {
            return;
        }

        _players.Remove(player);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
