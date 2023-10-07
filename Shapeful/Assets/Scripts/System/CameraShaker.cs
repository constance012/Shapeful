using UnityEngine;
using Cinemachine;

public class CameraShaker : Singleton<CameraShaker>
{
	[Header("References"), Space]
	[SerializeField] private CinemachineVirtualCamera virtualCamera;

	[Header("Global Shake Settings"), Space]
	[SerializeField] private float globalInitialAmplitude;
	[SerializeField] private float globalShakeDuration;

	// Private fields.
	private CinemachineBasicMultiChannelPerlin _shaker;
	private float _remainingTime;

	private float _localInitialAmplitude;
	private float _localShakeDuration;
	private bool _isLocalShake;

	protected override void Awake()
	{
		base.Awake();

		_shaker = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
	}

	private void Update()
	{
		if (_remainingTime > 0f)
		{
			if (!_isLocalShake)
				_shaker.m_AmplitudeGain = Mathf.Lerp(globalInitialAmplitude, 0f, 1f - (_remainingTime / globalShakeDuration));
			else
				_shaker.m_AmplitudeGain = Mathf.Lerp(_localInitialAmplitude, 0f, 1f - (_remainingTime / _localShakeDuration));

			_remainingTime -= Time.deltaTime;
		}
	}

	/// <summary>
	/// Shakes the camera using the global shake settings.
	/// </summary>
	public void ShakeCamera()
	{
		_shaker.m_AmplitudeGain = globalInitialAmplitude;
		_remainingTime = globalShakeDuration;
		_isLocalShake = false;
	}

	/// <summary>
	/// Shakes the camera with the specified amplitude and duration.
	/// </summary>
	/// <param name="amplitude"></param>
	/// <param name="duration"></param>
	public void ShakeCamera(float amplitude, float duration)
	{
		_localInitialAmplitude = amplitude;
		_localShakeDuration = duration;

		_shaker.m_AmplitudeGain = amplitude;
		_remainingTime = duration;
		_isLocalShake = true;
	}
}
