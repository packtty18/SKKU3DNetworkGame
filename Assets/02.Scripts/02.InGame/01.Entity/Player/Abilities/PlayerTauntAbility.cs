using Photon.Pun;
using UnityEngine;

public class PlayerTauntAbility : PlayerAbility
{
    private static readonly int TauntCountHash = Animator.StringToHash("TauntCount");
    private static readonly int OnTauntHash = Animator.StringToHash("OnTaunt");
    private static readonly int OnTauntCancelHash = Animator.StringToHash("OnTauntCancel");
    
    private bool _isTaunting;
    
    private void Update()
    {
        if (_owner.IsDead || !_owner.PhotonView.IsMine) return;

        int tauntNumber = GetRequestedTauntNumber();
        if (tauntNumber > 0)
        {
            if (_isTaunting)
            {
                CancelTauntNetworked();
            } 
            _owner.Animator.SetInteger(TauntCountHash, tauntNumber);
            PlayTauntNetworked();
        }

        if (_isTaunting && HasInterruptInput())
        {
            CancelTauntNetworked();
        }
    }
    
    private int GetRequestedTauntNumber()
    {
        PlayerInputs inputs = _owner.Inputs;

        if (inputs.Skill1Pressed) return 1;
        if (inputs.Skill2Pressed) return 2;
        if (inputs.Skill3Pressed) return 3;

        return 0;
    }

    private bool HasInterruptInput()
    {
        PlayerInputs inputs = _owner.Inputs;

        bool hasMoveInput = Mathf.Abs(inputs.MoveHorizontalInput) > 0.01f ||
                            Mathf.Abs(inputs.MoveVerticalInput) > 0.01f;

        return hasMoveInput ||
               inputs.JumpPressed ||
               inputs.DashPressed ||
               inputs.AttackPressed;
    }
    
    private void PlayTaunt()
    {
        if (_owner.Animator == null)
        {
            return;
        }
        
        _isTaunting = true;
        
        _owner.Animator.SetTrigger(OnTauntHash);
    }

    private void PlayTauntNetworked()
    {
        PlayTaunt();

        if (_owner == null || _owner.PhotonView == null || !_owner.PhotonView.IsMine)
        {
            return;
        }

        _owner.PhotonView.RPC(nameof(RpcPlayTaunt), RpcTarget.Others);
    }

    private void CancelTaunt()
    {
        if (_owner.Animator == null)
        {
            return;
        }
        _isTaunting = false;
        _owner.Animator.ResetTrigger(OnTauntHash);
        _owner.Animator.SetTrigger(OnTauntCancelHash);
    }

    private void CancelTauntNetworked()
    {
        CancelTaunt();

        if (_owner == null || _owner.PhotonView == null || !_owner.PhotonView.IsMine)
        {
            return;
        }

        _owner.PhotonView.RPC(nameof(RpcCancelTaunt), RpcTarget.Others);
    }
    
    [PunRPC]
    private void RpcPlayTaunt()
    {
        PlayTaunt();
    }

    [PunRPC]
    private void RpcCancelTaunt()
    {
        CancelTaunt();
    }
}
