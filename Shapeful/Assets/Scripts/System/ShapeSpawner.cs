using UnityEngine;

public class ShapeSpawner : Singleton<ShapeSpawner>
{
	[Header("Prefab"), Space]
	[SerializeField] private GameObject shapePrefab;

	[Header("Shapes Data"), Space]
	[SerializeField] private ShapeData normalShape;
	[SerializeField] private ShapeData[] specialShapes;

	[Header("Collectibles"), Space]
	[SerializeField] private Collectible[] collectibles;
	[SerializeField, Range(0f, 1f)] private float collectibleSpawnChance;

	[Header("Timer"), Space]
	[SerializeField] private float spawnDelay = 1f;

	[Header("Special Shape Spawning Condition"), Space]
	[Tooltip("The chance to accumulate the spawning meter starts at 0when the player scores points." +
		"A special shape will spawn once this meter reaches 100."), SerializeField, Range(0f, 1f)]
	private float accumulateTriggerChance;

	[Tooltip("The random range of accumulation amount that the meter will gain once the trigger condition is met."), SerializeField]
	private Vector2Int accumulateGainRange;

	// Private fields.
	private ShapeData _nextShape;
	private float _delay;
	private int _specialSpawningMeter = 0;
	private bool _specialShapeNext;

	private void Start()
	{
		_nextShape = Instantiate(normalShape);
	}

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
		_nextShape.InitializeVertices();

		GameObject spawnedShape = Instantiate(shapePrefab, Vector3.zero, Quaternion.identity);

		Collectible collectible = CheckCollectibleSpawn();
		spawnedShape.GetComponentInChildren<ShapeMono>().InitializeComponents(_nextShape, collectible);

		AccumulateSpawningMeter();
	}

	private Collectible CheckCollectibleSpawn()
	{
		if (Random.value <= collectibleSpawnChance)
		{
			int randomIndex = Random.Range(0, collectibles.Length);
			return collectibles[randomIndex];
		}

		return null;
	}

	private void AccumulateSpawningMeter()
	{
		if (_specialShapeNext)
		{
			_nextShape = Instantiate(normalShape);
			_specialShapeNext = false;
			return;
		}

		if (Random.value <= accumulateTriggerChance)
		{
			_specialSpawningMeter += Random.Range(accumulateGainRange.x, accumulateGainRange.y);

			if (_specialSpawningMeter >= 100)
			{
				int randomIndex = Random.Range(0, specialShapes.Length);
				ShapeData specialShape = specialShapes[randomIndex];

				_nextShape = Instantiate(specialShape);
				
				_specialSpawningMeter = 0;
				_specialShapeNext = true;
			}
		}
	}
}
