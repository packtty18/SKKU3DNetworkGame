using UnityEngine;

public class PlayerInputs
{
    public float MoveHorizontalInput { get; set; }
    public float MoveVerticalInput { get; set; }
    public float LookInputX { get; set; }
    public float LookInputY { get; set; }
    public bool DashPressed { get; set; }
    public bool JumpPressed { get; set; }
    public bool AttackPressed { get; set; }
    public bool Skill1Pressed { get; set; }
    public bool Skill2Pressed { get; set; }
    public bool Skill3Pressed { get; set; }

    public PlayerInputs()
    {
        ClearInputState();
    }
    public void ClearInputState()
    {
        MoveHorizontalInput = 0;
        MoveVerticalInput = 0;
        LookInputX = 0;
        LookInputY = 0;
        DashPressed = false;
        JumpPressed = false;
        
        AttackPressed = false;
        Skill1Pressed = false;
        Skill2Pressed = false;
        Skill3Pressed = false;
    }
}