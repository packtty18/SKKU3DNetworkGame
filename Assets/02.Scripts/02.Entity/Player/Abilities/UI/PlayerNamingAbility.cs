using TMPro;
using UnityEngine;

public class PlayerNamingAbility : PlayerAbility
{
    [SerializeField] private TextMeshProUGUI _nameText;
    private void Start()
    {
        //내껀 안보임=> HUD로 보여줄것, 상대껏만 보임
        /*if (_owner.PhotonView.IsMine)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            _nameText.text = _owner.PhotonView.Owner.NickName;
            if (_owner.PhotonView.IsMine)
            {
                _nameText.color = Color.green;
            }
            else
            {
                _nameText.color = Color.red;
            }
        }*/
        
        _nameText.text = _owner.PhotonView.Owner.NickName;
        if (_owner.PhotonView.IsMine)
        {
            _nameText.color = Color.green;
        }
        else
        {
            _nameText.color = Color.red;
        }
    }
}