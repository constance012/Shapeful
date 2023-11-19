using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAdsManager : Singleton<InterstitialAdsManager>, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
	[Header("Game IDs"), Space]
	[SerializeField] private string androidGameID;
	[SerializeField] private string iOSGameID;
	[SerializeField] private bool testMode;

	[Header("Interstitial Ads Settings"), Space]
	[SerializeField, Tooltip("The minimum interval required for showing the ad, in SECONDS.")]
	private double minimumTimeInterval;

	private const string androidAdUnitID = "Interstitial_Android";
	private const string iOSAdUnitID = "Interstitial_iOS";

	// Private fields.
	private TimeSpan _currentInterval;
	private DateTime _mostRecent;

	private string _gameID;
	private string _adUnitID = "";

	protected override void Awake()
	{
		base.Awake();
			
		InitializeAd();
	}

	private void Start()
	{
		_currentInterval = TimeSpan.FromSeconds(minimumTimeInterval);
		_mostRecent = DateTime.Now;

		LoadAd();
	}

	private void InitializeAd()
	{
#if UNITY_ANDROID || UNITY_EDITOR
		_gameID = androidGameID;
		_adUnitID = androidAdUnitID;

#elif UNITY_IOS
		_gameID = iOSGameID;
		_adUnitID = iOSAdUnitUD;
		
#endif

		if (!Advertisement.isInitialized)
			Advertisement.Initialize(_gameID, testMode, this);
	}

	private void LoadAd()
	{
		Debug.Log($"Loading an interstitial ad for {_adUnitID}");

		if (Advertisement.isInitialized)
		{
			Advertisement.Load(_adUnitID, this);
		}
	}

	public bool TryShowAd()
	{
		TimeSpan timePassed = DateTime.Now - _mostRecent;
		bool success;

		if (timePassed >= _currentInterval)
		{
			Advertisement.Show(_adUnitID, this);

			_currentInterval = TimeSpan.FromSeconds(minimumTimeInterval);
			success = true;
		}
		else
		{
			_currentInterval -= timePassed;
			success = false;
		}

		_mostRecent = DateTime.Now;

		return success;
	}

	#region Ads Initialization Interface Methods.
	public void OnInitializationComplete()
	{
		Debug.Log("Ads initialization complete.");

		LoadAd();
	}

	public void OnInitializationFailed(UnityAdsInitializationError error, string message)
	{
		Debug.LogError($"Ads initialization FAILED: {error} - {message}");
	}
	#endregion

	#region Ads Load Interface Methods.
	public void OnUnityAdsAdLoaded(string adUnitID)
	{
		Debug.Log($"Ad loaded for {adUnitID}");
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
		Debug.Log("Interstitial ad completed.");
		
		LoadAd(); // Load a new ad ready for the next show.
	}

	public void OnUnityAdsShowFailure(string adUnitID, UnityAdsShowError error, string message)
	{
		Debug.LogError($"FAILED to SHOW the ad for {adUnitID}: {error} - {message}");
	}

	#endregion
}
