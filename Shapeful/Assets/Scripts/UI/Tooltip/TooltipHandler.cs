using UnityEngine;
using UnityEngine.UI;

public class TooltipHandler : Singleton<TooltipHandler>
{
	[Header("References"), Space]
	[SerializeField] private Tooltip tooltip;
	[SerializeField] private GraphicRaycaster raycaster;
	
	public bool IsShowed { get; private set; }

	public void Show(Vector3 triggerPosition, string contentText, string headerText = "")
	{
		if (!IsShowed)
		{
			tooltip.Initialize(triggerPosition, contentText, headerText);

			tooltip.gameObject.SetActive(true);
			raycaster.enabled = true;

			IsShowed = true;
		}
	}

	public void Hide()
	{
		if (IsShowed)
		{
			tooltip.gameObject.SetActive(false);
			raycaster.enabled = false;

			IsShowed = false;
		}
	}
}
