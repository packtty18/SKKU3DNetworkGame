using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : PunCallbackSingleton<GameFlowManager>
{
    private bool _spawned;

    protected override void OnInitialize()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnShutdown()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public override void OnJoinedRoom()
    {
        _spawned = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Game")
        {
            return;
        }

        if (!PhotonNetwork.InRoom)
        {
            return;
        }

        if (_spawned)
        {
            return;
        }

        _spawned = true;

        PlayerSpawnManager spawnManager = PlayerSpawnManager.Instance;
        if (spawnManager != null)
        {
            spawnManager.PlayerRandomPointSpawn();
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.Refresh();
        }
    }

    public void LeaveRoom()
    {
        SingletonRegistry.Instance.ShutdownByType(ESingletonType.Scene);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Lobby");
    }
}