using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : Singleton<AdsInitializer>, IUnityAdsInitializationListener
{
	[SerializeField] private string androidGameID;
	[SerializeField] private string iOSGameID;
	[SerializeField] private bool testMode;

	private string _gameID;

	protected override void Awake()
	{
		base.Awake();
			
		InitializeAd();
	}

	public void InitializeAd()
	{
		#if UNITY_ANDROID || UNITY_EDITOR
			_gameID = androidGameID;

		#elif UNITY_IOS
			_gameID = iOSGameID;
		
		#endif

		if (!Advertisement.isInitialized)
			Advertisement.Initialize(_gameID, testMode, this);
	}

	public void OnInitializationComplete()
	{
		Debug.Log("Ads initialization complete.");
	}

	public void OnInitializationFailed(UnityAdsInitializationError error, string message)
	{
		Debug.LogError($"Ads initialization FAILED: {error} - {message}");
	}
}
