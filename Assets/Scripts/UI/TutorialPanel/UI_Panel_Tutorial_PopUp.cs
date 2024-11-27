using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TutorialPanel
{
    public class UI_Panel_Tutorial_PopUp : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _balloonText;
        [SerializeField] private Button closeButton;
        
        private List<string> textLists = new();
        
        public Action<int> OnClickExitButton;

        public void RegisterReference()
        {
            textLists = new List<string>
            {
                "캔닝머신을 건설하기",
                "스탠드 건설하기",
                "사냥터 해금하기",
                "토마토 사냥하기",
                "토마토 통조림 제작하기",
                "토마토 통조림 진열하기",
                "토마토 통조림 판매하기",
                "캔닝머신 가격 올리기",
                "직원 고용하기",
            };
            
            // if (GameManager.Instance.ES3Saver.PopUpTutorialClear.Count == 0)
            // {
            //     for (var i = 0 ; i < uiPanelPopUpTutorialItem.Count ; i++)
            //     {
            //         GameManager.Instance.ES3Saver.PopUpTutorialClear.TryAdd(i, false);
            //     }
            // }
        }

        public void ActivatePanel(int index)
        {
            _balloonText.text = textLists[index];
            gameObject.SetActive(true);
            
            closeButton.onClick.RemoveAllListeners();;
            closeButton.onClick.AddListener(() =>
            {
                OnClickExitButton?.Invoke(index);
                gameObject.SetActive(false);
            });
        }
    }
}