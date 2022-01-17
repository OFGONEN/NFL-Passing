/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Runner : MonoBehaviour
{
#region Fields
	[ BoxGroup( "Setup" ) ] public SharedReferenceNotifier runner_ballKick_Transform;

    private Mover runner_mover;
    private ToggleRagdoll runner_ragdoll;

	[ SerializeField ] private float movement_dodge_direction; // +1 is right
	[ SerializeField ] private bool hasBall;
	// [ SerializeField, ReadOnly ] private bool hasBall;

	private Vector3 runner_ballKick_Position;

	private RecycledSequence recycledSequence = new RecycledSequence();
	private RecycledTween recycledTween       = new RecycledTween();
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
        runner_mover   = GetComponent< Mover >();
        runner_ragdoll = GetComponent< ToggleRagdoll >();

		runner_ragdoll.Deactivate();
	}
#endregion

#region API
	[ Button() ]
    public void OnLevelStart()
    {
		runner_mover.Enable();
	}

    public void OnLevelFinish()
    {
		runner_mover.Disable();
    }

    public void OnDoor_Buff_Start()
    {
		runner_mover.ChangeSpeed( GameSettings.Instance.runner_movement_speed_buff );
	}

    public void OnDoor_Buff_End()
    {
		runner_mover.DefaultSpeed();
	}

	//! This can happen while runner has a buff
	[ Button() ]
    public void OnFinishLine()
    {
        if( hasBall )
		{
			//TODO(ofg) disable collider
			runner_mover.Disable();
			runner_ballKick_Position = ( runner_ballKick_Transform.SharedValue as Transform ).position;

			var target_position_local = transform.InverseTransformPoint( runner_ballKick_Position );
			var duration = target_position_local.z / runner_mover.Speed; // X / V = T

			recycledTween.Recycle( transform.DOMove( runner_ballKick_Position, duration ).OnUpdate( OnBallKick_Move_Update ),
				OnBallKick_Move_Complete );
		}
        else
		{
			runner_mover.Disable(); 
			//TODO(ofg) disable collider
		}
	}

	[ Button() ]
    public void OnObstacle()
    {
        if( hasBall )
		{
			runner_mover.Disable();
			runner_ragdoll.Activate();
			runner_ragdoll.GiveForce( transform.forward * -1f * GameSettings.Instance.runner_ragdoll_force, ForceMode.Impulse );
		}
        else
		{
			var position = transform.position;

			var sequence = DOTween.Sequence();
			sequence.Append( transform.DOMoveX( position.x + GameSettings.Instance.runner_movement_speed_dodge * movement_dodge_direction, 
				GameSettings.Instance.runner_movement_dodge_duration / 2f ) );
			sequence.Append( transform.DOMoveX( position.x,
				GameSettings.Instance.runner_movement_dodge_duration / 2f ) );
			
			recycledSequence.Recycle( sequence, OnDodgeComplete );
			//TODO(ofg) Play dodge animation, Disable collider, 
		}
	}
	#endregion

	#region Implementation
	private void OnDodgeComplete()
	{
		//TODO(ofg) Enable collider
	}

	private void OnBallKick_Move_Update()
	{
		transform.LookAtOverTimeAxis( runner_ballKick_Position + Vector3.forward, Vector3.up, Time.deltaTime * GameSettings.Instance.runner_look_speed );
	}

	private void OnBallKick_Move_Complete()
	{
		transform.LookAtAxis( runner_ballKick_Position + Vector3.forward, Vector3.up );
		//TODO(ofg) Play ball kick animation
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}