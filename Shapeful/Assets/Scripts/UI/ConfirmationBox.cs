using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static IconCustomizeMenu;

public class ConfirmationBox : MonoBehaviour
{
	public enum ButtonSet { Confirm_Cancel, OK_Cancel, OK, Cancel }

	[Header("General References"), Space]
	[SerializeField] private TextMeshProUGUI infoText;
	[SerializeField] private GameObject confirmButtonGameObject;
	[SerializeField] private GameObject cancelButtonGameObject;
	[SerializeField] private GameObject okButtonGameObject;

	[Header("Icon Preview References"), Space]
	[SerializeField] private GameObject iconPreviewer;

	// Private fields.
	private Action _onConfirm;
	private Button _okButton;

	private Image _staticPreview;
	private Image _primaryPreview;
	private Image _secondaryPreview;

	private void Awake()
	{
		if (iconPreviewer != null)
		{
			_okButton = okButtonGameObject.GetComponent<Button>();

			_staticPreview = iconPreviewer.transform.GetComponentInChildren<Image>("Static");
			_primaryPreview = iconPreviewer.transform.GetComponentInChildren<Image>("Primary");
			_secondaryPreview = iconPreviewer.transform.GetComponentInChildren<Image>("Secondary");
		}

		gameObject.SetActive(false);
	}

	private void OnDisable()
	{
		_okButton.onClick.RemoveAllListeners();
	}

	#region Callback Methods for UI Events.
	public void ConfirmAction()
	{
		if (_onConfirm != null)
			_onConfirm();

		CancelAction();
	}

	public void CancelAction()
	{
		_onConfirm = null;
		gameObject.SetActive(false);
	}
	#endregion

	public void ToggleActiveState(bool active, Action onConfirmed = null)
	{
		gameObject.SetActive(active);
		
		if (onConfirmed != null)
			_onConfirm = onConfirmed;
	}

	public void Setup(string info, ButtonSet layout)
	{
		infoText.text = info;

		iconPreviewer.SetActive(false);
		ConfigButtonLayout(layout);
	}

	public void Setup(string info, PlayerIcon icon, ButtonSet layout)
	{
		infoText.text = info;

		ShowPreviewIcon(icon);
		ConfigButtonLayout(layout);
	}

	private void ConfigButtonLayout(ButtonSet layout)
	{
		confirmButtonGameObject.SetActive(false);
		cancelButtonGameObject.SetActive(false);
		okButtonGameObject.SetActive(false);

		switch (layout)
		{
			case ButtonSet.Confirm_Cancel:
				confirmButtonGameObject.SetActive(true);
				cancelButtonGameObject.SetActive(true);
				break;

			case ButtonSet.OK_Cancel:
				okButtonGameObject.SetActive(true);
				_okButton.onClick.AddListener(ConfirmAction);

				cancelButtonGameObject.SetActive(true);
				break;

			case ButtonSet.OK:
				okButtonGameObject.SetActive(true);
				_okButton.onClick.AddListener(CancelAction);
				break;

			case ButtonSet.Cancel:
				cancelButtonGameObject.SetActive(true);
				break;
		}
	}

	private void ShowPreviewIcon(PlayerIcon icon)
	{
		iconPreviewer.SetActive(true);

		_staticPreview.sprite = icon.staticSprite;
		_primaryPreview.sprite = icon.primarySprite;
		_secondaryPreview.sprite = icon.secondarySprite;
	}
}
