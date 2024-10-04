using System;
using System.Collections;
using UnityEngine;

namespace Units.Managers
{
    public static class CoroutineManager
    {
        public static IEnumerator Timer(float seconds, Action<bool> callback)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke(true);
        }
    }
}