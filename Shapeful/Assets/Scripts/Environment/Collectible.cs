using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private ParticleSystem collectEffect;
    protected static Player player;

	private void Awake()
	{
		if (player == null)
			player = GameObject.FindWithTag("Player").GetComponent<Player>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collectEffect != null)
			Instantiate(collectEffect, player.transform.position, Quaternion.identity);

        OnCollected();

		Destroy(gameObject);
	}

	protected abstract void OnCollected();
}
