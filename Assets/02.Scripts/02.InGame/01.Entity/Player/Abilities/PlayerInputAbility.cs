using UnityEngine;

public class PlayerInputAbility : PlayerAbility
{
    public PlayerInputs Inputs { get; } = new PlayerInputs();

    private void Update()
    {
        if (_owner == null || _owner.PhotonView == null || !_owner.PhotonView.IsMine)
        {
            Inputs.ClearInputState();
            return;
        }

        Inputs.MoveHorizontalInput = Input.GetAxisRaw("Horizontal");
        Inputs.MoveVerticalInput = Input.GetAxisRaw("Vertical");
        Inputs.DashPressed = Input.GetKey(KeyCode.LeftShift);
        Inputs.JumpPressed = Input.GetKeyDown(KeyCode.Space);

        Inputs.LookInputX = Input.GetAxis("Mouse X");
        Inputs.LookInputY = Input.GetAxis("Mouse Y");

        Inputs.AttackPressed = Input.GetMouseButton(0);

        Inputs.Skill1Pressed = Input.GetKeyDown(KeyCode.Alpha1);
        Inputs.Skill2Pressed = Input.GetKeyDown(KeyCode.Alpha2);
        Inputs.Skill3Pressed = Input.GetKeyDown(KeyCode.Alpha3);
    }
}
