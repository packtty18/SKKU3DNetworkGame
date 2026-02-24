using UnityEngine;

public class PlayerMoveAbility : PlayerAbility
{
    private const float GRAVITY = -9f;
    
    private CharacterController _characterController;
    private Animator _animator;
    private float _yVelocity = 0f;
    

    private ConsumableStat _stamina => _owner?.Stamina;

    private void Start()
    {
        if (!_owner.PhotonView.IsMine) return;
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!_owner.PhotonView.IsMine) return;
        
        float deltaTime = Time.deltaTime;   
        
        //이동 방향 정의
        Vector3 dir = new Vector3(_owner.Inputs.MoveHorizontalInput, 0f, _owner.Inputs.MoveVerticalInput).normalized;
        _animator.SetFloat("Move", dir.magnitude);

        dir = Camera.main.transform.TransformDirection(dir);
        
        //점프
        if (_owner.Inputs.JumpPressed && 
            _characterController.isGrounded && 
            _owner.TryUseStamina(_owner.Stat.JumpCost))
        {
            _yVelocity = _owner.Stat.JumpPower;
        }

        _yVelocity += GRAVITY * deltaTime;
        dir.y = _yVelocity;
        
        //대쉬
        float resultSpeed = _owner.Stat.MoveSpeed;
        if (_owner.Inputs.DashPressed && _owner.TryUseStamina(_owner.Stat.DashCost * deltaTime))
        {
            resultSpeed =  _owner.Stat.DashSpeed;
        }

        if (_owner.Exhausted)
        {
            resultSpeed = _owner.Stat.ExhaustSpeed;
        }

        _characterController.Move(dir * resultSpeed * Time.deltaTime);
    }
}
