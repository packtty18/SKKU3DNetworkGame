using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RoomItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _roomNameText;
    [SerializeField] private TextMeshProUGUI _roomMasterNameText;
    [SerializeField] private TextMeshProUGUI _roomPlayerCountText;
    [SerializeField] private Button _enterRoomButton;
    
    private RoomInfo _roomInfo;

    private void Start()
    {
        _enterRoomButton.onClick.RemoveAllListeners();
        _enterRoomButton.onClick.AddListener(EnterRoom);
    }

    public void SetItem(RoomInfo info)
    {
        _roomInfo = info;

        _roomNameText.text = _roomInfo.Name;
        _roomMasterNameText.text = _roomInfo.CustomProperties["MasterName"] as string;
        _roomPlayerCountText.text = $"{_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers}";
        
        gameObject.SetActive(true);
    }

    public void Reset()
    {
        _roomInfo = null;
        _roomNameText.text = "룸 이름";
        _roomMasterNameText.text = "반장 이름";
        _roomPlayerCountText.text = "0/0";
        
        gameObject.SetActive(false);
    }

    private void EnterRoom()
    {
        if (_roomInfo == null)
        {
            return;
        }

        PhotonNetwork.JoinRoom(_roomInfo.Name);
    }
}
