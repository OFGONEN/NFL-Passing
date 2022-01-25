/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Ball : MonoBehaviour
{
#region Fields
	[ BoxGroup( "Setup" ), SerializeField ] private MultipleEventListenerDelegateResponse level_finish_listener;
	[ BoxGroup( "Setup" ), SerializeField ] private EventListenerDelegateResponse ball_kick_listener;
	[ BoxGroup( "Setup" ), SerializeField ] private SharedReferenceNotifier ball_target_position_reference;
	[ BoxGroup( "Setup" ), SerializeField ] private ParticleSpawnEvent thrown_particle_event;
	[ BoxGroup( "Setup" ), SerializeField ] private GameEvent thrown_start_event;
	[ BoxGroup( "Setup" ), SerializeField ] private GameEvent thrown_end_event;

	private RecycledSequence thrown_sequence = new RecycledSequence();
	private Sequence kick_sequence_movement;
	private Tween kick_tween_rotation;

	private Rigidbody ball_rigidbody;
	private TrailRenderer ball_trail;
	private Collider ball_collider;
#endregion

#region Properties
#endregion

#region Unity API
	private void OnEnable()
	{
		ball_kick_listener.OnEnable();
		level_finish_listener.OnEnable();
	}

	private void OnDisable()
	{
		ball_kick_listener.OnDisable();
		level_finish_listener.OnDisable();
	}

	private void Awake()
	{
		ball_kick_listener.response = Kick;
		level_finish_listener.response = LevelFinishListener;

		ball_rigidbody = GetComponent< Rigidbody >();
		ball_rigidbody.isKinematic = true;
		ball_rigidbody.useGravity  = false;

		ball_collider  = GetComponent< Collider >();
		ball_collider.isTrigger = true;
		ball_collider.enabled   = false;

		ball_trail = GetComponent< TrailRenderer >();
		ball_trail.emitting = false;
	}
#endregion

#region API
    public void Spawn( Transform parent )
    {
		transform.SetParent( parent );
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}

	public void Throw( Vector3 position )
	{
		thrown_start_event.Raise();

		ball_trail.emitting = true;

		var current_position = transform.position;
		transform.SetParent( null );

		var duration = GameSettings.Instance.ball_throw_duration;

		var sequence = DOTween.Sequence();
		sequence.Join( transform.DOMoveX( position.x, duration ) );
		sequence.Join( transform.DOMoveZ( position.z, duration ) );
		sequence.Join( transform.DOMoveY( GameSettings.Instance.ball_throw_height, duration / 2f ) );
		sequence.Join( transform.DOMoveY( position.y, duration / 2f ).SetDelay( duration / 2f ) );
		sequence.SetEase( GameSettings.Instance.ball_throw_ease );

		thrown_sequence.Recycle( sequence, OnThrowComplete );
	}
#endregion

#region Implementation
	private void Kick()
	{
		ball_kick_listener.response = ExtensionMethods.EmptyMethod;
		ball_collider.enabled = true;
		ball_trail.emitting = true;

		var position = ( ball_target_position_reference.SharedValue as Transform ).position;

		transform.SetParent( null );
		kick_sequence_movement = transform.DOJump( position, GameSettings.Instance.ball_kick_height, 1, GameSettings.Instance.ball_kick_duration )
			.SetEase( GameSettings.Instance.ball_kick_ease )
			.OnComplete( OnKickComplete );

		kick_tween_rotation = transform.DORotate( GameSettings.Instance.ball_kick_rotation.GiveRandom(), GameSettings.Instance.ball_kick_rotation_duration, RotateMode.FastBeyond360 );
		kick_tween_rotation.SetEase( Ease.Linear );
		kick_tween_rotation.SetLoops( -1, LoopType.Incremental );
	}

	private void LevelFinishListener()
	{
		thrown_sequence.Kill();
		kick_tween_rotation    = kick_tween_rotation.KillProper();
		kick_sequence_movement = kick_sequence_movement.KillProper();

		ball_rigidbody.isKinematic = false;
		ball_rigidbody.useGravity  = true;

		ball_collider.enabled = true;
		ball_collider.isTrigger = false;
		ball_trail.emitting   = false;
	}

	private void OnKickComplete()
	{
		kick_tween_rotation.Kill();
		ball_collider.enabled = false;
		ball_trail.emitting   = false;
	}

	private void OnThrowComplete()
	{
		thrown_end_event.Raise();

		ball_trail.emitting = false;
		ball_trail.Clear();

		thrown_particle_event.Raise( "ball", transform.position );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}