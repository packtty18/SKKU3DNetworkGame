using TMPro;
using UnityEngine;

public class PlayerNamingAbility : PlayerAbility
{
    [SerializeField] private TextMeshProUGUI _nameText;
    private void Start()
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

    private void FixedUpdate()
    {
        Transform _billboardTarget = Camera.main.transform;
        Vector3 targetPosition = new Vector3(_billboardTarget.position.x, 
            transform.position.y, 
            _billboardTarget.position.z);
        transform.LookAt(targetPosition);
    }
}