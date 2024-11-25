using System;
using System.Collections.Generic;
using Firebase.Analytics;
using Modules.DesignPatterns.Singletons;
using UnityEngine;

namespace Managers
{
    public class AdsManager : SingletonMono<AdsManager>
    {
        private Action<MaxSdk.Reward, MaxSdkBase.AdInfo> pendingRewardAction;
        private string adUnitKey;
        private Dictionary<string, int> adsShownToday = new Dictionary<string, int>();
        private DateTime lastAdShownDate;
        private bool isInitialized = false;
        // private string MaxSdkKey = "QqxxU259hFqPcXv2vIq4mtdJVaJ7Dt7G3WBsXWONa76yR9urWDB9M55t5o7ZlHG602Od1bEFcTGNOmxNxsEv1L";
        // #if UNITY_ANDROID && UNITY_EDITOR
        private string adUnitId = "af6b54d8cfff4ac8";
    
        private const string ADS_SHOWN_TODAY_SAVE = "ADS_SHOWN_TODAY_SAVE";
        private const string LAST_AD_SHOW_DATE_SAVE = "LAST_AD_SHOW_DATE_SAVE";
    

        // #endif
        int retryAttempt;
        
        public void Initialize()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                InitializeRewardedAds();
                isInitialized = true;
            };
            //MaxSdk.SetSdkKey(MaxSdkKey);
            MaxSdk.InitializeSdk();

            adsShownToday = ES3.Load<Dictionary<string, int>>(ADS_SHOWN_TODAY_SAVE, new Dictionary<string, int>());
            lastAdShownDate = ES3.Load<DateTime>(LAST_AD_SHOW_DATE_SAVE, DateTime.Today);
            UpdateAdTracking();
        }
        #region AppLovinMax Mathrd
        public void InitializeRewardedAds()
        {
            // Attach callback
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += HandleReward;
            // Load the first rewarded ad
            LoadRewardedAd();
        }
        private void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(adUnitId);
        }
        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
            // Reset retry attempt
            retryAttempt = 0;
        }
        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).
            retryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
            Invoke("LoadRewardedAd", (float)retryDelay);
        }
        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // 광고 표시 실패 로깅
            LogAdEvent($"reward_ad_{adUnitKey}", adUnitKey, "failed", errorInfo.Message);
            LoadRewardedAd();
        }
        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd();
        }
        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.
        }
        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // 광고 수익 발생 시 별도 이벤트 로깅
            try
            {
                Parameter[] parameters = new Parameter[]
                {
                    new Parameter(FirebaseAnalytics.ParameterCurrency, "USD"),
                    new Parameter(FirebaseAnalytics.ParameterValue, adInfo.Revenue),
                    new Parameter("network_name", adInfo.NetworkName),
                    new Parameter("ad_unit_id", adUnitId),
                    new Parameter("placement", adInfo.Placement)
                };

                FirebaseAnalytics.LogEvent("ad_revenue_paid", parameters);
            }
            catch (Exception e)
            {
                Debug.LogError($"수익 이벤트 로깅 실패: {e.Message}");
            }
        }
        #endregion

        // 광고 이벤트 타입 정의
        public enum AdEventType
        {
            Item,       // 소환 아이템
            Tower,      // 소환 타워
            Speed,      // 2배속
            Buff,       // 버프 선택 다시
            Clear,      // 보상 2배
            Heart       // 생명력 충전
        }

        private void LogAdEvent(string eventName, string adUnitKey, string status, string errorMessage = null)
        {
            try
            {
                var parameters = new List<Parameter>
                {
                    new Parameter("ad_type", adUnitKey),
                    new Parameter("status", status),
                    new Parameter("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                };

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    parameters.Add(new Parameter("error_message", errorMessage));
                }

                FirebaseAnalytics.LogEvent(eventName, parameters.ToArray());
                Debug.Log($"Firebase Analytics 이벤트 기록: {eventName} - {adUnitKey} - {status}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Firebase Analytics 이벤트 기록 실패: {e.Message}");
            }
        }

        public void ShowRewardedAd(string adUnitKey, Action<MaxSdk.Reward, MaxSdkBase.AdInfo> rewardAction)
        {
            // 광고 시청 시도 로깅
            LogAdEvent($"reward_ad_{adUnitKey}", adUnitKey, "attempt");

            this.adUnitKey = adUnitKey;
            pendingRewardAction = (reward, adInfo) =>
            {
                // 광고 시청 완료 로깅
                LogAdEvent($"reward_ad_{adUnitKey}", adUnitKey, "completed");
                rewardAction?.Invoke(reward, adInfo);
            };

            if (MaxSdk.IsRewardedAdReady(adUnitId))
            {
                MaxSdk.ShowRewardedAd(adUnitId);
            }
            else
            {
                LogAdEvent($"reward_ad_{adUnitKey}", adUnitKey, "not_ready");
                Debug.Log("Ad not ready, loading ad...");
                LoadRewardedAd();
            }
        }

        public void ShowRewardedAdByTime(string adUnitKey, Action<MaxSdk.Reward, MaxSdkBase.AdInfo> rewardAction)
        {
            adsShownToday[adUnitKey] = 0;
            UpdateAdTracking();
            ShowRewardedAd(adUnitKey, rewardAction);
        }

        public void ShowRewardedAdByDay(string adUnitKey, Action<MaxSdk.Reward, MaxSdkBase.AdInfo> rewardAction)
        {
            UpdateAdTracking();
            if (!adsShownToday.ContainsKey(adUnitKey))
            {
                adsShownToday[adUnitKey] = 0;
            }

            if (adsShownToday[adUnitKey] < 3)
            {
                ShowRewardedAd(adUnitKey, rewardAction);
            }
            else
            {
                LogAdEvent($"reward_ad_{adUnitKey}", adUnitKey, "daily_limit_reached");
                Debug.Log("Daily limit for this ad reached. No more views today.");
            }
        }

        public int GetAdCount(string adType)
        {
            if (adsShownToday.ContainsKey(adType))
            {
                return adsShownToday[adType];
            }
            return 0;
        }
        private void HandleReward(string adUnitKey, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            if (pendingRewardAction != null)
            {
                // 광고 시청 완료 로깅
                LogAdEvent($"reward_ad_{adUnitKey}", adUnitKey, "completed");

                IncrementAdCount(this.adUnitKey);
                pendingRewardAction.Invoke(reward, adInfo);
                pendingRewardAction = null;
                this.adUnitKey = null;
            }
        }
        private void IncrementAdCount(string adUnitKey)
        {
            if (!adsShownToday.ContainsKey(adUnitKey))
            {
                adsShownToday[adUnitKey] = 0;
            }
            adsShownToday[adUnitKey]++;
            lastAdShownDate = DateTime.Today;

            try
            {
                ES3.Save(ADS_SHOWN_TODAY_SAVE, adsShownToday, ES3.settings);
                ES3.Save(LAST_AD_SHOW_DATE_SAVE, lastAdShownDate, ES3.settings);

                Debug.Log($"광고 데이터 저장 성공: {adUnitKey} - 횟수: {adsShownToday[adUnitKey]}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"광고 데이터 저장 실패: {e.Message}");
            }
        }
        private void UpdateAdTracking()
        {
            DateTime today = DateTime.Today;
            if (lastAdShownDate != today)
            {
                lastAdShownDate = today;
                adsShownToday.Clear();

                try
                {
                    ES3.Save(ADS_SHOWN_TODAY_SAVE, adsShownToday, ES3.settings);
                    ES3.Save(LAST_AD_SHOW_DATE_SAVE, lastAdShownDate, ES3.settings);

                    Debug.Log("광고 데이터 초기화 저장 성공");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"광고 데이터 초기화 저장 실패: {e.Message}");
                }
            }
        }
        private void OnApplicationQuit()
        {
            try
            {
                ES3.Save(ADS_SHOWN_TODAY_SAVE, adsShownToday, ES3.settings);
                ES3.Save(LAST_AD_SHOW_DATE_SAVE, lastAdShownDate, ES3.settings);

                Debug.Log("앱 종료 시 광고 데이터 저장 성공");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"앱 종료 시 광고 데이터 저장 실패: {e.Message}");
            }
        }
        public bool IsInitialized()
        {
            return isInitialized;
        }
        public TimeSpan GetTimeUntilNextReset()
        {
            DateTime now = DateTime.Now;
            DateTime nextReset = DateTime.Today.AddDays(1); // 다음날 자정
            return nextReset - now;
        }
    }
}