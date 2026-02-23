using UnityEngine;

public class PlayerMoveAbility : PlayerAbility
{
    private float _gravity = -9f;
    
    private CharacterController _characterController;
    private Animator _animator;
    
    //누적 y
    private float _yVelocity = 0f;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        if (!_owner.PhotonView.IsMine) return;
        
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        //인풋에 따른 방향 정의
        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized;
        
        //애니메이션 적용
        _animator.SetFloat("Move", dir.magnitude);
        
        dir = Camera.main.transform.TransformDirection(dir); //카메라가 보는 방향 기준
        
        //점프 기능
        if (Input.GetKeyDown(KeyCode.Space) && _characterController.isGrounded)
        {
            Debug.Log("점프");
            _yVelocity = _owner.Stat.JumpPower;
        }
        
        //중력 적용(프레임마다 중력값만큼 누적)
        _yVelocity += _gravity * Time.deltaTime;
        dir.y = _yVelocity;
        
        
        //최종 적용
        
        _characterController.Move(dir * _owner.Stat.MoveSpeed * Time.deltaTime);
    }
}
