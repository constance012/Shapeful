using System;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Shape Data", menuName = "Shape Data")]
public class ShapeData : ScriptableObject
{
	[Header("Vertices"), Space]
	[ReadOnly] public int vertexCount;
	[ReadOnly] public Vector3[] _vertices;

	[Header("General Properties"), Space]
	public Gradient colorGradient;

	[Space]
	public bool canSpin;
	[Tooltip("The range of the spinning speed.")]
	public Vector2 spinSpeed;

	[Space, Min(1f)] public float shrinkSpeed;
	[Min(1f)] public float initialScale;

	[Header("Player Affect Properties"), Space]
	[Min(1)] public int scoreGain;
	[Min(1)] public int contactDamage;

	// Properties.
	public Vector3 GetCollectablePosition
	{
		get
		{
			int lastIndex = _vertices.Length - 1;
			return (_vertices[0] + _vertices[lastIndex]) / 2f;
		}
	}

	public float GetBasedShrinkTime => (initialScale - .1f) / shrinkSpeed;

	[ContextMenu("Initialize Vertex Positions")]
	public void InitializeVertices()
	{
		vertexCount = UnityRandom.Range(3, 7);

		float angleBetweenAdjacentVertices = 360f / vertexCount;

		_vertices = new Vector3[vertexCount];
		_vertices[0] = new Vector3(1f, 0f);

		for (int i = 1; i < vertexCount; i++)
		{
			float x = Mathf.Cos(angleBetweenAdjacentVertices * i * Mathf.Deg2Rad);
			float y = Mathf.Sin(angleBetweenAdjacentVertices * i * Mathf.Deg2Rad);

			float roundedX = (float)Math.Round(x, 3);
			float roundedY = (float)Math.Round(y, 3);

			_vertices[i] = new Vector3(roundedX, roundedY);
		}
	}
}
