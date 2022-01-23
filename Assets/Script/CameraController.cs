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
	[ BoxGroup( "Event Listener" ),SerializeField ] private EventListenerDelegateResponse buff_start_listener;
	[ BoxGroup( "Event Listener" ),SerializeField ] private EventListenerDelegateResponse buff_end_listener;

	[ BoxGroup( "Fired Event" ), SerializeField ] private GameEvent level_start_event;

	[ BoxGroup( "Setup" ), SerializeField ] private Camera camera_main;
	[ BoxGroup( "Setup" ), SerializeField ] private Transform camera_transition;
	[ BoxGroup( "Setup" ), SerializeField ] private SharedFloat runner_movement_speed;

	//Private
	private RecycledSequence recycledSequence = new RecycledSequence();
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
		buff_start_listener    .OnEnable();
		buff_end_listener	   .OnEnable();
	}

	private void OnDisable()
	{
		level_finished_listener.OnDisable();
		level_revealed_listener.OnDisable();
		buff_start_listener    .OnDisable();
		buff_end_listener	   .OnDisable();
	}

	private void Awake()
	{
		level_finished_listener.response = ExtensionMethods.EmptyMethod;
		level_revealed_listener.response = ExtensionMethods.EmptyMethod;
		buff_start_listener    .response = ExtensionMethods.EmptyMethod;
		buff_end_listener	   .response = ExtensionMethods.EmptyMethod;

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
	private void LevelRevealedListener()
	{
		var duration = GameSettings.Instance.camera_transition_duration;
		var sequence = DOTween.Sequence();

		sequence.Append( transform.DOMove( camera_transition.position, duration ) );
		sequence.Join( transform.DORotate( camera_transition.eulerAngles, duration ) );

		recycledSequence.Recycle( sequence, OnCameraTransition );
	}

	private void LevelFinishedListener()
	{
		updateMethod = ExtensionMethods.EmptyMethod;
		BuffEndListener(); // Just in case
	}

	private void BuffStartListener()
	{
		speed_current           = GameSettings.Instance.runner_movement_speed_buff;
		camera_main.fieldOfView = GameSettings.Instance.camera_FOV_buff;

		//todo(ofg): ENABLE speed trails particle
	}

	private void BuffEndListener()
	{
		speed_current           = runner_movement_speed.sharedValue;
		camera_main.fieldOfView = GameSettings.Instance.camera_FOV_normal;

		//todo(ofg): DISABLE speed trails particle
	}

	private void OnUpdate_Movement()
	{
		var position_current   = transform.position;
		    transform.position = Vector3.MoveTowards( position_current, position_current + transform.forward, speed_current * Time.deltaTime );
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