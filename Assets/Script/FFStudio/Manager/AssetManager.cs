/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;

/* This class holds references to ScriptableObject assets. These ScriptableObjects are singletons, so they need to load before a Scene does.
 * Using this class ensures at least one script from a scene holds a reference to these important ScriptableObjects. */
public class AssetManager : MonoBehaviour
{
#region Fields
	public GameSettings gameSettings;
	public CurrentLevelData currentLevelData;

	public Obstacle_Runner_Pool[] obstacle_Runner_Pools;

	private void Awake()
	{
		for( var i = 0; i < obstacle_Runner_Pools.Length; i++ )
		{
			obstacle_Runner_Pools[ i ].InitPool( transform, false );
		}
	}
#endregion
}
