using System;

namespace Services.Ads
{
    public interface IAdsViewer
    {
        public bool IsShowingAd { get; set; }
        public bool CanShowInterstitial { get; set; }
        public bool CanShowRewarded { get; set; }
        
        public void ShowInterstitial(Action<bool> callback);
        public void ShowRewarded(Action<bool> callback);
    }
}