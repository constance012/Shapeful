using CSTGames.DataPersistence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationMenu : MonoBehaviour, ISaveDataTransceiver
{
	[Header("UI References"), Space]
	[SerializeField] private Slider redSlider;
	[SerializeField] private Slider greenSlider;
	[SerializeField] private Slider blueSlider;

	[Space]
	[SerializeField] private Image preview;

	// Private fields.
	private TextMeshProUGUI _redText;
	private TextMeshProUGUI _greenText;
	private TextMeshProUGUI _blueText;

	private Color _currentColor;

	private void Awake()
	{
		_redText = redSlider.GetComponentInChildren<TextMeshProUGUI>("Value");
		_greenText = greenSlider.GetComponentInChildren<TextMeshProUGUI>("Value");
		_blueText = blueSlider.GetComponentInChildren<TextMeshProUGUI>("Value");
	}

	private void Start()
	{
		GameDataManager.Instance.DistributeDataToTransceivers();
	}

	#region Interface Methods.
	public void SaveData(GameData data)
	{
		data.playerColor = _currentColor;
	}

	public void LoadData(GameData data)
	{
		_currentColor = data.playerColor;
		ReloadUI();
	}
	#endregion

	#region Callback Method for UI Events.
	public void SetRedValue(float amount)
	{
		_currentColor.r = amount;
		_redText.text = (255f * amount).ToString("0");

		preview.color = _currentColor;
	}

	public void SetGreenValue(float amount)
	{
		_currentColor.g = amount;
		_greenText.text = (255f * amount).ToString("0");

		preview.color = _currentColor;
	}

	public void SetBlueValue(float amount)
	{
		_currentColor.b = amount;
		_blueText.text = (255f * amount).ToString("0");

		preview.color = _currentColor;
	}

	public void ConfirmChanges()
	{
		GameDataManager.Instance.SaveGame();
		gameObject.SetActive(false);
	}
	#endregion

	private void ReloadUI()
	{
		redSlider.value = _currentColor.r;
		greenSlider.value = _currentColor.g;
		blueSlider.value = _currentColor.b;

		preview.color = _currentColor;
	}
}
