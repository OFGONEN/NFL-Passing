/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FFStudio
{
	public class AppManager : MonoBehaviour
	{
#region Fields
		[ Header( "Event Listeners" ) ]
		public EventListenerDelegateResponse loadNewLevelListener;
		public EventListenerDelegateResponse resetLevelListener;
		public EventListenerDelegateResponse vibrateListener;

		[ Header( "Fired Events" ) ]
		public GameEvent levelLoaded;
		public GameEvent cleanUpEvent;

		[ Header( "Fired Events" ) ]
		public SharedFloatNotifier levelProgress;
#endregion

#region Unity API
		private void OnEnable()
		{
			loadNewLevelListener.OnEnable();
			resetLevelListener.OnEnable();
			vibrateListener.OnEnable();
		}
		
		private void OnDisable()
		{
			loadNewLevelListener.OnDisable();
			resetLevelListener.OnDisable();
			vibrateListener.OnDisable();
		}
		
		private void Awake()
		{
			loadNewLevelListener.response = LoadNewLevel;
			resetLevelListener.response   = ResetLevel;
			vibrateListener.response 	  = Handheld.Vibrate;
		}

		private void Start()
		{
			StartCoroutine( LoadLevel() );
		}
#endregion

#region API
#endregion

#region Implementation
		private void ResetLevel()
		{
			var operation = SceneManager.UnloadSceneAsync( CurrentLevelData.Instance.levelData.sceneIndex );
			operation.completed += ( AsyncOperation operation ) => StartCoroutine( LoadLevel() );
		}
		
		private IEnumerator LoadLevel()
		{
			CurrentLevelData.Instance.currentLevel_Real = PlayerPrefs.GetInt( "Level", 1 );
			CurrentLevelData.Instance.currentLevel_Shown = PlayerPrefs.GetInt( "Consecutive Level", 1 );

			CurrentLevelData.Instance.LoadCurrentLevelData();

			cleanUpEvent.Raise();
			// SceneManager.LoadScene( CurrentLevelData.Instance.levelData.sceneIndex, LoadSceneMode.Additive );
			var operation = SceneManager.LoadSceneAsync( CurrentLevelData.Instance.levelData.sceneIndex, LoadSceneMode.Additive );

			levelProgress.SharedValue = 0;

			while( !operation.isDone )
			{
				yield return null;

				levelProgress.SharedValue = operation.progress;
			}

			levelLoaded.Raise();
		}
		
		private void LoadNewLevel()
		{
			CurrentLevelData.Instance.currentLevel_Real++;
			CurrentLevelData.Instance.currentLevel_Shown++;
			PlayerPrefs.SetInt( "Level", CurrentLevelData.Instance.currentLevel_Real );
			PlayerPrefs.SetInt( "Consecutive Level", CurrentLevelData.Instance.currentLevel_Shown );

			var operation = SceneManager.UnloadSceneAsync( CurrentLevelData.Instance.levelData.sceneIndex );
			operation.completed += ( AsyncOperation operation ) => StartCoroutine( LoadLevel() );
		}
#endregion
	}
}