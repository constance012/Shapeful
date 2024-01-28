#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using CSTGames.DataPersistence;
using UnityObject = UnityEngine.Object;

public class IconCustomizeMenu : AppearanceMenu, ISaveDataTransceiver
{
	[Header("Icon Sprite Sheets References"), Space]
	[SerializeField] private Texture2D staticSheet;
	[SerializeField] private Texture2D primarySheet;
	[SerializeField] private Texture2D secondarySheet;

	[Header("Icon Slots"), Space]
	[SerializeField, Tooltip("All the selectable UI slots, must be correctly ordered by their transform index.")]
	private SelectableUISlot[] slots;

	[Header("Available Icons"), Space]
	[SerializeField] private PlayerIcon[] playerIcons;

	[Header("UI References"), Space]
	[SerializeField] private TextMeshProUGUI pageNumber;
	[SerializeField] private Button nextPageButton;
	[SerializeField] private Button previousPageButton;

	// Private fields.
	private int _maxPage;
	private uint _currentPage = 1;

	private int _startIndex = 0;
	private PlayerIcon _selectedIcon;
	private SelectableUISlot _lockedSlot;

	#region Interface Methods
	public bool Ready => _primaryPreview != null;

	public void SaveData(GameData data)
	{
		data.gemShards = GemShards;
		data.playerIconData = new PlayerIconData(_selectedIcon);

		data.iconLockStates ??= new bool[playerIcons.Length];
		
		for (int i = 0; i < playerIcons.Length; i++)
		{
			data.iconLockStates[i] = playerIcons[i].isLocked;
		}
	}

	public void LoadData(GameData data)
	{
		GemShards = data.gemShards;
		_gemShardsDisplayText.text = GemShards.ToString();

		// Reconstruct the saved data if it's not null.
		// Otherwise, default to the first icon in the array.
		_selectedIcon = new PlayerIcon(data.playerIconData, playerIcons[0]);

		_primaryPreview.sprite = _selectedIcon.primarySprite;
		_secondaryPreview.sprite = _selectedIcon.secondarySprite;
		_staticPreview.sprite = _selectedIcon.staticSprite;

		InitializePageNaviation(data.iconLockStates);
		ReloadUI();
	}
	#endregion

	#region Callback Methods for UI Events.
	public void SelectIcon(SelectableUISlot chosenSlot)
	{
		if (chosenSlot.HasUnlocked)
		{
			// Deselect all slots.
			Array.ForEach(slots, (slot) => slot.SetAsSelected(false));
			
			// Unlock this icon before selecing it.
			_selectedIcon = chosenSlot.Icon;

			// Select the chosen one.
			chosenSlot.SetAsSelected(true);

			_primaryPreview.sprite = _selectedIcon.primarySprite;
			_secondaryPreview.sprite = _selectedIcon.secondarySprite;
			_staticPreview.sprite = _selectedIcon.staticSprite;
		}
		else
		{
			_lockedSlot = chosenSlot;

			Color iconTextColor = new Color(.12f, .35f, .6f);
			Color gemTextColor = new Color(.61f, .12f, .58f);
			string infoText;

			if (GemShards >= _lockedSlot.UnlockCost)
			{
				int cost = playerIcons[_lockedSlot.IconIndex].unlockCost;
				infoText = $"Unlock this<color=#{ColorUtility.ToHtmlStringRGB(iconTextColor)}> icon </color>" +
						   $"with<color=#{ColorUtility.ToHtmlStringRGB(gemTextColor)}> {cost} </color> gem shards?";

				_confirmBox.Setup(infoText, _lockedSlot.Icon, ConfirmationBox.ButtonSet.OK_Cancel);
				_confirmBox.ToggleActiveState(true, UnlockIcon);
			}
			else
			{
				infoText = $"You don't have enough" +
						   $"<color=#{ColorUtility.ToHtmlStringRGB(gemTextColor)}> gem shards </color> to unlock this" +
						   $"<color=#{ColorUtility.ToHtmlStringRGB(iconTextColor)}> icon.</color>";

				_confirmBox.Setup(infoText, ConfirmationBox.ButtonSet.OK);
				_confirmBox.ToggleActiveState(true);
			}
		}
	}

	public void UnlockIcon()
	{
		_lockedSlot.Unlock();
		playerIcons[_lockedSlot.IconIndex].isLocked = false;

		GemShards -= _lockedSlot.UnlockCost;
		_gemShardsDisplayText.text = GemShards.ToString();

		SelectIcon(_lockedSlot);
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

#if UNITY_EDITOR
	[ContextMenu("Reference All Sprites")]
	private void ReferenceAllSprites()
	{
		if (staticSheet == null || primarySheet == null || secondarySheet == null)
		{
			Debug.LogWarning("Reference FAILED: Some of the icon sprite sheets are not assigned!");
			return;
		}

		UnityObject[] staticSprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(staticSheet));
		UnityObject[] primarySprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(primarySheet));
		UnityObject[] secondarySprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(secondarySheet));

		// Minus 1 because we ignore the main Texture2D.
		int total = primarySprites.Length - 1;
		Debug.Log($"Loaded {total} total icons.");

		if (playerIcons.Length < total)
		{
			Debug.LogWarning("Reference FAILED: The icon list has fewer elements than the total icons to be able to load!");
			return;
		}

		for (int i = 0; i < total; i++)
		{
			playerIcons[i].staticSprite = (Sprite)staticSprites[i + 2];
			playerIcons[i].primarySprite = (Sprite)primarySprites[i + 1];
			playerIcons[i].secondarySprite = (Sprite)secondarySprites[i + 1];
		}

		Debug.Log($"SUCCESSFULLY referenced {total} / {playerIcons.Length} icons.");
	}

	[ContextMenu("Lock All Sprites")]
	private void LockAllSprites()
	{
		GameDataManager manager = FindObjectOfType<GameDataManager>();

		if (manager == null)
		{
			Debug.LogWarning("Game Data Manager object was NOT found! Can not access the save file.");
			return;
		}

		SaveFileHandler<GameData> handler = manager.DisposableHandler;
		GameData data = handler.LoadDataFromFile();
		
		for (int i = 0; i < data.iconLockStates.Length; i++)
		{
			data.iconLockStates[i] = i != 0;
			playerIcons[i].isLocked = i != 0;
		}

		handler.SaveDataToFile(data);

		Debug.LogWarning("Save file ALTERED! All icons have been locked, except the first one.");
	}
#endif

	[Serializable]
	public struct PlayerIcon
	{
		[Header("Sprites")]
		[HideInInspector] public int index;
		public string iconName;
		public Sprite staticSprite;
		public Sprite primarySprite;
		public Sprite secondarySprite;

		[Header("Unlocking criteria")]
		public bool isLocked;
		public int unlockCost;

		public PlayerIcon(PlayerIconData iconData, PlayerIcon defaultIcon)
		{
			if (!iconData.IsDefault)
			{
				this.index = iconData.index;
				this.iconName = iconData.iconName;
				
				this.staticSprite = iconData.ReconstructSprite(PlayerSpriteLayer.Static);
				this.primarySprite = iconData.ReconstructSprite(PlayerSpriteLayer.Primary);
				this.secondarySprite = iconData.ReconstructSprite(PlayerSpriteLayer.Secondary);
			}
			else
			{
				this = defaultIcon;
			}

			this.isLocked = false;
			this.unlockCost = 0;
		}
	}
}