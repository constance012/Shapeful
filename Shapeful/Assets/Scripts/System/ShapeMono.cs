using System.Collections.Generic;
using UnityEngine;

public class ShapeMono : MonoBehaviour
{
	[ReadOnly] public ShapeData shapeData;

	[Header("References"), Space]
	[SerializeField] private Rigidbody2D _rb2D;
	[SerializeField] private LineRenderer _lineRenderer;

	[Space]
	[SerializeField] private EdgeCollider2D _edgeCollider;
	[SerializeField] private EdgeCollider2D _scoreTrigger;

	public static float SpinSpeedMultiplier { get; set; }

	// Private fields.
	private GameObject _collectable;
	private float _spinSpeed;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ReloadStaticFields()
	{
		SpinSpeedMultiplier = 1f;
	}

	private void Start()
	{
		// Apply random rotation and initial scale.
		_rb2D.rotation = Random.Range(-180f, 180f);
		transform.localScale = Vector3.one * shapeData.initialScale;

		_spinSpeed = Random.Range(shapeData.spinSpeed.x, shapeData.spinSpeed.y) * Mathf.Sign(Mathf.Sin(Time.time));
	}

	private void Update()
	{
		transform.localScale -= Vector3.one * shapeData.shrinkSpeed * Time.deltaTime;

		if (_collectable != null)
		{
			_collectable.transform.localScale = Vector3.one * .7f / this.transform.localScale.x;
			_collectable.transform.rotation = Quaternion.identity;
		}

		if (transform.localScale.x < .1f)
			Destroy(gameObject);
	}

	private void FixedUpdate()
	{
		if (shapeData.canSpin)
		{
			transform.Rotate(transform.forward, _spinSpeed * SpinSpeedMultiplier * Time.deltaTime);
		}
	}

	public void InitializeComponents(ShapeData data, Collectable collectable = null)
	{
		this.shapeData = data;

		int sideCount = data._vertices.Length;

		_lineRenderer.positionCount = sideCount;
		_lineRenderer.SetPositions(data._vertices);
		
		_lineRenderer.colorGradient = data.colorGradient;

		Vector2[] colliderPoints = new Vector2[sideCount];

		for (int i = 0; i < sideCount; i++)
		{
			colliderPoints[i] = new Vector2(data._vertices[i].x, data._vertices[i].y);
		}

		_edgeCollider.points = colliderPoints;

		Vector2[] scorePoints = _scoreTrigger.points;
		scorePoints[0] = data._vertices[0];
		scorePoints[1] = data._vertices[sideCount - 1];

		_scoreTrigger.points = scorePoints;

		if (collectable != null)
		{
			_collectable = Instantiate(collectable.gameObject, this.transform);
			_collectable.transform.localPosition = shapeData.CollectablePosition;
			_collectable.transform.localScale = Vector3.one / this.transform.localScale.x;
		}
	}
}
