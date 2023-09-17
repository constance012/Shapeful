using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerControl : MonoBehaviour
{
	[Header("Move Speed"), Space]
	[SerializeField] private float moveSpeed;

	[Header("Health"), Space]
	public int health;
	[SerializeField, Min(1f)] private int damageFlashCount;

	// Private fields.
	private SpriteRenderer _renderer;
	private int _input;

	private void Awake()
	{
		_renderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		GameManager.Instance.UpdatePlayerHealth(health);
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

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (GameManager.Instance.gameOver)
			return;

		if (collision.tag.Equals("ScoreTrigger"))
			GameManager.Instance.UpdateScore();
		else
		{
			health--;
			StopAllCoroutines();
			StartCoroutine(DamageFlash());

			GameManager.Instance.UpdatePlayerHealth(health);
			AudioManager.Instance.PlayWithRandomPitch("Collide 1", .5f, 1f);
		}
	}
}
