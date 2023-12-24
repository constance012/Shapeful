using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PowerUpManager : Singleton<PowerUpManager>
{
	[SerializeField, ReadOnly] private List<PowerUp> powerUps;

	[Header("References"), Space]
	[SerializeField] private Transform powerUpsPanel;
	[SerializeField] private GameObject uiIndicatorPrefab;

	[Header("Events"), Space]
	public UnityEvent<PowerUp> onPowerUpRemoved = new UnityEvent<PowerUp>();

	public bool IsLimitReached => powerUpsPanel.transform.childCount > 5;
	public bool AnyActivePowerUps => powerUps.Count > 0;

	private void Update()
	{
		if (!AnyActivePowerUps)
		{
			return;
		}

		// Remove any expired power ups first.
		powerUps.RemoveAll(CheckRemovable);

		foreach(PowerUp powerUp in powerUps)
		{
			powerUp.UpdateDuration();
		}
	}

	/// <summary>
	/// Tries applying this power-up to the player.
	/// </summary>
	/// <param name="powerUp"></param>
	/// <returns> <b>True</b> if the power-up has been applied successfully, <b>False</b> otherwise. </returns>
	public bool TryApply(PowerUp powerUp)
	{
		if (powerUp == null)
			return false;

		Debug.Log($"Attempting to apply {powerUp.powerUpName} to the player.");

		if (!HasAny(powerUp.powerUpName, out PowerUp existing))
		{
			powerUp.IndicatorUI = Instantiate(uiIndicatorPrefab, powerUpsPanel).GetComponent<PowerUpIndicator>();

			powerUps.Add(powerUp);
			powerUp.ApplyEffect();
		}
		else
		{
			// Reset the duration and use times if this same power-up is already existed.
			existing.duration = powerUp.duration;
			existing.SetUseTimes(powerUp.maxUseTimes);
		}

		return true;
	}

	public bool HasAny(string targetName, out PowerUp target)
	{
		target = powerUps.Find(powerUp => powerUp.CompareName(targetName));
		return target != null;
	}

	public void DecreaseUseTimes(string powerUpName)
	{
		if (HasAny(powerUpName, out PowerUp target))
		{
			target.UpdateUseTimes();
		}
	}

	public void DecreaseUseTimes(IVisualPowerUp visualPowerUp)
	{
		(visualPowerUp as PowerUp).UpdateUseTimes();
	}

	public bool IsVisualPowerUp(string powerUpName, out IVisualPowerUp visualPowerUp)
	{
		HasAny(powerUpName, out PowerUp target);
		Debug.Log(target.GetType());
		Debug.Log(target as IVisualPowerUp == null);

		visualPowerUp = target as IVisualPowerUp;
		return visualPowerUp != null;
	}

	public bool IsVisualPowerUp(PowerUp target, out IVisualPowerUp visualPowerUp)
	{
		Debug.Log($"Currently checking if {target.GetType()} is a visual power-up.");

		visualPowerUp = target as IVisualPowerUp;
		return visualPowerUp != null;
	}

	public void ForceRemoveAll()
	{
		Debug.Log("Game over, removing all power-ups.");

		powerUps.RemoveAll((powerUp) =>
		{
			powerUp.RemoveEffect();
			return true;
		});
	}

	private bool CheckRemovable(PowerUp powerUp)
	{
		if (powerUp.ReadyToBeRemoved)
		{
			Debug.Log($"Ready to be removed, removing {powerUp.powerUpName}.");
			
			powerUp.RemoveEffect();
			onPowerUpRemoved?.Invoke(powerUp);

			return true;
		}

		return false;
	}
}
