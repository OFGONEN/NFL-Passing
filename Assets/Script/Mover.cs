/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FFStudio;

public class Mover : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ) ] public SharedFloat movement_speed_shared;
    [ BoxGroup( "Setup" ) ] public Vector3 movement_axis;

    private float movement_speed_cofactor = 1f;
    private UnityMessage updateMethod;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		updateMethod = ExtensionMethods.EmptyMethod;
	}

    private void Update()
    {
		updateMethod(); //TODO(ofg) Make this class use Cluster
	}
#endregion

#region API
    public void Enable()
    {
		updateMethod = OnUpdate_Move;
	}

    public void Disable()
    {
		updateMethod = ExtensionMethods.EmptyMethod;
    }

    public void ChangeSpeed( float speed )
    {
		movement_speed_cofactor = speed / movement_speed_shared.sharedValue;
	}

    public void DefaultSpeed()
    {
		movement_speed_cofactor = 1f;
	}
#endregion

#region Implementation
    private void OnUpdate_Move()
    {
		var position_current = transform.position;
		transform.position = Vector3.MoveTowards( position_current, position_current + movement_axis, movement_speed_shared.sharedValue * Time.deltaTime * movement_speed_cofactor );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
