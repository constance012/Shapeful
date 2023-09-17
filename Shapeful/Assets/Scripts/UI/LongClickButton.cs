using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public UnityEvent onHold = new UnityEvent();

	private bool _touchDown;
	
	private void Update()
	{
		if (_touchDown && onHold != null)
		{
			onHold?.Invoke();
		}
	}
	public void OnPointerDown(PointerEventData eventData)
	{
		_touchDown = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_touchDown = false;
	}

}
