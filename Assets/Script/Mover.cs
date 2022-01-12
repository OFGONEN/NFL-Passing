/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FFStudio;

public class Mover : MonoBehaviour , IClusterEntity
{
#region Fields
    [ BoxGroup( "Setup" ) ] public Cluster cluster;
    [ BoxGroup( "Setup" ) ] public SharedFloat movement_speed_shared;
    [ BoxGroup( "Setup" ) ] public Vector3 movement_axis;

    private float movement_speed_cofactor = 1f;
    private UnityMessage updateMethod;
#endregion

#region Properties
#endregion

#region Unity API
	private void OnEnable()
	{
		Subscribe_Cluster();
	}

	private void OnDisable()
	{
		UnSubscribe_Cluster();
	}

    private void Awake()
    {
		updateMethod = ExtensionMethods.EmptyMethod;
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

#region ICluster
	public void Subscribe_Cluster()
	{
		cluster.Subscribe( this );
	}

	public void UnSubscribe_Cluster()
	{
		cluster.UnSubscribe( this );
	}

	public void OnUpdate_Cluster()
	{
		updateMethod();
	}

	public int InstanceID()
	{
		return GetInstanceID();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
