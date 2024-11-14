using System;
using Units.Stages.Managers;
using UnityEngine;

namespace Modules
{
    public class Test : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                LoadingSceneManager.Instance.LoadSceneWithLoadingScreen("MainScene");
            }
        }
    }
}