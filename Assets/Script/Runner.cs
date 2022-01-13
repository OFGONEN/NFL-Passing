/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FFStudio;

public class Runner : MonoBehaviour
{
#region Fields
    public Mover runner_mover;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
        runner_mover = GetComponent< Mover >();
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
