using Photon.Pun;
using UnityEngine;

public class PlayerAttackAbility : PlayerAbility
{
    private static readonly int AttackCountHash = Animator.StringToHash("AttackCount");
    private static readonly int OnAttackHash = Animator.StringToHash("OnAttack");

    [SerializeField] private EAnimationSequenceType _animationSequenceType;

    private int _prevAnimationNumber = 0;
    private float _attackTimer = 0f;

    private void Update()
    {
        if (_owner.IsDead || !_owner.PhotonView.IsMine) return;

        _attackTimer += Time.deltaTime;

        if (_owner.Inputs.AttackPressed &&
            _attackTimer >= _owner.Stat.AttackSpeed &&
            _owner.TryUseStamina(_owner.Stat.AttackCost))
        {
            _attackTimer = 0f;

            int animationNumber = 0;
            switch (_animationSequenceType)
            {
                case EAnimationSequenceType.Sequence:
                {
                    animationNumber = 1 + (_prevAnimationNumber++) % 3;
                    break;
                }
                case EAnimationSequenceType.Random:
                {
                    animationNumber = Random.Range(1, 4);
                    break;
                }
            }

            PlayAttackNetworked(animationNumber);
        }
    }

    private void SetAttackCount(int attackCount)
    {
        _owner.Animator.SetInteger(AttackCountHash, attackCount);
    }

    private void TriggerAttack()
    {
        _owner.Animator.SetTrigger(OnAttackHash);
    }

    private void PlayAttackNetworked(int attackCount)
    {
        SetAttackCount(attackCount);
        TriggerAttack();

        if (_owner == null || _owner.PhotonView == null || !_owner.PhotonView.IsMine)
        {
            return;
        }

        _owner.PhotonView.RPC(nameof(RpcPlayAttack), RpcTarget.Others, attackCount);
    }
    
    [PunRPC]
    private void RpcPlayAttack(int attackCount)
    {
        SetAttackCount(attackCount);
        TriggerAttack();
    }

}

public enum EAnimationSequenceType
{
    Sequence,
    Random,
}
