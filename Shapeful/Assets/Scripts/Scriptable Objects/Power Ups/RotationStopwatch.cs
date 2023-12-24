using UnityEngine;

[CreateAssetMenu(fileName = "New Power-Up", menuName = "Power-Ups/Stopwatch")]
public class RotationStopwatch : PowerUp
{
	[Header("Timer"), Space]
	[Min(1f)] public float stopTimer;
	[Min(1f)] public float resumeTimer;

	public override void ApplyEffect()
	{
		ShapeSpawner.Instance.ControlShapesSpinningSpeed(true, stopTimer);

		AudioManager.Instance.PlayWithRandomPitch("Stopwatch Activate", .8f, 1.2f);
		CameraShaker.Instance.ShakeCamera(.8f, .5f);
	}

	public override void RemoveEffect()
	{
		ShapeSpawner.Instance.ControlShapesSpinningSpeed(false, resumeTimer);
	}
}
