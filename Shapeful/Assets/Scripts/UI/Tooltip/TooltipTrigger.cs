using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerDownHandler
{
	[Header("Texts"), Space]
	public string header;
	[Space, TextArea(5, 10)] public string content;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (TooltipHandler.Instance.IsShowed)
		{
			TooltipHandler.Instance.Hide();
			return;
		}

		if (!string.IsNullOrEmpty(header) || !string.IsNullOrEmpty(content))
			TooltipHandler.Instance.Show(transform.position, content, header);
	}

	private void OnMouseDown()
	{
		if (TooltipHandler.Instance.IsShowed)
		{
			TooltipHandler.Instance.Hide();
			return;
		}

		if (!string.IsNullOrEmpty(header) || !string.IsNullOrEmpty(content))
			TooltipHandler.Instance.Show(transform.position, content, header);
	}

	private void OnDestroy()
	{
		TooltipHandler.Instance.Hide();
	}
	
	private void OnDisable()
	{
		TooltipHandler.Instance.Hide();
	}
}
