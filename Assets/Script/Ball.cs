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
	[ BoxGroup( "Setup" ) ] public EventListenerDelegateResponse ball_kick_listener;
	[ BoxGroup( "Setup" ) ] public GameEvent thrown_start_event;
	[ BoxGroup( "Setup" ) ] public GameEvent thrown_end_event;

	private RecycledSequence thrown_sequence = new RecycledSequence();
#endregion

#region Properties
#endregion

#region Unity API
	private void OnEnable()
	{
		ball_kick_listener.OnEnable();
	}

	private void OnDisable()
	{
		ball_kick_listener.OnDisable();
	}

	private void Awake()
	{
		ball_kick_listener.response = Kick;
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
	public void Kick()
	{
		FFLogger.Log( "Kick" );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}