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

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0f, v).normalized;
        _animator.SetFloat("Move", dir.magnitude);

        dir = Camera.main.transform.TransformDirection(dir);

        if (Input.GetKeyDown(KeyCode.Space) && _characterController.isGrounded)
        {
            _yVelocity = _owner.Stat.JumpPower;
        }

        _yVelocity += GRAVITY * Time.deltaTime;
        dir.y = _yVelocity;

        float speed = _owner.Stat.MoveSpeed;
        float deltaTime = Time.deltaTime;

        bool wantsDash = Input.GetKey(KeyCode.LeftShift);
        bool dashBlockedByExhaust = _owner.Exhausted;

        if (wantsDash && !dashBlockedByExhaust && _stamina != null)
        {
            if (_stamina.TryConsume(_owner.Stat.DashCost * deltaTime))
            {
                speed = _owner.Stat.DashSpeed;

                if (_stamina.IsEmpty)
                {
                    _owner.Exhausted = true;
                }
            }
            else
            {
                _stamina.SetCurrent(0f);
                _owner.Exhausted = true;
            }
        }
        else
        {
            _stamina?.TryRegenerate(deltaTime);
        }

        if ( _owner.Exhausted && _stamina != null && _stamina.Ratio >= _owner.Stat.ExhaustRecoveryRatio)
        {
            _owner.Exhausted = false;
        }

        _characterController.Move(dir * speed * Time.deltaTime);
    }
}
