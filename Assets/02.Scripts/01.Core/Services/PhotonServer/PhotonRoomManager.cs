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
    
    
    //이건 오버라이드라 다른 처리를 하지 않아도 알아서 서버에 반영됨
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

    public void TryOnPlayerDeath(int attackerActorNumber, int victimActorNumber)
    {
        OnPlayerDeath(attackerActorNumber,victimActorNumber);
        photonView.RPC(nameof(OnPlayerDeath), RpcTarget.Others, attackerActorNumber,victimActorNumber);
    }
    
    //이건 직접 만든거라 네트워크에 반영 안됨
    [PunRPC]
    private void OnPlayerDeath(int attackerActorNumber , int victimActorNumber)
    {
        string attackerNickName = _room.Players[attackerActorNumber].NickName;
        string victimNickName = _room.Players[victimActorNumber].NickName;
        
        OnPlayerDeathed?.Invoke(attackerNickName, victimNickName);
    }
}
