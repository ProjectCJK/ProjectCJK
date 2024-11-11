using System;
using System.Collections;
using Modules.DesignPatterns.Singletons;
using UnityEngine;

namespace Managers
{
    public class CoroutineManager : SingletonMono<CoroutineManager>
    {
        // MonoBehaviour 없는 클래스에서도 사용할 수 있는 코루틴 실행 메서드
        public void StartManagedCoroutine(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }

        // 코루틴을 정지하는 메서드도 추가
        public void StopManagedCoroutine(IEnumerator coroutine)
        {
            StopCoroutine(coroutine);
        }

        public static IEnumerator Timer(float seconds, Action<bool> callback)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke(true);
        }
    }
}