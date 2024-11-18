using System;
using System.Collections.Generic;
using Managers;
using Units.Stages.Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.StageMapPanel
{
    public class UI_Panel_StageMap : MonoBehaviour
    {
        [SerializeField] List<Button> buttons = new();

        public void RegisterReference()
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                var capturedIndex = i; // 로컬 변수에 i 값 복사
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => ChangeStage(capturedIndex));
            }
        }

        private void ChangeStage(int index)
        {
            ES3.Save($"{EES3Key.CurrentStage}", index, ES3.settings);
            
            LoadingSceneManager.Instance.LoadSceneWithLoadingScreen(ESceneName.MainScene);
        }
    }
}