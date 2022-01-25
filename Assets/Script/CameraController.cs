/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

public class CameraController : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Event Listener" ),SerializeField ] private MultipleEventListenerDelegateResponse level_finished_listener; 
	[ BoxGroup( "Event Listener" ),SerializeField ] private EventListenerDelegateResponse level_revealed_listener;
	[ BoxGroup( "Event Listener" ),SerializeField ] private EventListenerDelegateResponse finish_line_listener;
	[ BoxGroup( "Event Listener" ),SerializeField ] private EventListenerDelegateResponse ball_kick_listener;
	[ BoxGroup( "Event Listener" ),SerializeField ] private EventListenerDelegateResponse buff_start_listener;
	[ BoxGroup( "Event Listener" ),SerializeField ] private EventListenerDelegateResponse buff_end_listener;

	[ BoxGroup( "Fired Event" ), SerializeField ] private GameEvent level_start_event;

	[ BoxGroup( "Setup" ), SerializeField ] private Camera camera_main;
	[ BoxGroup( "Setup" ), SerializeField ] private Transform camera_transition;
	[ BoxGroup( "Setup" ), SerializeField ] private SharedFloat runner_movement_speed;
	[ BoxGroup( "Setup" ), SerializeField ] private ParticleSystem VFX_speedTrails;

	//Private
	private RecycledSequence recycledSequence = new RecycledSequence();
	private RecycledTween recycledTween       = new RecycledTween();
	private UnityMessage updateMethod;

	private float speed_current;
#endregion

#region Properties
#endregion

#region Unity API
	private void OnEnable()
	{
		level_finished_listener.OnEnable();
		level_revealed_listener.OnEnable();
		finish_line_listener   .OnEnable();
		ball_kick_listener     .OnEnable();
		buff_start_listener    .OnEnable();
		buff_end_listener	   .OnEnable();
	}

	private void OnDisable()
	{
		level_finished_listener.OnDisable();
		level_revealed_listener.OnDisable();
		finish_line_listener   .OnDisable();
		ball_kick_listener     .OnDisable();
		buff_start_listener    .OnDisable();
		buff_end_listener	   .OnDisable();
	}

	private void Awake()
	{
		level_revealed_listener.response = LevelRevealedResponse;
		level_finished_listener.response = LevelFinishedResponse;
		finish_line_listener.response    = LevelFinishedResponse;
		ball_kick_listener.response      = BallKickResponse;
		buff_start_listener    .response = BuffStartResponse;
		buff_end_listener	   .response = BuffEndResponse;

		updateMethod = ExtensionMethods.EmptyMethod;
	}

	private void Update()
	{
		updateMethod();
	}
#endregion

#region API
#endregion

#region Implementation
	private void LevelRevealedResponse()
	{
		var duration = GameSettings.Instance.camera_transition_duration;
		var sequence = DOTween.Sequence();

		sequence.Append( transform.DOMove( camera_transition.position, duration ) );
		sequence.Join( transform.DORotate( camera_transition.eulerAngles, duration ) );

		recycledSequence.Recycle( sequence, OnCameraTransition );
	}

	private void LevelFinishedResponse()
	{
		updateMethod = ExtensionMethods.EmptyMethod;
		BuffEndResponse(); // Just in case
	}

	private void BallKickResponse()
	{
		BuffEndResponse();
		updateMethod  = OnUpdate_Movement;
		speed_current = GameSettings.Instance.camera_ball_follow_speed;
	}

	private void BuffStartResponse()
	{
		speed_current = GameSettings.Instance.runner_movement_speed_buff;
		recycledTween.Recycle( camera_main.DOFieldOfView( GameSettings.Instance.camera_FOV_buff, GameSettings.Instance.camera_transition_FOV_duration ) );

		VFX_speedTrails.Play();
	}

	private void BuffEndResponse()
	{
		speed_current = runner_movement_speed.sharedValue;
		recycledTween.Recycle( camera_main.DOFieldOfView( GameSettings.Instance.camera_FOV_normal, GameSettings.Instance.camera_transition_FOV_duration ) );

		VFX_speedTrails.Stop();
	}

	private void OnUpdate_Movement()
	{
		var position_current   = transform.position;
		    transform.position = Vector3.MoveTowards( position_current, position_current + Vector3.forward, speed_current * Time.deltaTime );
	}

	private void OnCameraTransition()
	{
		level_start_event.Raise();

		speed_current = runner_movement_speed.sharedValue;
		updateMethod  = OnUpdate_Movement;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Handles.ArrowHandleCap( -1, camera_transition.position, camera_transition.rotation, 1, EventType.Ignore );
	}
#endif
#endregion
}