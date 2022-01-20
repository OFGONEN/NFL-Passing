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
	[ SerializeField, BoxGroup( "Event Listener" ) ] private EventListenerDelegateResponse level_start_listener;
	[ SerializeField, BoxGroup( "Event Listener" ) ] private MultipleEventListenerDelegateResponse level_finish_listener;
	[ SerializeField, BoxGroup( "Event Listener" ) ] private EventListenerDelegateResponse finish_line_listener;
	[ SerializeField, BoxGroup( "Event Listener" ) ] private EventListenerDelegateResponse input_finger_down_listener;
	[ SerializeField, BoxGroup( "Event Listener" ) ] private EventListenerDelegateResponse ball_thrown_start_listener;
	[ SerializeField, BoxGroup( "Event Listener" ) ] private EventListenerDelegateResponse ball_thrown_end_listener;
	[ SerializeField, BoxGroup( "Event Listener" ) ] private EventListenerDelegateResponse buff_start_listener;
	[ SerializeField, BoxGroup( "Event Listener" ) ] private EventListenerDelegateResponse buff_end_listener;

	[ SerializeField, BoxGroup( "Setup" ) ] private SharedReferenceNotifier runner_ballKick_Transform;
	[ SerializeField, BoxGroup( "Setup" ) ] private SharedReferenceNotifier runner_ball_reference;
	[ SerializeField, BoxGroup( "Setup" ) ] private SharedReferenceNotifier runner_reference;
	[ SerializeField, BoxGroup( "Setup" ) ] private Transform runner_ball_parent_throw;
	[ SerializeField, BoxGroup( "Setup" ) ] private Transform runner_ball_parent;
	[ SerializeField, BoxGroup( "Setup" ) ] private bool runner_startWithBall;

    private Mover runner_mover;
    private ToggleRagdoll runner_ragdoll;
	private Animator runner_animator; // bool: run, buffed, ball; trigger: throw, dodge, kick

	[ SerializeField ] private float movement_dodge_direction; // +1 is right
	[ SerializeField ] private bool has_Ball;
	[ SerializeField ] private bool has_Buff;

	private Ball runner_ball;
	private Vector3 runner_ballKick_Position;
	private RecycledSequence recycledSequence = new RecycledSequence();
	private RecycledTween recycledTween       = new RecycledTween();
#endregion

#region Properties
	public Vector3 BallThrowPosition => runner_ball_parent_throw.position;
	public bool HasBall => has_Ball;
	public bool HasBuff => has_Buff;
#endregion

#region Unity API
	private void OnEnable()
	{
		level_start_listener.OnEnable();
		level_finish_listener.OnEnable();
		finish_line_listener.OnEnable();

		input_finger_down_listener.OnEnable();
		ball_thrown_start_listener.OnEnable();
		ball_thrown_end_listener.OnEnable();

		buff_start_listener.OnEnable();
		buff_end_listener.OnEnable();
	}

	private void OnDisable()
	{
		level_start_listener.OnDisable();
		level_finish_listener.OnDisable();
		finish_line_listener.OnDisable();

    	input_finger_down_listener.OnDisable();
		ball_thrown_start_listener.OnDisable();
		ball_thrown_end_listener.OnDisable();

		buff_start_listener.OnDisable();
		buff_end_listener.OnDisable();
	}

    private void Awake()
    {
        runner_mover    = GetComponent< Mover >();
        runner_ragdoll  = GetComponent< ToggleRagdoll >();
        runner_animator = GetComponentInChildren< Animator >();

		level_start_listener.response  = LevelStartListener;
		level_finish_listener.response = LevelFinishListener;
		finish_line_listener.response  = FinishLineListener;
		buff_start_listener.response   = BuffStartListener;
		buff_end_listener.response     = BuffEndListener;

		runner_ragdoll.Deactivate();

		input_finger_down_listener.response = ExtensionMethods.EmptyMethod;
	}

	private void Start()
	{
		ball_thrown_end_listener.response = ExtensionMethods.EmptyMethod;

		if( runner_startWithBall )
		{
			SpawnBall(); // Enables input

			has_Ball = true;
			ball_thrown_start_listener.response = ExtensionMethods.EmptyMethod;
		}
		else
			ball_thrown_start_listener.response = BallThrown_StartListener;
	}
#endregion

#region API
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

	public void ThrowBall()
	{
		var runner_other = runner_reference.SharedValue as Runner;
		var position     = runner_other.BallThrowPosition;
		position.z 		 += GameSettings.Instance.ball_throw_duration * runner_mover.Speed;

		runner_ball.Throw( position );
		has_Ball = false;

		ball_thrown_start_listener.response = BallThrown_StartListener;
		input_finger_down_listener.response = ExtensionMethods.EmptyMethod;
	}
#endregion

#region Implementation
    private void LevelStartListener()
    {
		runner_mover.Enable();
		runner_animator.SetBool( "run", true );
	}

    private void LevelFinishListener()
    {
		runner_mover.Disable();
		runner_animator.SetBool( "run", false );
    }

    private void FinishLineListener()
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

    private void BuffStartListener()
    {
		runner_mover.ChangeSpeed( GameSettings.Instance.runner_movement_speed_buff );
		runner_animator.SetBool( "buffed", true );
	}

    private void BuffEndListener()
    {
		runner_mover.DefaultSpeed();
		runner_animator.SetBool( "buffed", false );
	}

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

	private void SpawnBall()
	{
		runner_ball = runner_ball_reference.SharedValue as Ball;
		runner_ball.Spawn( runner_ball_parent );

		input_finger_down_listener.response = ThrowAnimation;
	}

	private void ThrowAnimation()
	{
		runner_animator.SetTrigger( "throw" );
		input_finger_down_listener.response = ExtensionMethods.EmptyMethod;
	}

	private void BallThrown_StartListener()
	{
		runner_animator.SetBool( "ball", true );
		has_Ball = true;

		ball_thrown_start_listener.response = ExtensionMethods.EmptyMethod;
		ball_thrown_end_listener.response = BallThrown_EndListener;
	}

	private void BallThrown_EndListener()
	{
		runner_animator.SetBool( "ball", false );

		SpawnBall(); // Enables input

		ball_thrown_start_listener.response = ExtensionMethods.EmptyMethod;
		ball_thrown_end_listener.response = ExtensionMethods.EmptyMethod;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}