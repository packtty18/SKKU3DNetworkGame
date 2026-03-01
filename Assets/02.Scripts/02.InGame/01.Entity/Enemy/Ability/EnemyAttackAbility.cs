using UnityEngine;

public class EnemyAttackAbility : EnemyAbility
{
    private static readonly int AttackCountHash = Animator.StringToHash("AttackCount");
    private static readonly int OnAttackHash = Animator.StringToHash("OnAttack");

    [SerializeField] private EnemyAttackAnimationSequenceType _animationSequenceType = EnemyAttackAnimationSequenceType.Sequence;

    private EnemyAttackColliderAbility _attackColliderAbility;
    private int _previousAnimationNumber;
    private float _attackCooldown;

    protected override void Awake()
    {
        base.Awake();
        _attackColliderAbility = GetComponent<EnemyAttackColliderAbility>();
    }

    private void Update()
    {
        if (_attackCooldown <= 0f)
        {
            return;
        }

        _attackCooldown -= Time.deltaTime;
    }

    public bool TryAttack()
    {
        if (_owner == null || _owner.Stat == null || _owner.Animator == null || _attackColliderAbility == null)
        {
            return false;
        }

        if (_attackCooldown > 0f)
        {
            return false;
        }

        _attackCooldown = Mathf.Max(0.01f, _owner.Stat.AttackDelay);
        int attackNumber = SelectAttackNumber();
        PlayAttack(attackNumber);
        return true;
    }

    public void CancelAttack()
    {
        if (_attackColliderAbility == null)
        {
            return;
        }

        _attackColliderAbility.DeactivateAllColliders();
    }

    // Animation event.
    public void OnAttackAnimationStart(int attackNumber)
    {
        if (_attackColliderAbility == null)
        {
            return;
        }

        _attackColliderAbility.BeginAttack(attackNumber);
    }

    // Animation event.
    public void OnAttackAnimationEnd()
    {
        if (_attackColliderAbility == null)
        {
            return;
        }

        _attackColliderAbility.EndAttack();
    }

    private int SelectAttackNumber()
    {
        switch (_animationSequenceType)
        {
            case EnemyAttackAnimationSequenceType.Sequence:
            {
                int sequenceAttackNumber = 1 + (_previousAnimationNumber++ % 4);
                return sequenceAttackNumber;
            }
            case EnemyAttackAnimationSequenceType.Random:
            {
                return Random.Range(1, 5);
            }
            default:
            {
                return 1;
            }
        }
    }

    private void PlayAttack(int attackCount)
    {
        _owner.Animator.SetInteger(AttackCountHash, attackCount);
        _owner.Animator.SetTrigger(OnAttackHash);
    }
}

public enum EnemyAttackAnimationSequenceType
{
    Sequence,
    Random,
}
