using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Services.Ads
{
    public class UnityAdsViewer : MonoBehaviour, IUnityAdsShowListener, IUnityAdsInitializationListener, IUnityAdsLoadListener, IAdsViewer
    {
        private const string MyGameIdAndroid = "5607757";
        private const string ADUnitIdAndroid = "Interstitial_Android";
        private const string ADUnitIdAndroidRewarded = "Rewarded_Android";

        public bool IsShowingAd { get; set; }
        public bool CanShowInterstitial { get; set; }
        public bool CanShowRewarded { get; set; }
        
        [SerializeField] private bool _testMode = true;

        private bool _adStarted;
        private string _adStartedId;

        private Action<bool> _endCallback;

        public void Initialize()
        {
            Advertisement.Initialize(MyGameIdAndroid, _testMode, this);
        }

        public void ShowInterstitial(Action<bool> endCallback)
        {
            if (!Advertisement.isInitialized)
            {
                _adStarted = false;
                endCallback?.Invoke(false);
                return;
            }
            if (_adStarted)
            {
                endCallback?.Invoke(false);
                return;
            }

            IsShowingAd = true;
            Advertisement.Load(ADUnitIdAndroid, this);
            _endCallback = endCallback;
            _adStarted = true;
        }
        
        public void ShowRewarded(Action<bool> endCallback)
        {
            if (!Advertisement.isInitialized)
            {
                _adStarted = false;
                endCallback?.Invoke(false);
                return;
            }
            if (_adStarted)
            {
                endCallback?.Invoke(false);
                return;
            }

            IsShowingAd = true;
            Advertisement.Load(ADUnitIdAndroidRewarded, this);
            _endCallback = endCallback;
            _adStarted = true;
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.LogError($"Ad {placementId} show failure, error: {error}, message: {message}");
            IsShowingAd = false;
            _adStarted = false;
            _endCallback?.Invoke(false);
            _endCallback = null;
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            Debug.Log($"Ad {placementId} show start");
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log($"Ad {placementId} show click");
            _adStarted = false;
            IsShowingAd = false;
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.Log($"Ad {placementId} show complete");
            _adStarted = false;
            IsShowingAd = false;
            _endCallback?.Invoke(showCompletionState == UnityAdsShowCompletionState.COMPLETED);
            _endCallback = null;
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Ads initialized");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.LogError($"Ads initialization error: {error}, message: {message}");
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.Log($"Ad {placementId} loaded");
            Advertisement.Show(placementId, this);
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.LogError($"Ad {placementId} failed to load, error: {error}, message: {message}");
            _adStarted = false;
            IsShowingAd = false;
            _endCallback?.Invoke(false);
            _endCallback = null;
        }
    }
}