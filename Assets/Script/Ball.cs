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
	[ BoxGroup( "Setup" ) ] public MultipleEventListenerDelegateResponse level_finish_listener;
	[ BoxGroup( "Setup" ) ] public EventListenerDelegateResponse ball_kick_listener;
	[ BoxGroup( "Setup" ) ] public SharedReferenceNotifier ball_target_position_reference;
	[ BoxGroup( "Setup" ) ] public GameEvent thrown_start_event;
	[ BoxGroup( "Setup" ) ] public GameEvent thrown_end_event;

	private RecycledSequence thrown_sequence = new RecycledSequence();
	private Sequence kick_tween;
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

		var current_position = transform.position;
		transform.SetParent( null );

		var duration = GameSettings.Instance.ball_throw_duration;

		var sequence = DOTween.Sequence();
		sequence.Join( transform.DOMoveX( position.x, duration ) );
		sequence.Join( transform.DOMoveZ( position.z, duration ) );
		sequence.Join( transform.DOMoveY( GameSettings.Instance.ball_throw_height, duration / 2f ) );
		sequence.Join( transform.DOMoveY( position.y, duration / 2f ).SetDelay( duration / 2f ) );

		sequence.AppendCallback( thrown_end_event.Raise );
	}
#endregion

#region Implementation
	private void Kick()
	{
		var position = ( ball_target_position_reference.SharedValue as Transform ).position;

		transform.SetParent( null );
		kick_tween = transform.DOJump( position, GameSettings.Instance.ball_kick_height, 1, GameSettings.Instance.ball_kick_duration )
			.SetEase( Ease.Linear );
	}

	private void LevelFinishListener()
	{
		kick_tween = kick_tween.KillProper();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}