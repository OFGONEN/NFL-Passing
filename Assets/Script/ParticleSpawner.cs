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
    [ BoxGroup( "Setup" ) ] public ParticleData[] particleDatas;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    public void Spawn( int index )
    {
		var data = particleDatas[ index ];

		Transform parent = data.parent ? transform : null;
		particle_event.Raise( data.alias, transform.position + data.offset, parent );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
		for( var i = 0; i < particleDatas.Length; i++ )
		{
			var data = particleDatas[ i ];
			Handles.Label( transform.position + data.offset, "Particle Spawn:" + data.alias);
			Handles.DrawWireCube( transform.position + data.offset, Vector3.one / 4f );
		}
	}
#endif
#endregion
}
