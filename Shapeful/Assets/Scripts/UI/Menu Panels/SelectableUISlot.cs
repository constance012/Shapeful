using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static IconCustomizeMenu;

public class SelectableUISlot : Button
{
	[Header("References"), Space]
	[SerializeField] private Image staticSprite;
	[SerializeField] private Image primarySprite;
	[SerializeField] private Image secondarySprite;

	[SerializeField] private GameObject selectedBorder;

	[SerializeField] private GameObject lockOverlay;
	[SerializeField] private TextMeshProUGUI unlockText;

	public PlayerIcon Icon => _displayIcon;
	public int IconIndex => _displayIcon.index;
	public bool HasUnlocked => !_displayIcon.isLocked;
	public int UnlockCost { get; private set; }

	// Private fields.
	private PlayerIcon _displayIcon;

	public void ClearSprites()
	{
		primarySprite.sprite = null;
		secondarySprite.sprite = null;
		staticSprite.sprite = null;

		gameObject.SetActive(false);
	}

	public void AddIcon(PlayerIcon icon)
	{
		_displayIcon = icon;

		primarySprite.sprite = _displayIcon.primarySprite;
		secondarySprite.sprite = _displayIcon.secondarySprite;
		staticSprite.sprite = _displayIcon.staticSprite;

		unlockText.text = $"{_displayIcon.unlockCost}";
		UnlockCost = _displayIcon.unlockCost;
		lockOverlay.SetActive(_displayIcon.isLocked);

		gameObject.SetActive(true);
	}

	public void SetAsSelected(bool isSelect)
	{
		selectedBorder.SetActive(isSelect);
	}

	public void SetAsSelected(int iconIndex)
	{
		selectedBorder.SetActive(IconIndex == iconIndex);
	}

	public void Unlock()
	{
		lockOverlay.SetActive(false);
		_displayIcon.isLocked = false;
	}
}
