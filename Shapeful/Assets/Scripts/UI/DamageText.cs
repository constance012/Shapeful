using UnityEngine;
using TMPro;

public enum DamageTextStyle { Normal, Critical }

/// <summary>
/// A class to generates an UI popup text.
/// </summary
public class DamageText : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private TextMeshProUGUI displayText;
	
	[Header("Configurations"), Space]
	[Min(1f)] public float maxLifeTime = 1f;

	[Space, Min(.5f)] public float maxVelocity;
	[SerializeField] private AnimationCurve velocityCurve;
	[SerializeField] private Animator animator;

	public static readonly Color DefaultCriticalColor = new Color(.821f, .546f, .159f);
	public static readonly Color DefaultDamageColor = Color.red;
	public static readonly Color DefaultHealingColor = Color.green;

	// Private fields.
	private Color _currentTextColor;
	private string _animationStateName;

	private float _currentLifeTime;
	private bool _criticalHit;

	private void Start()
	{
		animator.Play(_animationStateName);
	}

	private void Update()
	{
		if (_currentLifeTime >= maxLifeTime)
		{
			animator.Play("End");
			return;
		}

		float lifeTimePassed = Mathf.Min(_currentLifeTime / maxLifeTime, 1f);
		float yVelocity = velocityCurve.Evaluate(lifeTimePassed) * maxVelocity;

		float selectedVelocity = _criticalHit ? yVelocity * 1.5f : yVelocity;

		// Gradually move up.
		transform.position += selectedVelocity * Time.deltaTime * Vector3.up;

		_currentLifeTime += Time.deltaTime;
	}

	#region Generate Method Overloads
	// Default color is red, and parent is world canvas.
	public static DamageText Generate(GameObject prefab, Vector3 pos, DamageTextStyle style, string textContent)
	{
		Transform canvas = GameObject.FindWithTag("WorldCanvas").transform;
		GameObject dmgTextObj = Instantiate(prefab, pos, Quaternion.identity);
		dmgTextObj.transform.SetParent(canvas, true);

		DamageText dmgText = dmgTextObj.GetComponent<DamageText>();

		Color textColor = style == DamageTextStyle.Critical ? DefaultCriticalColor : DefaultDamageColor;

		dmgText.Setup(textColor, textContent, style);
		return dmgText;
	}

	// Default parent is world canvas.
	public static DamageText Generate(GameObject prefab, Vector3 pos, Color txtColor, DamageTextStyle style, string textContent)
	{
		Transform canvas = GameObject.FindWithTag("WorldCanvas").transform;

		GameObject dmgTextObj = Instantiate(prefab, pos, Quaternion.identity);
		dmgTextObj.transform.SetParent(canvas, true);

		DamageText dmgText = dmgTextObj.GetComponent<DamageText>();

		dmgText.Setup(txtColor, textContent, style);
		return dmgText;
	}
	#endregion

	private void Setup(Color txtColor, string textContent, DamageTextStyle style)
	{
		displayText.text = textContent.ToUpper();
		_currentTextColor = txtColor;

		displayText.color = _currentTextColor;
		displayText.fontSize = 0f;

		_animationStateName = $"Start_{style}";
		_criticalHit = style == DamageTextStyle.Critical;
	}

	public void DestroyObject()
	{
		Destroy(gameObject);
	}
}
