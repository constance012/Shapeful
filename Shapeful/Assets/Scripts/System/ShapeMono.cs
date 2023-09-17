using System.Collections.Generic;
using UnityEngine;

public class ShapeMono : MonoBehaviour
{
	[ReadOnly] public ShapeData data;

	[Header("References"), Space]
	[SerializeField] private Rigidbody2D _rb2D;
	[SerializeField] private LineRenderer _lineRenderer;

	[Space]
	[SerializeField] private EdgeCollider2D _edgeCollider;
	[SerializeField] private EdgeCollider2D _scoreTrigger;

	private float _spinSpeed;

	private void Start()
	{
		InitializeComponents();

		// Apply random rotation and initial scale.
		_rb2D.rotation = Random.Range(-180f, 180f);
		transform.localScale = Vector3.one * data.initalScale;

		_spinSpeed = Random.Range(data.spinSpeed.x, data.spinSpeed.y) * Mathf.Sign(Mathf.Sin(Time.time));
	}

	private void Update()
	{
		transform.localScale -= Vector3.one * data.shrinkSpeed * Time.deltaTime;			

		if (transform.localScale.x < .1f)
			Destroy(gameObject);
	}

	private void FixedUpdate()
	{
		if (data.canSpin)
		{
			transform.Rotate(transform.forward, _spinSpeed * Time.deltaTime);
		}
	}

	private void InitializeComponents()
	{
		int sideCount = data._vertices.Length;

		_lineRenderer.positionCount = sideCount;
		_lineRenderer.SetPositions(data._vertices);

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
	}
}
