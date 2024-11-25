using System;
using System.Collections.Generic;
using Modules.DesignPatterns.Singletons;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

namespace Managers
{
    [Serializable]
    public struct PushNotification
    {
        public int time; // 초 단위로 알림 발송 시간 설정
        public string description; // 알림 내용
    }

    public class PushManager : SingletonMono<PushManager>
    {
        [SerializeField] private List<PushNotification> _pushNotification;

        private static int HasPermissionChecked
        {
            get => PlayerPrefs.GetInt("hasPermissionChecked_" + Application.productName, 0);
            set => PlayerPrefs.SetInt("hasPermissionChecked_" + Application.productName, value);
        }

        private static DateTime LastExitTime
        {
            get => DateTime.TryParse(PlayerPrefs.GetString("LastExitTime", ""), out DateTime result) ? result : DateTime.MinValue;
            set => PlayerPrefs.SetString("LastExitTime", value.ToString("o"));
        }

#if UNITY_EDITOR || UNITY_ANDROID

        private void Start()
        {
            // 최초 권한 체크
            if (HasPermissionChecked == 0)
            {
                CheckNotificationPermission();
                HasPermissionChecked = 1;
            }

            // 기존 알림 제거
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.CancelAllScheduledNotifications();

            // 권한 상태 확인 후 알림 예약
            if (Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                SchedulePushNotifications();
            }
            else
            {
                Debug.LogWarning("푸시 알림 권한이 허용되지 않았습니다.");
            }
        }

        private void CheckNotificationPermission()
        {
            // 푸시 알림 권한 확인
            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                // 권한 요청
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                // 게임이 백그라운드로 이동할 때 종료 시간 기록
                LastExitTime = DateTime.Now;
                return;
            }

            // 앱 재개 시 알림 제거 및 재등록
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.CancelAllScheduledNotifications();

            if (Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                SchedulePushNotifications();
            }
        }

        private void OnApplicationQuit()
        {
            // 게임 종료 시 시간 기록
            LastExitTime = DateTime.Now;
        }

        private void SchedulePushNotifications()
        {
            // 알림 채널 등록
            var channel = new AndroidNotificationChannel
            {
                Id = "default_channel",
                Name = "Default Notifications",
                Importance = Importance.Default,
                Description = "Default Channel for App Notifications"
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            // 종료 후 경과 시간 계산
            TimeSpan timeSinceExit = DateTime.Now - LastExitTime;

            // _pushNotification 순회 및 알림 예약
            foreach (PushNotification push in _pushNotification)
            {
                var remainingTime = push.time - (int)timeSinceExit.TotalSeconds;

                if (remainingTime > 0)
                {
                    var notification = new AndroidNotification
                    {
                        Title = "The Last Food Outpost : Tycoon",
                        Text = push.description,
                        FireTime = DateTime.Now.AddSeconds(remainingTime)
                    };

                    AndroidNotificationCenter.SendNotification(notification, "default_channel");
                    Debug.Log($"푸시 알림 예약: {push.description} ({remainingTime}초 후)");
                }
                else
                {
                    Debug.Log($"푸시 알림 건너뜀: {push.description} (이미 초과)");
                }
            }
        }

#endif
    }
}