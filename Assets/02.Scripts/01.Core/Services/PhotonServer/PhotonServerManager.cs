using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonServerManager : MonoBehaviourPunCallbacks
{
    private string _version = "0.0.1";
    private string _nickname = "Player";
    [SerializeField] private PhotonPrefabPool _prefabPool;
    
    private void Start()
    {
        if (_prefabPool == null)
        {
            _prefabPool = GetComponent<PhotonPrefabPool>();
        }

        if (_prefabPool != null)
        {
            PhotonNetwork.PrefabPool = _prefabPool;
        }
        else
        {
            Debug.LogWarning("PhotonPrefabPool is not assigned. Falling back to default Resources-based pool.");
        }

        _nickname += $"_{UnityEngine.Random.Range(100, 999)}";
        
        //설정 파트
        PhotonNetwork.GameVersion = _version;
        PhotonNetwork.NickName = _nickname;
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
        Debug.Log(PhotonNetwork.InLobby);

        // 랜덤 방 입장 시도
        PhotonNetwork.JoinRandomRoom();
    }
    
    // 방 입장에 성공하면 자동으로 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log("룸 입장 완료!");
        
        Debug.Log($"룸: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"플레이어 인원: {PhotonNetwork.CurrentRoom.PlayerCount}");
        
        // 룸에 입장한 플레이어 정보
        Dictionary<int, Player> roomPlayers = PhotonNetwork.CurrentRoom.Players;
        foreach (KeyValuePair<int, Player> player in roomPlayers)
        {
            Debug.Log($"{player.Value.NickName} : {player.Value.ActorNumber}");
        }
        
        // 리소스 폴더 대신 IPunPrefabPool 커스텀 풀로 등록된 프리팹을 대상으로 생성
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

    // 랜덤방 입장에 실패하면 자동으로 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"랜덤 방 입장에 실패했습니다: {returnCode} - {message}");

        // 랜덤 룸 입장에 실패 => 룸없음 => 룸 생성
        
        // 룸 옵션
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;  // 룸 최대 접속자 수
        roomOptions.IsVisible = true; // 로비에서 룸을 보여줄 것인지
        roomOptions.IsOpen = true;    // 룸의 오픈 여부
        
        // 룸 만들기 
        PhotonNetwork.CreateRoom("test", roomOptions);
    }
    
    // 방 입장에 실패하면 자동으로 호출되는 콜백 함수
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"방 입장에 실패했습니다: {returnCode} - {message}");
    }
}
