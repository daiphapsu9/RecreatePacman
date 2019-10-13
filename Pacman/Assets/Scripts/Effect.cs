using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EffectType
{
    IncreaseSpeed, // increase speed by 0.5
    ReduceSpeed, // decrease speed by 0.5
    Stun // immobile, not using yet
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

    //when item is consume, effect will be added to item collector and start counting down duration
    public void StartDurationCountDown()
    {
        effectStartingTime = DateTime.Now;
        this.enabled = true;
    }

    public bool IsEffectEnded()
    {
        // check if time passes duration, then disable effect
        TimeSpan time = DateTime.Now - effectStartingTime;
        
        if (time.Seconds >= duration)
        {
            return true;
        }
        return false;
    }
}
