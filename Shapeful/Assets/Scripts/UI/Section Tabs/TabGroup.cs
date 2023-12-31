using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
	[Header("Section Tab Buttons"), Space]
	[SerializeField, ReadOnly] private List<MenuTabButton> tabButtons;

	[Header("Color Tints"), Space]
	[SerializeField] private Color normalColor;
	[SerializeField] private Color hoveringColor;
	[SerializeField] private Color selectedColor;

	// Private fields.
	private MenuTabButton _selectedTab;

	public void Subscribe(MenuTabButton button)
	{
		tabButtons ??= new List<MenuTabButton>();

		tabButtons.Add(button);
		tabButtons.Sort(CompareTabButtons);
	}

	public void OnTabEnter(MenuTabButton target)
	{
		ResetTabColors();

		// Change the color to hover if the tab is not selected.
		if(_selectedTab == null || target != _selectedTab)
			target.SetGraphicColor(hoveringColor);
	}
	
	public void OnTabExit()
	{
		ResetTabColors();
	}

	public void OnTabSelected(MenuTabButton target)
	{
		_selectedTab = target;

		ResetTabColors();

		target.SetGraphicColor(selectedColor);

		int currentIndex = target.transform.GetSiblingIndex();

		for (int i = 0; i < tabButtons.Count; i++)
		{
			tabButtons[i].SetContentState(i == currentIndex);
		}
	}

	private void ResetTabColors()
	{
		// Reset all other tabs' colors to normal except the selected one.
		foreach (MenuTabButton tab in tabButtons)
		{
			if (_selectedTab != null && _selectedTab == tab)
				continue;

			tab.SetGraphicColor(normalColor);
		}
	}

	private int CompareTabButtons(MenuTabButton a, MenuTabButton b)
	{
		int indexA = a.transform.GetSiblingIndex();
		int indexB = b.transform.GetSiblingIndex();

		return indexA - indexB;
	}
}
