using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonServerManager : PunCallbackSingleton<PhotonServerManager>
{
    private string _version = "0.0.1";
    private string _defaultNickname = "Player";

    protected override void OnInitialize()
    {
        //서버접속
        _defaultNickname += $"_{UnityEngine.Random.Range(100, 999)}";
        
        //설정 파트
        PhotonNetwork.GameVersion = _version;
        PhotonNetwork.NickName = _defaultNickname;
        // TCP/UDP : 빈신뢰성
        PhotonNetwork.SendRate          = 30; // 얼마나 자주 데이터를 송수신할 것인가..  (실제 송수신)
        PhotonNetwork.SerializationRate = 30; // 얼마나 자주 데이터를 직렬화 할 것인지.  (송수신 준비)
        // 방장이 로드한 씬 게임에 다른 유저들도 똑같이 그 씬을 로드하도록 동기화해준다.
        PhotonNetwork.AutomaticallySyncScene = true;
        // 위에 설정한 값들을 이용해서 서버로 접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 포톤 서버에 접속이 성공
    public override void OnConnected()
    {
        Debug.Log("네임서버 접속 완료!");
        // 네임 서버(AppId, GameVersion 등으로 구분되는 서버)
        Debug.Log(PhotonNetwork.CloudRegion);
        // ping 테스트를 통해서 가장 빠른 리전으로 자동 연결된다. (kr: korea)
    }

    public override void OnConnectedToMaster()
    {
        //TypedLobby lobby = new TypedLobby("3channel", LobbyType.Default);
        PhotonNetwork.JoinLobby(); // Default 로비 입장 시도
    }

    // 로비 입장에 성공하면 자동으로 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속 완료!");
    }
    
    // 랜덤방 입장에 실패하면 자동으로 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"랜덤 방 입장에 실패했습니다: {returnCode} - {message}");
    }
    
    // 방 입장에 실패하면 자동으로 호출되는 콜백 함수
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"방 입장에 실패했습니다: {returnCode} - {message}");
    }

    protected override void OnShutdown()
    {
        PhotonNetwork.Disconnect();
    }
}
