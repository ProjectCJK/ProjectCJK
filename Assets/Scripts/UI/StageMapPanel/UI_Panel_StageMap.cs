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
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => ChangeStage(i + 2));
            }
        }

        private void ChangeStage(int index)
        {
            ES3.Save($"{ES3Key.CurrentStage}", index);
            
            LoadingSceneManager.Instance.LoadSceneWithLoadingScreen(ESceneName.MainScene);
        }
    }
}