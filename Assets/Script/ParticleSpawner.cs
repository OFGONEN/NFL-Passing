/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using UnityEditor;

public class ParticleSpawner : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ) ] public ParticleSpawnEvent particle_event;
    [ BoxGroup( "Setup" ) ] public string particle_alias;
    [ BoxGroup( "Setup" ) ] public bool particle_parent;
    [ BoxGroup( "Setup" ) ] public Vector3 particle_offset;

    private Vector3 Position => transform.position + particle_offset;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
    public void Spawn()
    {
		Transform parent = particle_parent ? transform : null;
		particle_event.Raise( particle_alias, Position, parent );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
		Handles.Label( transform.position + particle_offset, "Particle Spawn:" + particle_alias );
		Handles.DrawWireCube( transform.position + particle_offset, Vector3.one / 2f );
	}
#endif
#endregion
}
