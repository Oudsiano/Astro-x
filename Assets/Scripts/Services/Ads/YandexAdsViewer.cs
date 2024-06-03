using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Services.Ads
{
    public class YandexAdsViewer : MonoBehaviour, IAdsViewer
    {
        public bool IsShowingAd { get; set; }
        public bool CanShowInterstitial { get; set; }
        public bool CanShowRewarded { get; set; }
        
        private InterstitialAdLoader _interstitialAdLoader;
        private Interstitial _interstitial;
        
        private RewardedAdLoader _rewardedAdLoader;
        private RewardedAd _rewarded;
        
        private Action<bool> _endCallback;

        private bool _interstitialRequested;
        private bool _rewardedRequested;

        public void Initialize()
        {
            _interstitialAdLoader = new InterstitialAdLoader();
            _interstitialAdLoader.OnAdLoaded += HandleInterstitialAdLoaded;
            _interstitialAdLoader.OnAdFailedToLoad += HandleInterstitialAdFailedToLoad;
            RequestInterstitial();

            _rewardedAdLoader = new RewardedAdLoader();
            _rewardedAdLoader.OnAdLoaded += HandleRewardedAdLoaded;
            _rewardedAdLoader.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            RequestRewarded();
        }

#region Interstitial

        public void ShowInterstitial(Action<bool> callback)
        {
            Debug.Log("Request for show interstitial");
            if (_interstitialRequested)
            {
                Debug.Log("Already requested!");
                callback?.Invoke(false);
                return;
            }

            _interstitialRequested = true;
            if (_interstitial == null)
            {
                Debug.Log("Interstitial not requested");
                RequestInterstitial();
                return;
            }

            if (!CanShowInterstitial) return;
            
            Debug.Log("Showing interstitial");
            _interstitial.Show();
            IsShowingAd = true;
        }

        private void RequestInterstitial()
        {
            // Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            // Replace demo Unit ID 'demo-interstitial-yandex' with actual Ad Unit ID
            string adUnitId = "R-M-7038912-2";

            _interstitial?.Destroy();
            Debug.Log("Requesting new interstitial");
            _interstitialAdLoader.LoadAd(new AdRequestConfiguration.Builder(adUnitId).Build());
        }

        private void HandleInterstitialAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
        {
            Debug.Log("Interstitial ad loaded");
            _interstitial = args.Interstitial;
            if (_interstitial == null)
            {
                Debug.Log("Loaded interstitial is null");
                _interstitialRequested = false;
                _endCallback?.Invoke(false);
                _endCallback = null;
                return;
            }

            _interstitial.OnAdClicked += HandleInterstitialAdClicked;
            _interstitial.OnAdShown += HandleInterstitialAdShown;
            _interstitial.OnAdFailedToShow += HandleInterstitialAdFailedToShow;
            _interstitial.OnAdImpression += HandleInterstitialImpression;
            _interstitial.OnAdDismissed += HandleInterstitialAdDismissed;

            if (!_interstitialRequested) return;
            if (!CanShowInterstitial) return;
            
            Debug.Log("Showing interstitial");
            _interstitial.Show();
            IsShowingAd = true;
        }

        private void HandleInterstitialAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("Interstitial ad failed to load");
            _interstitialRequested = false;
            _endCallback?.Invoke(false);
            _endCallback = null;
        }

        private void HandleInterstitialAdClicked(object sender, EventArgs args)
        {
            Debug.Log("Interstitial ad clicked");
            _interstitialRequested = false;
        }

        private void HandleInterstitialAdShown(object sender, EventArgs args)
        {
            Debug.Log("Interstitial ad shown");
            // _endCallback?.Invoke(true);
            _interstitialRequested = false;
        }

        private void HandleInterstitialAdDismissed(object sender, EventArgs args)
        {
            Debug.Log("Interstitial ad Dismissed");
            _interstitialRequested = false;
            DestroyInterstitial();
            
            _endCallback?.Invoke(true);
            _endCallback = null;
            
            RequestInterstitial();
        }

        private void HandleInterstitialImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            Debug.Log($"Interstitial ad impressed: {data}");
            _interstitialRequested = false;
        }

        private void HandleInterstitialAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            Debug.Log("Interstitial ad failed to show");
            _interstitialRequested = false;
            DestroyInterstitial();
            _endCallback?.Invoke(false);
            _endCallback = null;
        }

        private void DestroyInterstitial()
        {
            if (_interstitial == null) return;
            
            _interstitial.Destroy();
            _interstitial = null;
            _interstitialRequested = false;
            IsShowingAd = false;
        }
#endregion

#region Rewarded
        public void ShowRewarded(Action<bool> callback)
        {
            Debug.Log("Request for show rewarded");
            if (_rewardedRequested)
            {
                Debug.Log("Already requested!");
                callback?.Invoke(false);
                return;
            }

            _endCallback = callback;

            _rewardedRequested = true;
            if (_rewarded == null)
            {
                Debug.Log("Rewarded not requested");
                RequestRewarded();
                return;
            }

            if (!CanShowRewarded) return;
            Debug.Log("Showing rewarded on request");
            _rewarded.Show();
            IsShowingAd = true;
        }

        private void RequestRewarded()
        {
            // Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            // Replace demo Unit ID 'demo-interstitial-yandex' with actual Ad Unit ID
            string adUnitId = "R-M-7038912-1";

            _rewarded?.Destroy();
            Debug.Log("Requesting new interstitial");
            _rewardedAdLoader.LoadAd(new AdRequestConfiguration.Builder(adUnitId).Build());
        }
                
        private void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("Rewarded ad failed to load");
            _rewardedRequested = false;
            IsShowingAd = false;
            _endCallback?.Invoke(false);
            _endCallback = null;
        }

        private void HandleRewardedAdLoaded(object sender, RewardedAdLoadedEventArgs args)
        {
            Debug.Log("rewarded ad loaded");
            _rewarded = args.RewardedAd;
            if (_rewarded == null)
            {
                Debug.Log("Loaded rewarded is null");
                _rewardedRequested = false;
                _endCallback?.Invoke(false);
                _endCallback = null;
                return;
            }
            
            _rewarded.OnAdClicked += HandleRewardedAdClicked;
            _rewarded.OnAdShown += HandleRewardedAdShown;
            _rewarded.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            _rewarded.OnAdImpression += HandleRewardedImpression;
            _rewarded.OnAdDismissed += HandleRewardedAdDismissed;

            if (!_rewardedRequested) return;
            if (!CanShowRewarded) return;
            
            Debug.Log("Showing rewarded on load");
            _rewarded.Show();
            IsShowingAd = true;
        }

        private void HandleRewardedAdClicked(object sender, EventArgs args)
        {
            Debug.Log("Rewarded ad clicked");
            _rewardedRequested = false;
        }

        private void HandleRewardedAdShown(object sender, EventArgs args)
        {
            Debug.Log("Rewarded ad shown");
            _rewardedRequested = false;
        }

        private void HandleRewardedAdDismissed(object sender, EventArgs args)
        {
            Debug.Log("Rewarded ad Dismissed");
            _rewardedRequested = false;
            DestroyRewarded();
            
            _endCallback?.Invoke(false);
            _endCallback = null;
            
            RequestRewarded();
        }

        private void HandleRewardedImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            Debug.Log($"Rewarded ad impressed: {data}");
            _rewardedRequested = false;
            
            _endCallback?.Invoke(true);
            _endCallback = null;
        }

        private void HandleRewardedAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            Debug.Log("Rewarded ad failed to show");
            _rewardedRequested = false;
            DestroyRewarded();
            _endCallback?.Invoke(false);
            _endCallback = null;
        }

        private void DestroyRewarded()
        {
            if (_rewarded == null) return;
            
            _rewarded.Destroy();
            _rewarded = null;
            _rewardedRequested = false;
            IsShowingAd = false;
        }
#endregion
    }
}