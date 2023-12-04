using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
	[Header("Section Tab Buttons"), Space]
	[SerializeField, ReadOnly] private List<MenuTabButton> tabButtons;

	// Private fields.
	private MenuTabButton _currentTab;

	private readonly Color _normalColor = new Color(.367f, .266f, .085f, .392f);
	private readonly Color _hoveringColor = new Color(.783f, .637f, .388f);
	private readonly Color _selectedColor = new Color(.764f, .475f, .169f);

	public void Subscribe(MenuTabButton button)
	{
		if (tabButtons == null)
			tabButtons = new List<MenuTabButton>();

		tabButtons.Add(button);
	}

	public void OnTabEnter(MenuTabButton target)
	{
		ResetTabs();

		// Change the color to hover if the tab is not selected.
		if(_currentTab == null || target != _currentTab)
			target.SetTextColor(_hoveringColor);
	}
	
	public void OnTabExit(MenuTabButton target)
	{
		ResetTabs();
	}

	public void OnTabSelected(MenuTabButton target)
	{
		_currentTab = target;

		ResetTabs();

		target.SetTextColor(_selectedColor);

		int currentIndex = target.transform.GetSiblingIndex();

		for (int i = 0; i < tabButtons.Count; i++)
		{
			tabButtons[i].SetContentState(i == currentIndex);
		}
	}

	private void ResetTabs()
	{
		// Reset all other tabs except the selected one.
		foreach (MenuTabButton tab in tabButtons)
		{
			if (_currentTab != null && tab == _currentTab)
				continue;

			tab.SetTextColor(_normalColor);
		}
	}
}
