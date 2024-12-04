using System;
using UnityEngine;

namespace Modules
{
    public static class VibrationController
    {
        public static void TriggerVibration(int duration = 200)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using var vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                vibrator?.Call("vibrate", duration); // 진동 시간(밀리초) 설정
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to trigger vibration: {e.Message}");
            }
#else
            Debug.Log("Vibration triggered (Editor mode or non-Android platform)");
#endif
        }
    }
}