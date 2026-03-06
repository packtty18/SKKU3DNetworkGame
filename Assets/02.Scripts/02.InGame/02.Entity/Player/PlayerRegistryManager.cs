using System.Collections.Generic;
using UnityEngine;

public class PlayerRegistryManager : MonoSingleton<PlayerRegistryManager>
{
    private readonly List<PlayerController> _players = new();
    public IReadOnlyList<PlayerController> Players => _players;
    
    protected override void OnInitialize()
    {
        
    }

    protected override void OnShutdown()
    {
        
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
}
