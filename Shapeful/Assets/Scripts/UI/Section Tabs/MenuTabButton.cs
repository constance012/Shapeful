using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Graphic))]
public class MenuTabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	[Header("Current Tab Group"), Space]
	[SerializeField] private TabGroup tabGroup;
	[SerializeField] private bool isStartupTab;

	[Header("Content Page"), Space]
	public GameObject contentPage;

	[Header("Effect"), Space]
	[SerializeField] private float colorFadeTime;

	// Private fields.
	private Graphic _graphic;
	private Coroutine _coroutine;

	private void Awake()
	{
		tabGroup = GetComponentInParent<TabGroup>();
		_graphic = GetComponent<Graphic>();
		
		tabGroup.Subscribe(this);
	}

	private void Start()
	{
		if (isStartupTab)
			tabGroup.OnTabSelected(this);
	}

	#region Interface Methods.
	public void OnPointerEnter(PointerEventData eventData)
	{
		tabGroup.OnTabEnter(this);
	}
	
	public void OnPointerExit(PointerEventData eventData)
	{
		tabGroup.OnTabExit(this);
	}
	
	public void OnPointerClick(PointerEventData eventData)
	{
		tabGroup.OnTabSelected(this);
	}
	#endregion

	public void SetContentState(bool state)
	{
		contentPage.SetActive(state);
	}

	public void SetTextColor(Color color)
	{
		if (_coroutine != null)
			StopCoroutine(_coroutine);

		_coroutine = StartCoroutine(ColorFading(color));
	}

	private IEnumerator ColorFading(Color targetColor)
	{
		Color oldColor = _graphic.color;
		float elapsedTime = 0f;

		while(elapsedTime < colorFadeTime)
		{
			_graphic.color = Color.Lerp(oldColor, targetColor, elapsedTime / colorFadeTime);
			elapsedTime += Time.deltaTime;

			yield return null;
		}

		_graphic.color = targetColor;
	}
}
