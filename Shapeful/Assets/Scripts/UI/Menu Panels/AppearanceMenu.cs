using UnityEngine;
using UnityEngine.UI;
using CSTGames.DataPersistence;
using TMPro;

public class AppearanceMenu : MonoBehaviour
{
	[Header("Static References"), Space]
	protected static Image _primaryPreview;
	protected static Image _secondaryPreview;
	protected static Image _staticPreview;
	protected static TextMeshProUGUI _gemShardsDisplayText;
	protected static ConfirmationBox _confirmBox;

	public static int GemShards { get; protected set; }

	protected virtual void Awake()
	{
		Debug.Log(_primaryPreview == null);
		
		if (_primaryPreview == null)
		{
			_primaryPreview = transform.GetComponentInChildren<Image>("Player Sprite Preview/Primary");
			_secondaryPreview = transform.GetComponentInChildren<Image>("Player Sprite Preview/Secondary");
			_staticPreview = transform.GetComponentInChildren<Image>("Player Sprite Preview/Static");

			_gemShardsDisplayText = transform.GetComponentInChildren<TextMeshProUGUI>("Gem Shards Display/Amount");
			_confirmBox = transform.GetComponentInChildren<ConfirmationBox>("Confirmation Box");
		}
	}

	private void Start()
	{
		GameDataManager.Instance.DistributeDataToTransceivers();
	}

	/// <summary>
	/// Callback method for the "Done" button.
	/// </summary>
	public void ConfirmChanges()
	{
		GameDataManager.Instance.SaveGame();
		gameObject.SetActive(false);
	}

	protected virtual void ReloadUI() { }
}