using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class UI_RoomList : MonoBehaviourPunCallbacks
{
    [Title("Reference")]
    [SerializeField, SceneObjectsOnly] private Transform _itemRoot;
    [SerializeField, RequiredIn(PrefabKind.PrefabAsset)] private GameObject _roomItemPrefab;
    
    [Title("Cache")]
    private List<UI_RoomItem> _roomItems = new();
    [ShowInInspector,ReadOnly] private Dictionary<string, RoomInfo> _rooms = new();

    private void Awake()
    {
        _roomItems = _itemRoot.GetComponentsInChildren<UI_RoomItem>().ToList();
        
        HideAllRoomUI();
    }
    
    //로비에 입장후 방 내용이 바뀌면 자동으로 호출되는 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 모든 UI를 비활성화 하고,
        HideAllRoomUI();

        foreach (var room in roomList)
        {
            if (room.RemovedFromList)
            {
                _rooms.Remove(room.Name); // Delete
            }
            else
            {
                _rooms[room.Name] = room; // Add or Update
            }
        }
        
        int roomCount = _rooms.Count;
        List<RoomInfo> rooms = _rooms.Values.ToList();
        for (int i = 0; i < roomCount; i++)
        {
            _roomItems[i].SetItem(rooms[i]);
        }
    }

    private void HideAllRoomUI()
    {
        foreach (UI_RoomItem item in _roomItems)
        {
            item.Reset();
        }
    }
}
