using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EffectType
{
    IncreaseSpeed, // increase speed by 2
    ReduceSpeed, // decrease speed by 2
    Stun // immobile 
}

public class Effect : MonoBehaviour
{

    public EffectType type;
    public float duration;
    public float value;
    public DateTime effectStartingTime;

    public Effect(EffectType type, float duration, float value)
    {
        this.type = type;
        this.duration = duration;
        this.value = value;
    }

    public void StartDurationCountDown()
    {
        effectStartingTime = DateTime.Now;
        this.enabled = true;
    }

    public bool IsEffectEnded()
    {
        TimeSpan time = DateTime.Now - effectStartingTime;
        
        if (time.Seconds >= duration)
        {
            return true;
        }
        return false;
    }
}
