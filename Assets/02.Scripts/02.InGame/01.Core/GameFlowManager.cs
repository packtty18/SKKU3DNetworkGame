using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : PunCallbackSingleton<GameFlowManager>
{
    private bool _spawned;

    protected override void OnInitialize()
    {
        
    }

    protected override void OnShutdown()
    {
        
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        PlayerSpawnManager.Instance.PlayerRandomPointSpawn();
        ScoreManager.Instance.InitScore();
        
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return;
        }
        PlayerSpawnManager.Instance.PlayerRandomPointSpawn();
        ScoreManager.Instance.InitScore();
    }

    public void LeaveRoom()
    {
        SingletonRegistry.Instance.ShutdownByType(ESingletonType.Scene);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Lobby");
    }
}