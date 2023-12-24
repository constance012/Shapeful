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
	[SerializeField] private Toggle sameAsPrimaryToggle;

	[Space]
	[SerializeField] private Image primaryPreview;
	[SerializeField] private Image secondaryPreview;

	public static bool primaryColorSelected;

	// Private fields.
	private TextMeshProUGUI _redText;
	private TextMeshProUGUI _greenText;
	private TextMeshProUGUI _blueText;

	private Color _primaryColor;
	private Color _secondaryColor;

	private void Awake()
	{
		_redText = redSlider.GetComponentInChildren<TextMeshProUGUI>("Value");
		_greenText = greenSlider.GetComponentInChildren<TextMeshProUGUI>("Value");
		_blueText = blueSlider.GetComponentInChildren<TextMeshProUGUI>("Value");
		
		GameDataManager.Instance.DistributeDataToTransceivers();
	}

	#region Interface Methods.
	public void SaveData(GameData data)
	{
		data.primaryColor = _primaryColor;
		data.secondaryColor = _secondaryColor;
	}

	public void LoadData(GameData data)
	{
		_primaryColor = data.primaryColor;
		_secondaryColor = data.secondaryColor;

		ReloadUI();
	}
	#endregion

	#region Callback Method for UI Events.
	public void SetRedValue(float amount)
	{
		if (primaryColorSelected)
		{
			_primaryColor.r = amount;
			primaryPreview.color = _primaryColor;

			if (sameAsPrimaryToggle.isOn)
				secondaryPreview.color = _primaryColor;
		}
		else
		{
			_secondaryColor.r = amount;
			secondaryPreview.color = _secondaryColor;
		}

		_redText.text = (255f * amount).ToString("0");
	}

	public void SetGreenValue(float amount)
	{
		if (primaryColorSelected)
		{
			_primaryColor.g = amount;
			primaryPreview.color = _primaryColor;

			if (sameAsPrimaryToggle.isOn)
				secondaryPreview.color = _primaryColor;
		}
		else
		{
			_secondaryColor.g = amount;
			secondaryPreview.color = _secondaryColor;
		}

		_greenText.text = (255f * amount).ToString("0");
	}

	public void SetBlueValue(float amount)
	{
		if (primaryColorSelected)
		{
			_primaryColor.b = amount;
			primaryPreview.color = _primaryColor;

			if (sameAsPrimaryToggle.isOn)
				secondaryPreview.color = _primaryColor;
		}
		else
		{
			_secondaryColor.b = amount;
			secondaryPreview.color = _secondaryColor;
		}

		_blueText.text = (255f * amount).ToString("0");
	}

	public void ToggleSecondaryAsPrimary(bool state)
	{
		SetSlidersInteractable(!state);
		UserSettings.SecondaryColorSameAsPrimary = state;

		secondaryPreview.color = state ? _primaryColor : _secondaryColor;
	}

	public void ConfirmChanges()
	{
		GameDataManager.Instance.SaveGame();
		gameObject.SetActive(false);
	}

	public void OnColorCategoryChanged(bool primary)
	{
		if (primary)
		{
			Debug.Log("Switched to PRIMARY color editing.");
			primaryColorSelected = true;

			SetSlidersInteractable(true);
			ReloadSliders(_primaryColor);
		}
		else
		{
			Debug.Log("Switched to SECONDARY color editing.");
			primaryColorSelected = false;

			SetSlidersInteractable(!sameAsPrimaryToggle.isOn);
			ReloadSliders(sameAsPrimaryToggle.isOn ? _primaryColor : _secondaryColor);
		}
	}
	#endregion

	private void ReloadSliders(Color color)
	{
		redSlider.value = color.r;
		greenSlider.value = color.g;
		blueSlider.value = color.b;
	}

	private void ReloadUI()
	{
		sameAsPrimaryToggle.isOn = UserSettings.SecondaryColorSameAsPrimary;

		secondaryPreview.color = sameAsPrimaryToggle.isOn ? _primaryColor : _secondaryColor;
	}

	private void SetSlidersInteractable(bool isActive)
	{
		redSlider.interactable = isActive;
		greenSlider.interactable = isActive;
		blueSlider.interactable = isActive;
	}
}
