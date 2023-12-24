using UnityEngine;

public abstract class PowerUp : ScriptableObject
{
	[Header("General Properties"), Space]
	public string powerUpName;
	[Min(1f)] public float duration;

	[Space] public Sprite icon;
	[TextArea(3, 10)] public string description;

	[Header("Limited Use Times"), Space]
	public bool hasUseTimes;
	[Min(1)] public uint maxUseTimes;

	// Properties.
	public PowerUpIndicator IndicatorUI
	{
		get { return _indicatorUI; }
		set
		{
			_indicatorUI = value;
			_indicatorUI.Initialize(this);

			_currentUseTimes = maxUseTimes;
		}
	}

	public bool ReadyToBeRemoved { get; private set; }

	// Protected fields.
	protected uint _currentUseTimes;

	// Private fields.
	private PowerUpIndicator _indicatorUI;

	public void UpdateDuration()
	{
		duration -= Time.deltaTime;

		_indicatorUI.UpdateDurationUI(duration);

		if (duration <= 0f)
		{
			Destroy(_indicatorUI.gameObject);
			ReadyToBeRemoved = true;
		}
	}

	public void UpdateUseTimes()
	{
		if (!hasUseTimes)
			return;

		if (_currentUseTimes > 1)
		{
			_currentUseTimes--;
			_indicatorUI.UpdateUseTimesUI(_currentUseTimes);
		}
		else
		{
			Destroy(_indicatorUI.gameObject);
			ReadyToBeRemoved = true;
		}
	}

	public void SetUseTimes(uint useTimes)
	{
		this.maxUseTimes = useTimes;
		_indicatorUI.UpdateUseTimesUI(useTimes);
	}

	// TODO: Add to the power up from the manager's list.
	// TODO: Apply the effect to the player.
	public abstract void ApplyEffect();

	// TODO: Remove the power up from the manager's list.
	// TODO: Reverse the effect to normal.
	public abstract void RemoveEffect();

	public bool CompareName(string target)
	{
		return powerUpName.Equals(target);
	}
}
