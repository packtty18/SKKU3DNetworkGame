using System;
using TMPro;
using UnityEngine;

public class PlayerNamingAbility : PlayerAbility
{
    [SerializeField] private TextMeshProUGUI _nameText;

    private void Start()
    {
        SetNickName();
    }

    public void SetNickName()
    {
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