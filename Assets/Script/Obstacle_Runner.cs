/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;

public class Obstacle_Runner : MonoBehaviour
{
#region Fields
    public Obstacle_Runner_Pool pool;
	public MultipleEventListenerDelegateResponse loadLevelListener;

    // Private 
	private Animator obstacle_animator;
    private Mover obstacle_mover;
	private ToggleRagdoll obstacle_ragdoll;

	private RecycledTween recycledTween = new RecycledTween();
#endregion

#region Properties
#endregion

#region Unity API
	private void OnEnable()
	{
		loadLevelListener.OnEnable();
	}

	private void OnDisable()
	{
		loadLevelListener.OnDisable();
	}

    private void Awake()
    {
        obstacle_animator = GetComponentInChildren< Animator >();
        obstacle_mover    = GetComponent< Mover >();
        obstacle_ragdoll  = GetComponent< ToggleRagdoll >();

		loadLevelListener.response = ReturnToPool;
	}
#endregion

#region API
    public void Spawn( Vector3 position, Vector3 direction )
    {
		transform.position = position;
		transform.forward  = direction;
		gameObject.SetActive( true );

		GetComponent< Collider >().enabled = true;

		obstacle_animator.enabled = true;
		obstacle_mover.Enable();
		obstacle_ragdoll.Deactivate();
	}

    public void OnDestroyed()
    {
		obstacle_mover.Disable();
		obstacle_animator.enabled = false;
		obstacle_ragdoll.Activate();
		obstacle_ragdoll.GiveForce( transform.forward * -1f * GameSettings.Instance.obstacle_runner_ragdoll_force, ForceMode.Impulse );
		recycledTween.Recycle( DOVirtual.DelayedCall( GameSettings.Instance.obstacle_runner_ragdoll_duration, ReturnToPool ) );
	}

    public void OnInteraction()
    {
		obstacle_animator.SetTrigger( "interact" );
		recycledTween.Recycle( DOVirtual.DelayedCall( GameSettings.Instance.obstacle_runner_ragdoll_duration, ReturnToPool ) );
	}
#endregion

#region Implementation
    private void ReturnToPool()
    {
		recycledTween.Kill();
		pool.ReturnEntity( this );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
