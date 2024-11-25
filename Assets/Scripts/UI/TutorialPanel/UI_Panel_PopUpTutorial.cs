using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TutorialPanel
{
    public class UI_Panel_PopUpTutorial : MonoBehaviour
    {
        [SerializeField] private List<GameObject> uiPanelPopUpTutorialItem;
        [SerializeField] private Button closeButton;
        
        public Action OnClickExitButton;

        public void RegisterReference()
        {
            if (GameManager.Instance.ES3Saver.PopUpTutorialClear.Count == 0)
            {
                for (var i = 0 ; i < uiPanelPopUpTutorialItem.Count ; i++)
                {
                    GameManager.Instance.ES3Saver.PopUpTutorialClear.TryAdd(i, false);
                }
            }
            
            InactivePanel();
            
            closeButton.onClick.AddListener(() =>
            {
                OnClickExitButton?.Invoke();
                InactivePanel();
                gameObject.SetActive(false);
            });
        }

        private void InactivePanel()
        {
            foreach (GameObject tutorialPanelItem in uiPanelPopUpTutorialItem)
            {
                tutorialPanelItem.gameObject.SetActive(false);
            }
        }

        public void ActivatePanel(int index)
        {
            uiPanelPopUpTutorialItem[index].gameObject.SetActive(true);
            gameObject.SetActive(true);
        }
    }
}