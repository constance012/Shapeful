using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
	[Header("Audio Mixer"), Space]
	[SerializeField] private AudioMixer mixer;

	[Header("UI References"), Space]
	[SerializeField] private Slider _musicSlider;
	[SerializeField] private Slider _soundsSlider;
	[SerializeField] private TMP_Dropdown _qualityDropdown;
	[SerializeField] private Toggle _particlesToggle;

	// Private fields
	private TextMeshProUGUI _musicText;
	private TextMeshProUGUI _soundsText;

	private void Awake()
	{
		_musicText = _musicSlider.GetComponentInChildren<TextMeshProUGUI>();
		_soundsText = _soundsSlider.GetComponentInChildren<TextMeshProUGUI>();
	}

	private void Start()
	{
		ReloadUI();
	}

	#region Callback Method for UI Events.
	public void SetMusicVolume(float amount)
	{
		mixer.SetFloat("musicVol", amount);

		_musicText.text = $"Music: {ConvertDecibelToText(amount)}";
		UserSettings.MusicVolume = amount;
	}

	public void SetSoundsVolume(float amount)
	{
		mixer.SetFloat("soundsVol", amount);

		_soundsText.text = $"Sounds: {ConvertDecibelToText(amount)}";
		UserSettings.SoundsVolume = amount;
	}

	public void SetQualityLevel(int index)
	{
		QualitySettings.SetQualityLevel(index);
		UserSettings.QualityLevel = index;
	}

	public void ToggleBackgroundParticles(bool state)
	{
		LevelManager.Instance.EnableParticleEffect(state);
		UserSettings.EnableBackgroundParticles = state;
	}

	public void ResetToDefault()
	{
		UserSettings.ResetToDefault(UserSettings.SettingSection.All);
		ReloadUI();
	}
	#endregion

	private string ConvertDecibelToText(float amount)
	{
		float normalized = 1f - (Mathf.Abs(amount) / 80f);
		return (normalized * 100f).ToString("0");
	}

	private void ReloadUI()
	{
		float musicVol = UserSettings.MusicVolume;
		float soundsVol = UserSettings.SoundsVolume;

		_musicSlider.value = musicVol;
		_soundsSlider.value = soundsVol;
		_qualityDropdown.value = UserSettings.QualityLevel;

		_particlesToggle.isOn = UserSettings.EnableBackgroundParticles;
	}
}
