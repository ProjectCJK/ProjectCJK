using UnityEngine;
using UI;
using Managers;

namespace UI
{
    public class UI_Panel_Quest : MonoBehaviour
    {
        [SerializeField] private UI_QuestThumnail _thumbnailQuest;
        [SerializeField] private UI_QuestMain _mainQuest;
        
        public UI_QuestThumnail ThumbnailQuest => _thumbnailQuest;
        public UI_QuestMain MainQuest => _mainQuest;
        
        public void UpdateQuestPanel(QuestDataBundle questDataBundle)
        {
            _thumbnailQuest.UpdateThumbnailQuest(questDataBundle);

            _mainQuest.UpdateMainQuestProgress(
                questDataBundle.ClearedCount,
                questDataBundle.TotalCount,
                questDataBundle.ProgressRatio,
                questDataBundle.RewardCount
            );

            _mainQuest.UpdateQuestInfoItems(questDataBundle.QuestInfoItems);
            _mainQuest.SetAdvanceQuestAction(questDataBundle.AdvanceToNextQuestAction);
        }
    }
}