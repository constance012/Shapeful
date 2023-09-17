using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
	[Header("Camera Customizations"), Space]
	[SerializeField, Tooltip("The rotation speed range.")]
	private Vector2 camRotateSpeed;

	// Private fields.
	private Transform _mainCam;
	private float _rotateSpeed;
	private bool _enableRotation;

	private void Start()
	{
		_mainCam = Camera.main.transform;
		_rotateSpeed = Random.Range(camRotateSpeed.x, camRotateSpeed.y) * Mathf.Sign(Mathf.Sin(Time.time));
	}

	private void Update()
	{
		if (_enableRotation)
			_mainCam.Rotate(Vector3.forward, _rotateSpeed * Time.deltaTime);
	}

	public void RandomRotationSpeed()
	{
		if (!_enableRotation)
			_enableRotation = true;

		_rotateSpeed = Random.Range(camRotateSpeed.x, camRotateSpeed.y);
	}
}
