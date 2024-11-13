using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Managers;

namespace UI
{
    public class UI_QuestMain : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI TMP_EntireStageQuestProgress;
        [SerializeField] private TextMeshProUGUI TMP_MainQuestProgress;
        [SerializeField] private Slider Slider_MainQuestProgress;
        [SerializeField] private Button Button_Reward;
        [SerializeField] private TextMeshProUGUI Text_RewardCountText;
        [SerializeField] private List<UI_Panel_QuestInfoItem> uiPanelQuestInfoItems;

        public void UpdateMainQuestProgress(int clearedCount, int totalCount, float progressRatio, int rewardCount)
        {
            TMP_MainQuestProgress.text = $"{clearedCount}/{totalCount}";
            Text_RewardCountText.text = $"{rewardCount}";
            Slider_MainQuestProgress.value = progressRatio;
        }

        public void UpdateQuestInfoItems(List<UIQuestInfoItem> questInfoItems)
        {
            for (int i = 0; i < uiPanelQuestInfoItems.Count; i++)
            {
                if (i < questInfoItems.Count)
                {
                    uiPanelQuestInfoItems[i].Activate(questInfoItems[i], i);
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
            Button_Reward.gameObject.SetActive(true);
        }

        public void SetAdvanceQuestAction(Action advanceQuestAction)
        {
            Button_Reward.onClick.RemoveAllListeners();
            Button_Reward.onClick.AddListener(() => 
            {
                advanceQuestAction();
                Button_Reward.gameObject.SetActive(false);
            });
        }
    }
}