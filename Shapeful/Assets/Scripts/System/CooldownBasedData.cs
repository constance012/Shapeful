using System;
using UnityEngine;
using CSTGames.DataPersistence;

public class CooldownBasedData
{
	public uint value;
	public uint maxValue;

	/// <summary>
	/// The base cooldown for a single value (READ-ONLY).
	/// </summary>
	public readonly TimeSpan baseCooldown;

	/// <summary>
	/// The total cooldown for all continue attemps (READ-ONLY).
	/// </summary>
	public TimeSpan TotalCooldown { get { return baseCooldown * maxValue; } }

	/// <summary>
	/// The remaining cooldown for the next continue attempt.
	/// </summary>
	private TimeSpan _remainingCD;

	#region Properties.
	public TimeSpan RemainingCD
	{
		get { return _remainingCD; }
		set
		{
			_remainingCD = value;
			CheckForCooldown();
		}
	}

	public TimeSpan NextValueRemainingCD
	{
		get
		{
			return _remainingCD > baseCooldown ?
				   _remainingCD - (baseCooldown * (maxValue - value - 1)):
				   _remainingCD;
		}
	}

	public bool IsOnCooldown => value < maxValue;
	#endregion

	public CooldownBasedData(uint maxValue, double cooldownInSeconds)
	{
		this.value = maxValue;
		this.maxValue = maxValue;

		baseCooldown = TimeSpan.FromSeconds(cooldownInSeconds);
		_remainingCD = TimeSpan.Zero;
	}

	public void UnaryDecrement()
	{
		value--;
		_remainingCD += baseCooldown;
	}

	public Vector3Int FromRemainingCD()
	{
		int totalHours = Mathf.FloorToInt((float)_remainingCD.TotalHours);
		return new Vector3Int(totalHours, _remainingCD.Minutes, _remainingCD.Seconds);
	}

	public void LoadValueSinceLastPlayed(GameData data)
	{
		TimeSpan previousRemainingCD = new TimeSpan(data.ContinueAttemptRemainingCD.x,
												 data.ContinueAttemptRemainingCD.y,
												 data.ContinueAttemptRemainingCD.z);

		TimeSpan timeSinceLastPlayed = DateTime.Now - DateTime.FromBinary(data.lastUpdated);

		if (timeSinceLastPlayed >= previousRemainingCD)
		{
			value = maxValue;
			_remainingCD = TimeSpan.Zero;
		}
		else
		{
			_remainingCD = previousRemainingCD - timeSinceLastPlayed;

			TimeSpan completedValueCD = TotalCooldown - _remainingCD;
			value = data.continueAttempts + (uint)Math.Floor(completedValueCD / baseCooldown);
		}
	}

	public void CheckForCooldown()
	{
		//Debug.Log(_remainingCD.ToString(@"hh\:mm\:ss"));

		if (_remainingCD <= TimeSpan.Zero)
		{
			value = maxValue;
			_remainingCD = TimeSpan.Zero;
		}
		else if (value < maxValue)
		{
			uint usedValueCount = maxValue - value;
			TimeSpan totalUsedValueCD = baseCooldown * usedValueCount;

			TimeSpan completedValueCD = totalUsedValueCD - _remainingCD;
				
			value += (uint)Math.Floor(completedValueCD / baseCooldown);
		}
	}
}
