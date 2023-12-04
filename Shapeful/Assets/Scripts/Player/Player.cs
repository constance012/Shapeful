using CSTGames.DataPersistence;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine;

public class Player : MonoBehaviour, ISaveDataTransceiver
{
	private enum FlashType { Damage, Heal, PowerUp }

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
	[SerializeField] private Color powerUpColor;

	// Private fields.
	private LocalKeyword _isFlashing;
	private Material _mainMaterial;
	private ParticleSystem _deathEffect;
	private ParticleSystem _powerUpEffect;
	
	private int _input;
	private int _currentHealth;
	private float _invincibilityTime;

	private void Awake()
	{
		_mainMaterial = this.GetComponentInChildren<SpriteRenderer>("Graphics/Main").material;
		_deathEffect = this.GetComponentInChildren<ParticleSystem>("Death Effect");
		_powerUpEffect = this.GetComponentInChildren<ParticleSystem>("Power-up In Effect");

		_isFlashing = new LocalKeyword(_mainMaterial.shader, "_IS_FLASHING_ON");
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

		if (_powerUpEffect.isPlaying)
		{
			_powerUpEffect.transform.rotation = Quaternion.identity;
			OnAnyPowerUpsExpired();
		}

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
			if (!TakeDamage(shapeData.contactDamage))
				GameManager.Instance.UpdateScore(shapeData.scoreGain);
		}
	}

	#region Interface Methods.
	public void SaveData(GameData data) { }

	public void LoadData(GameData data)
	{
		Color currentColor = data.playerColor;

		_mainMaterial.SetColor("_BaseColor", currentColor);
	}
	#endregion

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

	public bool TakeDamage(int amount)
	{
		amount *= Mathf.RoundToInt(1f - GameManager.Instance.DamageReduction);

		_currentHealth -= amount;
		_currentHealth = Mathf.Max(0, _currentHealth);

		if (amount > 0)
		{
			GenerateDamageText(amount, DamageText.DefaultDamageColor, DamageTextStyle.Normal);
			AudioManager.Instance.PlayWithRandomPitch("Collide 1", .5f, 1.5f);
		}
		else
		{
			GenerateDamageText("Blocked", new Color(.8f, .8f, .8f), DamageTextStyle.Critical);
			AudioManager.Instance.PlayWithRandomPitch("Damage Blocked", .8f, 1.3f);
		}

		StopAllCoroutines();
		StartCoroutine(DamageFlash(FlashType.Damage));

		CameraShaker.Instance.ShakeCamera();
		GameManager.Instance.UpdatePlayerHealth(maxHealth, _currentHealth);

		DeviceVibration.Vibrate(100);

		_invincibilityTime = invincibilityTime;

		return amount > 0;
	}

	public void Heal(int amount)
	{
		_currentHealth += amount;
		_currentHealth = Mathf.Min(maxHealth, _currentHealth);

		GenerateDamageText(amount, DamageText.DefaultHealingColor, DamageTextStyle.Normal);

		StopAllCoroutines();
		StartCoroutine(DamageFlash(FlashType.Heal));

		GameManager.Instance.UpdatePlayerHealth(maxHealth, _currentHealth);
	}

	public void PowerUpReceived(string powerUpName)
	{
		if (_powerUpEffect.isStopped)
			_powerUpEffect.Play();

		GenerateDamageText(powerUpName, DamageText.DefaultPowerUpColor, DamageTextStyle.Normal);

		StopAllCoroutines();
		StartCoroutine(DamageFlash(FlashType.PowerUp));
	}

	public void OnAnyPowerUpsExpired()
	{
		if (!PowerUpManager.Instance.AnyActivePowerUps)
			_powerUpEffect.Stop();
	}

	public void GenerateDamageText(object displayObject, Color textColor, DamageTextStyle style)
	{
		Vector3 damageTextPos = transform.position + Vector3.up;
		DamageText.Generate(damageTextPrefab, damageTextPos, textColor, style, displayObject.ToString());
	}

	private IEnumerator DamageFlash(FlashType type)
	{
		_mainMaterial.EnableKeyword(_isFlashing);

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

			Color color = type == FlashType.Heal ? healColor : powerUpColor;
			_mainMaterial.SetColor("_FlashColor", color);

			do
			{
				_mainMaterial.SetFloat("_FlashIntensity", intensity);
				intensity -= Time.deltaTime;

				yield return null;
			}
			while (intensity >= 0f);
		}

		_mainMaterial.DisableKeyword(_isFlashing);
	}
}
