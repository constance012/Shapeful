using CSTGames.DataPersistence;
using UnityEngine;
using UnityEngine.UI;

public class AppearanceMenu : MonoBehaviour
{
	[Header("Static References"), Space]
	protected static Image _primaryPreview;
	protected static Image _secondaryPreview;

	protected virtual void Awake()
	{
		if (_primaryPreview == null)
		{
			_primaryPreview = this.GetComponentInChildren<Image>("Player Sprite Preview/Primary");
			_secondaryPreview = this.GetComponentInChildren<Image>("Player Sprite Preview/Secondary");
		}

		GameDataManager.Instance.DistributeDataToTransceivers();
	}

	public void ConfirmChanges()
	{
		GameDataManager.Instance.SaveGame();
		gameObject.SetActive(false);
	}

	protected virtual void ReloadUI() { }
}