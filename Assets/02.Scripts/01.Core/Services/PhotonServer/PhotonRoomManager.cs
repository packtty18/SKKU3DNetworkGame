using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonRoomManager : MonoBehaviourPunCallbacks
{
    public static PhotonRoomManager Instance { get; private set; }

    private Room _room;
    public Room Room => _room;

    public event Action OnDataChanged;
    public event Action<Player> OnPlayerEnter;
    public event Action<Player> OnPlayerLeft;
    public event Action<string, string> OnPlayerDeathed;
    
    private void Awake()
    {
        Instance = this;
    }

    // 방 입장에 성공하면 자동으로 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        _room = PhotonNetwork.CurrentRoom;
        OnDataChanged?.Invoke();

        if (PlayerSpawnManager.Instance != null)
        {
            PlayerSpawnManager.Instance.PlayerRandomPointSpawn();
        }
        else
        {
            Debug.Log("스폰매니저가 존재하지 않음");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnDataChanged?.Invoke();
        OnPlayerEnter?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnDataChanged?.Invoke();
        OnPlayerLeft?.Invoke(otherPlayer);
    }

    public void OnPlayerDeath(int attackerActorNumber)
    {
        string attackerNickName = _room.Players[attackerActorNumber].NickName;
        string myNickName = PhotonNetwork.LocalPlayer.NickName;
        
        OnPlayerDeathed?.Invoke(attackerNickName, myNickName);
    }
}
