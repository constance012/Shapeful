using UnityEngine;

public class ShapeSpawner : Singleton<ShapeSpawner>
{
	[Header("Prefab"), Space]
	[SerializeField] private GameObject shapePrefab;

	[Header("Shapes to spawn"), Space]
	[SerializeField] private ShapeData[] shapesData;

	[Header("Properties"), Space]
	public float spawnDelay = 1f;

	private float _delay;

	private void Update()
	{
		_delay -= Time.deltaTime;

		if (_delay <= 0f)
		{
			SpawnShape();
			_delay = spawnDelay;
		}
	}

	private void SpawnShape()
	{
		int randomIndex = Random.Range(0, shapesData.Length);
		ShapeData chosenShape = shapesData[randomIndex];

		GameObject spawnedShape = Instantiate(shapePrefab, Vector3.zero, Quaternion.identity);
		spawnedShape.GetComponent<ShapeMono>().data = chosenShape;
	}
}
