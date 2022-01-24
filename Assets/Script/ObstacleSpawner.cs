/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

public class ObstacleSpawner : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ) ] public MultipleEventListenerDelegateResponse level_finish_listener;
    [ BoxGroup( "Setup" ) ] public Obstacle_Runner_Pool[] obstacle_runner_pool;
    [ BoxGroup( "Setup" ) ] public float[] spawn_delays;

    private Collider spawn_collider;
    private RecycledTween recycledTween = new RecycledTween();
    private int spawn_index = 0;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		level_finish_listener.OnEnable();
	}

    private void OnDisable()
    {
		level_finish_listener.OnDisable();
    }
    private void Awake()
    {
		level_finish_listener.response = LevelFinishResponse;

        spawn_collider = GetComponentInChildren< Collider >();
	}
#endregion

#region API
    public void OnPlayerTrigger()
    {
		spawn_collider.enabled = false;

        if( 0 < spawn_delays.Length )
        {
			spawn_index = 0;
			recycledTween.Recycle( DOVirtual.DelayedCall( spawn_delays[ spawn_index ], Spawn ) );
		}
	}
#endregion

#region Implementation
    private void Spawn()
    {
		var obstacle = obstacle_runner_pool.GiveRandom<Obstacle_Runner_Pool>().GetEntity();
		obstacle.Spawn( transform.position, transform.forward );

		spawn_index++;

		if( spawn_index < spawn_delays.Length - 1 )
			recycledTween.Recycle( DOVirtual.DelayedCall( spawn_delays[ spawn_index ], Spawn ) );
	}
    private void LevelFinishResponse()
    {
        recycledTween.Kill();
    }
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
