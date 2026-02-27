using System;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemObjectFactory : MonoBehaviourPun
{
    public static ItemObjectFactory Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RequestSpawnCoins(Vector3 position, int count = 1)
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnCoins(position, count);   
        }
        else
        {
            photonView.RPC(nameof(SpawnCoins), RpcTarget.MasterClient,position, count);
        }
    }
    
    [PunRPC]
    private void SpawnCoins(Vector3 position, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            //플레이어 생명주기가 아닌 룸 생명 주기로 만든다. 하지만 이것을 사용할 권한은 마스터만이 가능하다
            //그렇기에 일반 유저는 RPC를 통해 방장에게 만들어달라고 요청해야한다.
            PhotonNetwork.InstantiateRoomObject("Coin", position, Quaternion.identity);
        }
    }
    
    
    public void RequestDelete(int targetId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Delete(targetId);
        }
        else
        {
            photonView.RPC(nameof(Delete), RpcTarget.MasterClient,targetId);
        }
    }

    [PunRPC]
    private void Delete(int targetId)
    {
        PhotonView targetView = PhotonView.Find(targetId);
        if(targetView == null) return;
        PhotonNetwork.Destroy(targetView);
    }
}
