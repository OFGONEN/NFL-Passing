/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using FFStudio;

public class Gate : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ) ] public UnityEvent unityEvent;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    public void OnTriggerListener_Enter( Collider collider )
    {
        var runner = collider.GetComponentInParent< Runner >();

        if( runner.HasBall )
			unityEvent.Invoke();
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
