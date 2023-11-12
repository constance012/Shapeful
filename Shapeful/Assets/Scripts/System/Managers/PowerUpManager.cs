using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : Singleton<PowerUpManager>
{
	[SerializeField, ReadOnly] private List<PowerUp> powerUps;

	[Header("References"), Space]
	[SerializeField] private Transform powerUpsPanel;
	[SerializeField] private GameObject uiIndicatorPrefab;

	public bool IsLimitReached => powerUpsPanel.transform.childCount > 5;
	public bool AnyActivePowerUps => powerUps.Count > 0;

	private void Update()
	{
		if (!AnyActivePowerUps)
		{
			return;
		}

		// Remove any expired power ups first.
		powerUps.RemoveAll(TryRemoveExpired);

		foreach(PowerUp powerUp in powerUps)
		{
			powerUp.UpdateDuration();
		}
	}

	/// <summary>
	/// Tries applying this power-up to the player.
	/// </summary>
	/// <param name="powerUp"></param>
	/// <returns> <b>True</b> if the player doesn't currently have the same power-up applied, <b>false</b> otherwise. </returns>
	public bool TryApply(PowerUp powerUp)
	{
		Debug.Log($"Attempting to apply {powerUp.powerUpName} to the player.");

		if (!HasAny(powerUp.powerUpName))
		{
			powerUp.IndicatorUI = Instantiate(uiIndicatorPrefab, powerUpsPanel).GetComponent<PowerUpIndicator>();

			powerUps.Add(powerUp);
			return true;
		}
		else
		{
			// Reset the duration if this same power-up is already existed.
			PowerUp existing = Get(powerUp.powerUpName);
			existing.duration = powerUp.duration;
			
			return false;
		}
	}

	public bool HasAny(string targetName)
	{
		return powerUps.Find(powerUp => powerUp.CompareName(targetName)) != null;
	}

	public PowerUp Get(string targetName)
	{
		return powerUps.Find(powerUp => powerUp.CompareName(targetName));
	}

	private bool TryRemoveExpired(PowerUp powerUp)
	{
		if (powerUp.DurationExpired)
		{
			Debug.Log($"Duration expired, removing {powerUp.powerUpName}.");
			
			powerUp.RemoveEffect();
			return true;
		}

		return false;
	}
}
