using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpIndicator : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private Image timer;
	[SerializeField] private Image icon;
	[SerializeField] private TextMeshProUGUI durationText;

	[Header("Tooltip Trigger"), Space]
	[SerializeField] private TooltipTrigger tooltip;

	// Private fields.
	private float _baseDuration;

	public void Initialize(PowerUp powerUp)
	{
		this._baseDuration = powerUp.duration;

		timer.fillAmount = 1f;
		this.icon.sprite = powerUp.icon;
		durationText.text = _baseDuration.ToString();

		tooltip.header = powerUp.powerUpName;
		tooltip.content = powerUp.description;
	}

	public void UpdateUI(float currentDuration)
	{
		timer.fillAmount = currentDuration / _baseDuration;

		durationText.text = Mathf.FloorToInt(currentDuration + 1f).ToString("0");
	}
}
