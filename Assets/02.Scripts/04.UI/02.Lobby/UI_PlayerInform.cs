
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UI_PlayerInform : MonoBehaviour
{
    [Title("Reference In Hierachy")]
    [SerializeField,SceneObjectsOnly] private TMP_InputField _nicknameInput;
    [SerializeField,SceneObjectsOnly] private TMP_InputField _roomInput;
    [SerializeField,SceneObjectsOnly] private Button _roomButton;

    [SerializeField,SceneObjectsOnly] private Button _maleButton;
    [SerializeField,SceneObjectsOnly] private Button _femaleButton;
    
    [SerializeField,SceneObjectsOnly] private GameObject _maleModel;
    [SerializeField,SceneObjectsOnly] private GameObject _femaleModel;
    
    
    private ECharacterType _characterType;

    private void Start()
    {
        OnMaleButtonClick();
        
        //버튼 이벤트 등록
        _roomButton.onClick.RemoveAllListeners();
        _roomButton.onClick.AddListener(OnRoomButtonClick);
        _maleButton.onClick.RemoveAllListeners();
        _maleButton.onClick.AddListener(OnMaleButtonClick);
        _femaleButton.onClick.RemoveAllListeners();
        _femaleButton.onClick.AddListener(OnFemaleButtonClick);
    }

    private void OnRoomButtonClick()
    {
        //매니저의 함수 가져오기(이곳의 책임이 아님)
        MakeRoom();
    }

    private void MakeRoom()
    {
        string nickname = _nicknameInput.text;
        string roomName = _roomInput.text;

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(roomName))
        {
            return;
        }
        
        PhotonNetwork.NickName = nickname;

        // 룸 옵션
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;  // 룸 최대 접속자 수
        roomOptions.IsVisible = true; // 로비에서 룸을 보여줄 것인지
        roomOptions.IsOpen = true;    // 룸의 오픈 여부
        Hashtable roomProperties = new Hashtable();
        roomProperties["MasterName"] = nickname;
        roomOptions.CustomRoomProperties = roomProperties;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "MasterName" };
        // 룸 만들기 
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    private void OnMaleButtonClick() => ModelActivate(ECharacterType.Male);
    private void OnFemaleButtonClick() =>   ModelActivate(ECharacterType.Female);
    

    private void ModelActivate(ECharacterType type)
    {
        _characterType = type;
        _maleModel.SetActive(_characterType == ECharacterType.Male);
        _femaleModel.SetActive(_characterType == ECharacterType.Female);
    }

}
