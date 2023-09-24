using CSTGames.DataPersistence;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private AudioMixer mixer;
	[SerializeField] private Animator animator;

	private static bool _hasStartup;

	private IEnumerator Start()
	{
		#if UNITY_EDITOR
		_hasStartup = false;
		#endif

		if (!_hasStartup)
		{
			InternalInitialization();
			animator.Play("Prepare For Startup");

			yield return new WaitForSecondsRealtime(.5f);

			animator.Play("Initial Startup");
			_hasStartup = true;
		}
	}

	public void StartGame()
	{
		LevelManager.Instance.LoadScene("Scenes/Game");
		GameDataManager.Instance.LoadGame(false);
	}

	public void QuitGame()
	{
		Debug.Log("Quiting player...");
		Application.Quit();
	}

	private void InternalInitialization()
	{
		Debug.Log("Initializing settings internally...");

		mixer.SetFloat("musicVol", UserSettings.MusicVolume);
		mixer.SetFloat("soundsVol", UserSettings.SoundsVolume);
		QualitySettings.SetQualityLevel(UserSettings.QualityLevel);
	}
}
