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

	// Private fields.
	private GameObject _collectible;
	private float _spinSpeed;

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

		if (_collectible != null)
		{
			_collectible.transform.localScale = Vector3.one * .7f / this.transform.localScale.x;
			_collectible.transform.rotation = Quaternion.identity;
		}

		if (transform.localScale.x < .1f)
			Destroy(gameObject);
	}

	private void FixedUpdate()
	{
		if (shapeData.canSpin)
		{
			transform.Rotate(transform.forward, _spinSpeed * Time.deltaTime);
		}
	}

	public void InitializeComponents(ShapeData data, Collectible collectible = null)
	{
		this.shapeData = data;

		int sideCount = data._vertices.Length;

		_lineRenderer.positionCount = sideCount;
		_lineRenderer.SetPositions(data._vertices);
		
		_lineRenderer.colorGradient = data.colorGradient;

		List<Vector2> colliderPoints = new List<Vector2>();

		for (int i = 0; i < sideCount; i++)
		{
			colliderPoints.Add(new Vector2(data._vertices[i].x, data._vertices[i].y));
		}

		_edgeCollider.SetPoints(colliderPoints);

		Vector2[] scorePoints = _scoreTrigger.points;
		scorePoints[0] = data._vertices[0];
		scorePoints[1] = data._vertices[sideCount - 1];

		_scoreTrigger.points = scorePoints;

		if (collectible != null)
		{
			_collectible = Instantiate(collectible.gameObject, this.transform);
			_collectible.transform.localPosition = shapeData.GetCollectiblePosition;
		}
	}
}
