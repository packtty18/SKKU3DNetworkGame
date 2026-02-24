using UnityEngine;

public class PlayerTauntAbility : PlayerAbility
{
    private Animator _animator;
    private bool _isTaunting;

    private void Start()
    {
        if (!_owner.PhotonView.IsMine) return;
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!_owner.PhotonView.IsMine || _animator == null) return;

        int tauntNumber = GetRequestedTauntNumber();
        if (tauntNumber > 0)
        {
            if (_isTaunting)
            {
                CancelTaunt();
            }

            PlayTaunt(tauntNumber);
        }

        if (_isTaunting && HasInterruptInput())
        {
            CancelTaunt();
        }
    }

    private void PlayTaunt(int tauntNum)
    {
        _animator.SetInteger("TauntCount", tauntNum);
        _animator.SetTrigger("OnTaunt");
        _isTaunting = true;
    }

    private void CancelTaunt()
    {
        _animator.ResetTrigger("OnTaunt");
        _animator.SetTrigger("TauntCancel");

        _isTaunting = false;
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
}
