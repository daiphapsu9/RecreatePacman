using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
