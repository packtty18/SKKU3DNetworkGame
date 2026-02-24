using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class EntityStat
{
    //체력
    public float MaxHealth = 10;
    public float RegenerateHealth = 0;
    
    //스테미나
    public float MaxStamina = 10;
    public float RegenerateStamina = 1;
    public float ExhaustRecoveryRatio = 1f / 3f;
    public float MoveSpeed = 5;
    public float ExhaustSpeed = 2f;
    public float DashSpeed = 10;
    public float DashCost = 2f;
    
    public float JumpPower = 2.5f;
    public float JumpCost = 2f;
    
    public float AttackSpeed = 0.6f;
    public float AttackCost = 2.5f;
    
    public float RotationSpeed =100;
}    