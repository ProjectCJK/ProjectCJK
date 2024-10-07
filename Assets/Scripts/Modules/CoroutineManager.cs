using System;
using System.Collections;
using UnityEngine;

namespace Modules
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