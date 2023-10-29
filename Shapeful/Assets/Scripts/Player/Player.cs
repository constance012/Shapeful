using System.Collections;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
	private enum FlashType { Damage, Heal }

	[Header("References"), Space]
	[SerializeField] private GameObject damageTextPrefab;
	[SerializeField] private GameObject graphics;

	[Header("Move Speed"), Space]
	[SerializeField] private float moveSpeed;

	[Header("Health"), Space]
	public int maxHealth;
	[SerializeField, Min(1)] private int damageFlashCount;
	[SerializeField, Range(.1f, 1f)] private float invincibilityTime;

	[Header("Flash Colors"), Space]
	[SerializeField] private Color damageColor;
	[SerializeField] private Color healColor;

	// Private fields.
	private Material _mainMaterial;
	private ParticleSystem _deathEffect;
	
	private int _input;
	private int _currentHealth;
	private float _invincibilityTime;

	private void Awake()
	{
		_mainMaterial = this.GetComponentInChildren<SpriteRenderer>("Graphics/Main").material;
		_deathEffect = this.GetComponentInChildren<ParticleSystem>("Death Effect");
	}

	private void Start()
	{
		_currentHealth = maxHealth;
		GameManager.Instance.UpdatePlayerHealth(maxHealth, _currentHealth);
	}

	private void Update()
	{
		if (_invincibilityTime > 0f)
			_invincibilityTime -= Time.deltaTime;

#if UNITY_EDITOR
		if (Input.GetKey(KeyCode.A))
			RotateLeft();
		if (Input.GetKey(KeyCode.D))
			RotateRight();
#endif
	}

	private void FixedUpdate()
	{
		float angle = _input * moveSpeed * Time.deltaTime;
		transform.RotateAround(Vector3.zero, Vector3.forward, angle);

		_input = 0;
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (GameManager.Instance.GameOver || collider.CompareTag("Untagged"))
			return;

		ShapeData shapeData = collider.transform.GetComponentInParent<ShapeMono>().shapeData;

		if (collider.CompareTag("ScoreTrigger"))
			GameManager.Instance.UpdateScore(shapeData.scoreGain);

		else if (_invincibilityTime <= 0f)
		{
			TakeDamage(shapeData.contactDamage);
		}
	}

	public void RotateLeft()
	{
		_input = 1;
	}

	public void RotateRight()
	{
		_input = -1;
	}

	/// <summary>
	/// Callback method for the onGameOver event of the Game Manager.
	/// </summary>
	public void OnPlayerDeath()
	{
		_deathEffect.Play();
		graphics.SetActive(false);
	}

	/// <summary>
	/// Callback method for the onGameOver event of the Game Manager.
	/// </summary>
	public void OnPlayerRespawn()
	{
		_currentHealth = maxHealth;
		GameManager.Instance.UpdatePlayerHealth(maxHealth, _currentHealth);

		graphics.SetActive(true);
	}

	public void TakeDamage(int amount)
	{
		_currentHealth -= amount;
		_currentHealth = Mathf.Max(0, _currentHealth);

		Vector3 damageTextPos = transform.position + Vector3.up;
		DamageText.Generate(damageTextPrefab, damageTextPos, DamageTextStyle.Normal, amount.ToString());

		StopAllCoroutines();
		StartCoroutine(DamageFlash(FlashType.Damage));

		CameraShaker.Instance.ShakeCamera();
		GameManager.Instance.UpdatePlayerHealth(maxHealth, _currentHealth);
		AudioManager.Instance.PlayWithRandomPitch("Collide 1", .5f, 1.5f);

		DeviceVibration.Vibrate(100);

		_invincibilityTime = invincibilityTime;
	}

	public void Heal(int amount)
	{
		_currentHealth += amount;
		_currentHealth = Mathf.Min(maxHealth, _currentHealth);

		Vector3 damageTextPos = transform.position + Vector3.up;
		DamageText.Generate(damageTextPrefab, damageTextPos, DamageText.DefaultHealingColor, DamageTextStyle.Normal, amount.ToString());

		StopAllCoroutines();
		StartCoroutine(DamageFlash(FlashType.Heal));

		GameManager.Instance.UpdatePlayerHealth(maxHealth, _currentHealth);
	}

	private IEnumerator DamageFlash(FlashType type)
	{
		if (type == FlashType.Damage)
		{
			_mainMaterial.SetColor("_FlashColor", damageColor);

			int totalFlashCount = damageFlashCount * 2;

			for (int i = 1; i <= totalFlashCount; i++)
			{
				_mainMaterial.SetFloat("_FlashIntensity", i % 2 != 0 ? 1f : 0f);

				yield return new WaitForSeconds(.05f);
			}
		}
		else
		{
			float intensity = 1f;

			_mainMaterial.SetColor("_FlashColor", healColor);

			do
			{
				_mainMaterial.SetFloat("_FlashIntensity", intensity);
				intensity -= Time.deltaTime;

				yield return null;
			}
			while (intensity >= 0f);
		}
	}
}
