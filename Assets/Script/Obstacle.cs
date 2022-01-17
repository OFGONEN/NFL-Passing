/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FFStudio;

public class Obstacle : MonoBehaviour
{
#region Fields
    public UnityEvent onDestroyEvent;
    public UnityEvent onInteractionEvent;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    public virtual void OnTriggerListener_Enter( Collider collider )
    {
        var runner = collider.GetComponent< TriggerListener >().AttachedComponent as Runner;

        if( runner.HasBuff )
			onDestroyEvent.Invoke();
		else
        {
			runner.OnObstacle();
			onInteractionEvent.Invoke();
		}
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
