using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
	[Header("Move Speed"), Space]
	[SerializeField] private float moveSpeed;

	[Header("Health"), Space]
	public int maxHealth;
	[SerializeField, Min(1)] private int damageFlashCount;
	[SerializeField, Range(.1f, 1f)] private float invincibilityTime;

	// Private fields.
	private SpriteRenderer _renderer;
	private ParticleSystem _deathEffect;
	
	private int _input;
	private int _currentHealth;
	private float _invincibilityTime;

	private void Awake()
	{
		_renderer = this.GetComponentInChildren<SpriteRenderer>("Graphics/Main");
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
	}

	private void FixedUpdate()
	{
		float angle = _input * moveSpeed * Time.deltaTime;
		transform.RotateAround(Vector3.zero, Vector3.forward, angle);

		_input = 0;
	}

	public void RotateLeft()
	{
		_input = 1;
	}

	public void RotateRight()
	{
		_input = -1;
	}

	private IEnumerator DamageFlash()
	{
		int totalFlashCount = damageFlashCount * 2;

		for (int i = 1; i <= totalFlashCount; i++)
		{
			_renderer.color = i % 2 != 0 ? new Color(.82f, .174f, .174f) : Color.white;

			yield return new WaitForSeconds(.05f);
		}
	}

	public void OnPlayerDeath()
	{
		_deathEffect.Play();
		_renderer.transform.parent.gameObject.SetActive(false);
	}

	public void OnPlayerRespawn()
	{
		_currentHealth = maxHealth;
		GameManager.Instance.UpdatePlayerHealth(maxHealth, _currentHealth);

		_renderer.transform.parent.gameObject.SetActive(true);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (GameManager.Instance.GameOver)
			return;

		if (collision.tag.Equals("ScoreTrigger"))
			GameManager.Instance.UpdateScore();

		else if (_invincibilityTime <= 0f)
		{
			_currentHealth--;
			_invincibilityTime = invincibilityTime;

			StopAllCoroutines();
			StartCoroutine(DamageFlash());

			CameraShaker.Instance.ShakeCamera();
			GameManager.Instance.UpdatePlayerHealth(maxHealth, _currentHealth);
			AudioManager.Instance.PlayWithRandomPitch("Collide 1", .5f, 1.5f);

			DeviceVibration.Vibrate(100);
		}
	}
}
