using UnityEngine;

public class PlayerMoveAbility : PlayerAbility
{
    private static readonly int MoveHash = Animator.StringToHash("Move");
    private const float GRAVITY = -9f;

    private CharacterController _characterController;
    private float _yVelocity = 0f;
    
    
    protected override void Awake()
    {
        base.Awake();
        _characterController = GetComponent<CharacterController>();
    }
    
    private void Update()
    {
        if (_owner.IsDead || !_owner.PhotonView.IsMine) return;
        
        float deltaTime = Time.deltaTime;   
        
        //이동 방향 정의
        Vector3 dir = new Vector3(_owner.Inputs.MoveHorizontalInput, 0f, _owner.Inputs.MoveVerticalInput).normalized;
        SetMoveMagnitude(dir.magnitude);

        dir = Camera.main.transform.TransformDirection(dir);
        
        //점프
        if (_owner.Inputs.JumpPressed && 
            _characterController.isGrounded && 
            _owner.GetAbility<PlayerStaminaAbility>().TryUseStamina(_owner.Stat.AttackCost))
        {
            _yVelocity = _owner.Stat.JumpPower;
        }

        _yVelocity += GRAVITY * deltaTime;
        dir.y = _yVelocity;
        
        //대쉬
        float resultSpeed = _owner.Stat.MoveSpeed;
        if (_owner.Inputs.DashPressed && _owner.GetAbility<PlayerStaminaAbility>().TryUseStamina(_owner.Stat.DashCost * deltaTime))
        {
            resultSpeed =  _owner.Stat.DashSpeed;
        }

        if (_owner.Exhausted)
        {
            resultSpeed = _owner.Stat.ExhaustSpeed;
        }

        _characterController.Move(dir * resultSpeed * Time.deltaTime);
    }
    
    
    private void SetMoveMagnitude(float moveMagnitude)
    {
        if (_owner.Animator == null)
        {
            return;
        }

        _owner.Animator.SetFloat(MoveHash, moveMagnitude);
    }

}
