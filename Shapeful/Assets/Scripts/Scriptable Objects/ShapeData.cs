using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shape Data", menuName = "Shape Data")]
public class ShapeData : ScriptableObject
{
	[Header("Vertices"), Space]
	[Range(3, 8)] public int vertexCount;
	[ReadOnly] public Vector3[] _vertices;

	[Header("Properties"), Space]
	public bool canSpin;
	[Tooltip("The range of the spinning speed.")]
	public Vector2 spinSpeed;

	[Space]
	[Min(1f)] public float shrinkSpeed;
	public float initalScale;

	[ContextMenu("Initialize Vertex Positions")]
	private void InitializeVertices()
	{
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
