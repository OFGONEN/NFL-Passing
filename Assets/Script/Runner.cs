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
	[ SerializeField, BoxGroup( "Setup" ) ] private SharedReferenceNotifier runner_ballKick_Transform;
	[ SerializeField, BoxGroup( "Setup" ) ] private SharedReferenceNotifier runner_ball_reference;
	[ SerializeField, BoxGroup( "Setup" ) ] private Transform runner_ball_parent;

    private Mover runner_mover;
    private ToggleRagdoll runner_ragdoll;
	private Animator runner_animator; // bool: run, buffed, ball; trigger: throw, dodge, kick

	[ SerializeField ] private float movement_dodge_direction; // +1 is right
	[ SerializeField ] private bool has_Ball;
	[ SerializeField ] private bool has_Buff;
	// [ SerializeField, ReadOnly ] private bool hasBall;

	private Vector3 runner_ballKick_Position;
	private RecycledSequence recycledSequence = new RecycledSequence();
	private RecycledTween recycledTween       = new RecycledTween();
#endregion

#region Properties
	public bool HasBall => has_Ball;
	public bool HasBuff => has_Buff;
#endregion

#region Unity API
    private void Awake()
    {
        runner_mover    = GetComponent< Mover >();
        runner_ragdoll  = GetComponent< ToggleRagdoll >();
        runner_animator = GetComponentInChildren< Animator >();

		runner_ragdoll.Deactivate();
	}
#endregion

#region API
	[ Button() ]
    public void OnLevelStart()
    {
		runner_mover.Enable();
		runner_animator.SetBool( "run", true );
	}

    public void OnLevelFinish()
    {
		runner_mover.Disable();
		runner_animator.SetBool( "run", false );
    }

    public void OnDoor_Buff_Start()
    {
		runner_mover.ChangeSpeed( GameSettings.Instance.runner_movement_speed_buff );
		runner_animator.SetBool( "buffed", true );
	}

    public void OnDoor_Buff_End()
    {
		runner_mover.DefaultSpeed();
		runner_animator.SetBool( "buffed", false );
	}

	[ Button() ]
    public void OnFinishLine()
    {
		runner_animator.SetBool( "buffed", false ); //Info: If finish line can happen while on buff

        if( has_Ball )
		{
			//TODO(ofg) disable collider
			runner_mover.Disable();
			runner_ballKick_Position = ( runner_ballKick_Transform.SharedValue as Transform ).position;

			// Move towards kick position
			var target_position_local = transform.InverseTransformPoint( runner_ballKick_Position );
			var duration = target_position_local.z / runner_mover.Speed; // X / V = T

			recycledTween.Recycle( transform.DOMove( runner_ballKick_Position, duration ).OnUpdate( OnBallKick_Move_Update ),
				OnBallKick_Move_Complete );
		}
        else
		{
			runner_mover.Disable();
			runner_animator.SetBool( "run", false );
			//TODO(ofg) disable collider
		}
	}

	[ Button() ]
    public void OnObstacle()
    {
        if( has_Ball )
		{
			runner_animator.enabled = false;
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

			runner_animator.SetTrigger( "dodge" );
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
		runner_animator.SetBool( "run", false );
		runner_animator.SetTrigger( "kick" );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}