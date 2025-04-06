using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("Game IDs")]
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;

    [Header("Ad Unit IDs")]
    [SerializeField] string _androidInterstitialAdUnitId = "Interstitial_Android";
    [SerializeField] string _androidRewardedAdUnitId = "Rewarded_Android";
    [SerializeField] string _androidBannerAdUnitId = "Banner_Android";

    [SerializeField] string _iOSInterstitialAdUnitId = "Interstitial_iOS";
    [SerializeField] string _iOSRewardedAdUnitId = "Rewarded_iOS";
    [SerializeField] string _iOSBannerAdUnitId = "Banner_iOS";

    private string _gameId;
    private string _interstitialAdUnitId;
    private string _rewardedAdUnitId;
    private string _bannerAdUnitId;

    private System.Action _onRewardedComplete;

    void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
#if UNITY_IOS
        _gameId = _iOSGameId;
        _interstitialAdUnitId = _iOSInterstitialAdUnitId;
        _rewardedAdUnitId = _iOSRewardedAdUnitId;
        _bannerAdUnitId = _iOSBannerAdUnitId;
#else
        _gameId = _androidGameId;
        _interstitialAdUnitId = _androidInterstitialAdUnitId;
        _rewardedAdUnitId = _androidRewardedAdUnitId;
        _bannerAdUnitId = _androidBannerAdUnitId;
#endif

        Advertisement.Initialize(_gameId, _testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialized.");
        Advertisement.Load(_interstitialAdUnitId, this);
        Advertisement.Load(_rewardedAdUnitId, this);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Load(_bannerAdUnitId);
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error} - {message}");
    }

    // ---------- Interstitial ----------
    public void ShowInterstitialAd()
    {
        Advertisement.Show(_interstitialAdUnitId, this);
    }

    // ---------- Rewarded ----------
    public void ShowRewardedAd(System.Action onRewarded)
    {
        _onRewardedComplete = onRewarded;
        Advertisement.Show(_rewardedAdUnitId, this);
    }

    // ---------- Banner ----------
    public void ShowBanner()
    {
        Advertisement.Banner.Show(_bannerAdUnitId);
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide(false);
    }

    // ---------- Load Listener ----------
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"Ad Loaded: {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Failed to Load Ad: {placementId} - {error} - {message}");
    }

    // ---------- Show Listener ----------
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == _rewardedAdUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Rewarded ad watched completely. Grant reward.");
            _onRewardedComplete?.Invoke();
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Ad Show Failed: {placementId} - {error} - {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"Ad Show Started: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"Ad Clicked: {placementId}");
    }
}
