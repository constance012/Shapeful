using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using CSTGames.DataPersistence;

public class IconCustomizeMenu : AppearanceMenu, ISaveDataTransceiver
{
	[Header("Icon Slots"), Space]
	[SerializeField, Tooltip("All the selectable UI slots, must be correctly ordered by their transform index.")]
	private SelectableUISlot[] slots;

	[Header("Available Icons"), Space]
	[SerializeField] private PlayerIcon[] playerIcons;
	[SerializeField, Tooltip("A default icon to use if the loaded data is null.")]
	public Sprite defaultSprite;

	[Header("UI References"), Space]
	[SerializeField] private TextMeshProUGUI pageNumber;
	[SerializeField] private Button nextPageButton;
	[SerializeField] private Button previousPageButton;

	// Private fields.
	private int _maxPage;
	private uint _currentPage = 1;

	private int _startIndex = 0;
	private PlayerIcon _selectedIcon;

	#region Interface Methods
	public void SaveData(GameData data)
	{
		data.SetPlayerIconTextures(_selectedIcon);

		data.iconLockStates ??= new bool[playerIcons.Length];
		
		for (int i = 0; i < playerIcons.Length; i++)
		{
			data.iconLockStates[i] = playerIcons[i].isLocked;
		}
	}

	public void LoadData(GameData data)
	{
		_selectedIcon = new PlayerIcon(data.playerIconData, defaultSprite);

		_primaryPreview.sprite = _selectedIcon.primary;
		_secondaryPreview.sprite = _selectedIcon.secondary;

		InitializePageNaviation(data.iconLockStates);
		ReloadUI();
	}
	#endregion

	#region Callback Methods for UI Events.
	public void SelectIcon(SelectableUISlot chosenSlot)
	{
		// TO-DO: Unlock icons by spending some type of currency.
		if (chosenSlot.HasUnlocked || chosenSlot.TryUnlockIcon())
		{
			// Deselect all slots.
			Array.ForEach(slots, (slot) => slot.SetAsSelected(false));
			
			// Unlock this icon before selecing it.
			_selectedIcon = chosenSlot.Icon;
			playerIcons[chosenSlot.IconIndex].isLocked = false;

			// Select the chosen one.
			chosenSlot.SetAsSelected(true);

			_primaryPreview.sprite = _selectedIcon.primary;
			_secondaryPreview.sprite = _selectedIcon.secondary;
		}
	}

	public void ToNextPage()
	{
		if (_currentPage < _maxPage)
		{
			_currentPage++;
			_startIndex += 6;

			nextPageButton.interactable = !(_currentPage == _maxPage);
			previousPageButton.interactable = true;

			ReloadUI();
		}
	}

	public void ToPreviousPage()
	{
		if (_currentPage > 1)
		{
			_currentPage--;
			_startIndex -= 6;

			previousPageButton.interactable = !(_currentPage == 1);
			nextPageButton.interactable = true;

			ReloadUI();
		}
	}
	#endregion

	protected override void ReloadUI()
	{
		// Clear all the slot first.
		Array.ForEach(slots, (slot) => slot.ClearSprites());

		// Then re-populate them with the current page content.
		int remainingLength = playerIcons.Length - _startIndex;
		int pageItemCount = (remainingLength / 6 > 0) ? 6 : remainingLength % 6;
		
		for (int i = 0; i < pageItemCount; i++)
		{
			slots[i].AddIcon(playerIcons[_startIndex + i]);
			
			// Selecting this slot if it holds the currently selected sprite.
			slots[i].SetAsSelected(_selectedIcon.index);
		}

		// Finally, set the page number.
		pageNumber.text = $"{_currentPage} / {_maxPage}";
	}

	private void InitializePageNaviation(bool[] iconLockStates)
	{
		_maxPage = playerIcons.Length / 6 + 1;

		// Assign indexes corresponding to each icon's position and their lock states.
		for (int i = 0; i < playerIcons.Length; i++)
		{
			playerIcons[i].index = i;

			// Update the default lock state only if the loaded array is not null.
			if (iconLockStates != null)
				playerIcons[i].isLocked = iconLockStates[i];
		}

		// Navigate to the page in which the previously selected icon is located.
		_currentPage = (uint)(_selectedIcon.index / 6 + 1);
		_startIndex = ((int)_currentPage - 1) * 6;

		nextPageButton.interactable = !(_currentPage == _maxPage);
		previousPageButton.interactable = !(_currentPage == 1);
	}

	[Serializable]
	public struct PlayerIcon
	{
		[Header("Sprites")]
		[HideInInspector] public int index;
		public string iconName;
		public Sprite primary;
		public Sprite secondary;

		[Header("Unlocking criteria")]
		public bool isLocked;
		public int unlockCost;

		public PlayerIcon(PlayerIconData iconData, Sprite defaultSprite)
		{
			if (iconData != null)
			{
				this.index = iconData.index;
				this.iconName = iconData.iconName;
				this.primary = iconData.ReconstructPrimarySprite();
				this.secondary = iconData.ReconstructSecondarySprite();
			}
			else
			{
				this.index = -1;
				this.iconName = "default_icon";
				this.primary = defaultSprite;
				this.secondary = defaultSprite;
			}

			this.isLocked = false;
			this.unlockCost = 0;
		}
	}
}