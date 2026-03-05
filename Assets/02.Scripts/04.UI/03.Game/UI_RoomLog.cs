using System;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class UI_RoomLog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _logText;

    private void Start()
    {
        _logText.text = "방에 입장했습니다";
        
        PhotonRoomManager.Instance.OnPlayerEnter += OnPlayerEnterLog;
        PhotonRoomManager.Instance.OnPlayerLeft += OnPlayerLeftLog;
        PhotonRoomManager.Instance.OnPlayerDeathed += OnPlayerDeathLog ;
    }

    
    private void OnPlayerEnterLog(Player enteredPlayer)
    {
        _logText.text += "\n" + $"{enteredPlayer.NickName}님이 입장했습니다";
    }
    private void OnPlayerLeftLog(Player leftedPlayer)
    {
        _logText.text += "\n" + $"{leftedPlayer.NickName}님이 퇴장했습니다";
    }
    private void OnPlayerDeathLog(string killer, string killed)
    {
        _logText.text += "\n" + $"{killer}님이 {killed}님을 처치했습니다";
    }
}
