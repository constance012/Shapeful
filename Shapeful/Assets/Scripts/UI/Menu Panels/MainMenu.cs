using CSTGames.DataPersistence;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
	[Header("Audio Mixer"), Space]
	[SerializeField] private AudioMixer mixer;

	[Header("UI References"), Space]
	[SerializeField] private TextMeshProUGUI versionText;

	[Header("Game Object References"), Space]
	[SerializeField] private Animator animator;

	// Private fields.
	private static bool _hasStartedUp;

	private IEnumerator Start()
	{
#if UNITY_EDITOR
		_hasStartedUp = false;
#endif

		GameDataManager.Instance.LoadGame(false);
		versionText.text = Application.version;
		
		if (!_hasStartedUp)
		{
			InternalInitialization();
			animator.Play("Prepare For Startup");

			yield return new WaitForSecondsRealtime(.5f);
			animator.Play("Initial Startup");
			

			_hasStartedUp = true;
		}
	}

	#region Callback method for UI Events.
	public void StartGame()
	{
		LevelManager.Instance.LoadScene("Scenes/Game");
	}

	public void QuitGame()
	{
		Debug.Log("Quiting player...");
		Application.Quit();
	}
	#endregion

	private void InternalInitialization()
	{
		Debug.Log("Initializing settings internally...");

		mixer.SetFloat("musicVol", UserSettings.MusicVolume);
		mixer.SetFloat("soundsVol", UserSettings.SoundsVolume);
		QualitySettings.SetQualityLevel(UserSettings.QualityLevel);

		LevelManager.Instance.EnableParticleEffect(UserSettings.EnableBackgroundParticles);
	}
}
