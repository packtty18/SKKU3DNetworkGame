using System;
using UnityEngine;

//소모가능한 자원에 대한 도메인
//체력, 스테미너
[Serializable]
public sealed class ConsumableStat
{
    public float Max { get; private set; }          //최대 보유량
    public float Current { get; private set; }      //현재 보유량    
    public float Regenerate { get; private set; }   //초당 회복량

    public bool IsFull => Current >= Max;   
    public bool IsEmpty => Current <= 0f;
    public float Ratio => Current / Max;

    public ConsumableStat(float max, float current = 0, float regenerate = 0)
    {
        Initialize(max, current, regenerate);
    }
    
    //Max는 0보다 커야하며, current와 regenrate는 0이상이어야한다.
    public void Initialize(float max, float current, float regenerate)
    {
        if (max <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(max));
        }
        if(current < 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(current));
        }
        if(regenerate < 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(regenerate));
        }
        
        Max = max;
        Regenerate = regenerate;
        Current = Mathf.Clamp(current, 0f, Max);
    }
    
    //소모 요구
    public bool TryConsume(float amount)
    {
        if (amount <= 0f)
        {
            return true;
        }

        float next = Current - amount;

        if (next < 0f)
        {
            return false;
        }

        Current = next;
        return true;
    }

    // 회복 요구
    public bool TryRecover(float amount)
    {
        if (amount <= 0f)
        {
            return true;
        }

        float next = Current + amount;

        if (next > Max)
        {
            return false;
        }

        Current = next;
        return true;
    }

    public bool TryRegenerateOnce()
    {
        return TryRecover(Regenerate);
    }

    // 초당 회복 지원용 (deltaTime 기반)
    public bool TryRegenerate(float deltaTime)
    {
        if (deltaTime <= 0f)
        {
            return false;
        }

        float amount = Regenerate * deltaTime;
        return TryRecover(amount);
    }

    public void SetMax(float max)
    {
        if (max <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(max));
        }

        Max = max;
        Current = Mathf.Clamp(Current, 0f, Max);
    }

    public void SetCurrent(float value)
    {
        Current = Mathf.Clamp(value, 0f, Max);
    }
    
}