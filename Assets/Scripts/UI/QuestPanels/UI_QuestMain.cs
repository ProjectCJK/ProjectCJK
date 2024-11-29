using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.QuestPanels
{
    [Serializable]
    public class UIListQuestInfoItem
    {
        public Sprite StageIcon;
        public string StageDescription;
        public int ListQuestCurrentIndex;
        public int ListQuestMaxIndex;
        
        public int QuestClearCount;
        public int QuestTotalCount;
        public float ProgressRatio;
        
        public Sprite RewardSprite;
        public int RewardCount;
        
        public List<UIQuestInfoItem> UiQuestInfoItems = new();
    }
    
    public class UI_QuestMain : MonoBehaviour
    {
        [SerializeField] private Image StageIcon;
        [SerializeField] private TextMeshProUGUI StageDescription;
        [SerializeField] private TextMeshProUGUI ListQuestProgress;
        [SerializeField] private TextMeshProUGUI TMP_EntireStageQuestProgress;
        [SerializeField] private TextMeshProUGUI TMP_MainQuestProgress;
        [SerializeField] private Slider Slider_MainQuestProgress;
        [SerializeField] private Button Button_Reward;
        [SerializeField] private GameObject Effect_Reward;
        [SerializeField] private Image Image_RewardImage;
        [SerializeField] private TextMeshProUGUI Text_RewardCountText;
        [SerializeField] private List<UI_Panel_QuestInfoItem> uiPanelQuestInfoItems;

        public void RegisterReference()
        {
            Button_Reward.onClick.RemoveAllListeners();
            Button_Reward.onClick.AddListener(() => 
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent($"list_{GameManager.Instance.ES3Saver.CurrentListQuestIndex}_{GameManager.Instance.ES3Saver.CurrentStageLevel}_completed");
                QuestManager.Instance.GetNextQuest();
                gameObject.SetActive(false);
            });
        }
        
        public void UpdateMainQuestProgress(Sprite stageIcon, string stageDescription, int listQuestCurrentIndex, int listQuestMaxIndex, int questClearCount, int questTotalCount, float progressRatio, Sprite rewardSprite, int rewardCount)
        {
            StageIcon.sprite = stageIcon;
            StageDescription.text = stageDescription;
            ListQuestProgress.text = $"({listQuestCurrentIndex}/{listQuestMaxIndex})";
            TMP_MainQuestProgress.text = $"{questClearCount}/{questTotalCount}";
            Image_RewardImage.sprite = rewardSprite;
            Text_RewardCountText.text = $"{rewardCount}";
            Slider_MainQuestProgress.value = progressRatio;
            
            Button_Reward.interactable = questClearCount == questTotalCount;
            Effect_Reward.gameObject.SetActive(questClearCount == questTotalCount);
        }

        public void UpdateQuestInfoItems(List<UIQuestInfoItem> questInfoItems)
        {
            for (var i = 0; i < uiPanelQuestInfoItems.Count; i++)
            {
                if (i < questInfoItems.Count)
                {
                    uiPanelQuestInfoItems[i].Activate(questInfoItems[i]);
                    uiPanelQuestInfoItems[i].gameObject.SetActive(true);
                }
                else
                {
                    uiPanelQuestInfoItems[i].gameObject.SetActive(false);
                }
            }
        }

        public void EnableRewardButton()
        {
            Button_Reward.interactable = true;
            Effect_Reward.gameObject.SetActive(true);
        }
    }
}