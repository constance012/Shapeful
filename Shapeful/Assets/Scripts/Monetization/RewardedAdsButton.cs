using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using TMPro;
using System;

public class RewardedAdsButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
	[SerializeField] private Button targetButton;
	[SerializeField] private GameObject allowContinueText;
	[SerializeField] private TextMeshProUGUI onCooldownText;
	
	private const string androidAdUnitID = "Rewarded_Android";
	private const string iOSAdUnitID = "Rewarded_iOS";

	// Private fields.
	private string _adUnitID = "";

	private void Awake()
	{
		#if UNITY_ANDROID || UNITY_EDITOR
			_adUnitID = androidAdUnitID;
		#elif UNITY_IOS
			_adUnitID = iOSAdUnitID;
		#endif
	}

	private void Start()
	{
		LoadAd();
	}

	private void LoadAd()
	{
		Debug.Log($"Loading a rewarded ad for {_adUnitID}");

		if (Advertisement.isInitialized)
		{
			Advertisement.Load(_adUnitID, this);
			targetButton.interactable = false;
		}
	}

	private void ShowAd()
	{
		targetButton.interactable = false;
		Advertisement.Show(_adUnitID, this);
	}

	public void SetStatus(bool interactable, TimeSpan remainingTime)
	{
		onCooldownText.text = remainingTime.ToString(@"hh\:mm\:ss");

		if (targetButton.interactable != interactable)
		{
			targetButton.interactable = interactable;

			if (interactable)
			{
				allowContinueText.SetActive(true);
				onCooldownText.gameObject.SetActive(false);
			}
			else
			{
				allowContinueText.SetActive(false);
				onCooldownText.gameObject.SetActive(true);
			}
		}
	}

	#region Ads Load Interface Methods.
	public void OnUnityAdsAdLoaded(string adUnitID)
	{
		Debug.Log($"Ad loaded for {adUnitID}");

		if (adUnitID.Equals(_adUnitID))
		{
			targetButton.onClick.AddListener(ShowAd);
			targetButton.interactable = true;
		}
	}

	public void OnUnityAdsFailedToLoad(string adUnitID, UnityAdsLoadError error, string message)
	{
		Debug.LogError($"FAILED to LOAD the ad for {adUnitID}: {error} - {message}");
	}
	#endregion

	#region Ads Show Interface Methods.
	public void OnUnityAdsShowStart(string adUnitID) { }
	public void OnUnityAdsShowClick(string adUnitID) { }

	public void OnUnityAdsShowComplete(string adUnitID, UnityAdsShowCompletionState showCompletionState)
	{
		LoadAd(); // Load a new ad ready for the next available click.

		if (adUnitID.Equals(_adUnitID) && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
		{
			Debug.Log("Rewarded ad completed.");
			targetButton.onClick.RemoveAllListeners();

			// TODO - Grant rewards.
			// TODO - Check for remaing attempts.
			GameManager.Instance.ContinueGame();
		}
	}

	public void OnUnityAdsShowFailure(string adUnitID, UnityAdsShowError error, string message)
	{
		Debug.LogError($"FAILED to SHOW the ad for {adUnitID}: {error} - {message}");
	}

	#endregion
}
