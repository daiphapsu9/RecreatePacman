using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class that enable Pacman and Ghost to pick up item and store it effect
// it also calculate time to remove effect based on effect duration
public abstract class ItemCollector : MonoBehaviour
{
    public Effect appliedEffect = null; 

    public abstract void OnPickupItem(CollectableItem item);

    public virtual void Update()
    {
        if (appliedEffect != null)
        {
            if (appliedEffect.IsEffectEnded())
            {
                appliedEffect = null;
            }
        }
    }
}
