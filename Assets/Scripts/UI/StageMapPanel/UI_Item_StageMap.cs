using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.StageMapPanel
{
    public enum EStageState
    {
        Current,
        Open,
        Standby,
        Lock
    }
    
    public class UI_Item_StageMap : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI extraRevenueText;
        [SerializeField] private TextMeshProUGUI requiredLevel;
        
        [SerializeField] private Image requiredLevelImage;
        [SerializeField] private List<TextMeshProUGUI> requiredGold;
        [SerializeField] private List<Image> stageImage;
        [SerializeField] private List<Button> buttons;

        public void RegisterReference(Action onClickButton)
        {
            buttons[1].onClick.AddListener(() => onClickButton?.Invoke());
        }

        public void UpdateUI(StageData stageData)
        {
            extraRevenueText.text = $"Profit {stageData.RevenueMultiplier}%";
            requiredLevel.text = $"{stageData.RequiredLevel}";

            foreach (TextMeshProUGUI requiredGoldText in requiredGold)
            {
                requiredGoldText.text = $"<sprite=0> {stageData.UnlockCost}";
            }

            requiredLevelImage.gameObject.SetActive(false);

            foreach (Image image in stageImage)
            {
                image.gameObject.SetActive(false);
            }

            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(false);
            }

            if (GameManager.Instance.ES3Saver.CurrentStageLevel < stageData.StageLevel)
            {
                if (GameManager.Instance.ES3Saver.CurrentPlayerLevel >= stageData.RequiredLevel)
                {
                    if (GameManager.Instance.ES3Saver.Gold >= stageData.UnlockCost)
                    {
                        stageImage[1].gameObject.SetActive(true);
                        buttons[1].gameObject.SetActive(true);
                    }
                    else
                    {
                        stageImage[0].gameObject.SetActive(true);
                        buttons[0].gameObject.SetActive(true);
                    }
                }
                else
                {
                    requiredLevelImage.gameObject.SetActive(true);
                    stageImage[0].gameObject.SetActive(true);
                }
            }
            else if (GameManager.Instance.ES3Saver.CurrentStageLevel > stageData.StageLevel)
            {
                stageImage[0].gameObject.SetActive(true);
            }
            else
            {
                stageImage[1].gameObject.SetActive(true);
            }
        }
    }
}