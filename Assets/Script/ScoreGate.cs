/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FFStudio;
using TMPro;
using Sirenix.OdinInspector;

public class ScoreGate : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ) ] public float gate_score;
    [ BoxGroup( "Setup" ) ] public float gate_score_text;
    [ BoxGroup( "Setup" ) ] public UnityEvent level_score_win;
    [ BoxGroup( "Setup" ) ] public UnityEvent level_score_lost;
    [ BoxGroup( "Setup" ) ] public SharedFloatNotifier level_score;
    [ BoxGroup( "Setup" ) ] public TextMeshProUGUI gate_text;

    [ BoxGroup( "Fired Events" ) ] public GameEvent level_complete_event;
    [ BoxGroup( "Fired Events" ) ] public GameEvent level_failed_event;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
        gate_text      = GetComponentInChildren< TextMeshProUGUI >();
        gate_text.text = gate_score_text.ToString();
    }
#endregion

#region API
    public void OnBallTrigger()
    {
        if( level_score.SharedValue >= gate_score ) // Gate Passed
			level_score_win.Invoke();
        else
			level_score_lost.Invoke();
    }
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnValidate()
    {
        gate_text      = GetComponentInChildren< TextMeshProUGUI >();
        gate_text.text = gate_score_text.ToString();
	}
#endif
#endregion
}