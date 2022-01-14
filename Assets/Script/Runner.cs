/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FFStudio;

public class Runner : MonoBehaviour
{
#region Fields
    private Mover runner_mover;
    private ToggleRagdoll runner_ragdoll;
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
		runner_mover.ChangeSpeed( GameSettings.Instance.runner_movement_buff );
	}

    public void OnDoor_Buff_End()
    {
		runner_mover.DefaultSpeed();
	}

    public void OnFinishLine()
    {
		bool hasBall = false;

        if( hasBall )
			runner_mover.Disable(); //TODO(ofg) Run To goal kick position
        else
			runner_mover.Disable(); 
	}

    public void OnObstacle()
    {
		bool hasBall = false;

        if( hasBall )
			runner_mover.Disable();
        else
			runner_mover.Disable(); //TODO(ofg) Play dodge animation, Disable collider
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
