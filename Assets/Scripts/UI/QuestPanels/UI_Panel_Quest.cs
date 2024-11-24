using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.QuestPanels
{
    public class UI_Panel_Quest : MonoBehaviour
    {
        [SerializeField] private UI_QuestThumnail _thumbnailQuest;
        [SerializeField] private UI_QuestMain _mainQuest;
        
        public UI_QuestThumnail ThumbnailQuest => _thumbnailQuest;
        public UI_QuestMain MainQuest => _mainQuest;

        public void RegisterReference()
        {
            _mainQuest.RegisterReference();
        }
        
        public void UpdateQuestPanel(UIListQuestInfoItem questDataBundle)
        {
            ThumbnailQuest.GetComponent<Button>().onClick.RemoveAllListeners();
            ThumbnailQuest.GetComponent<Button>().onClick.AddListener(() => MainQuest.gameObject.SetActive(true));
            
            _thumbnailQuest.UpdateThumbnailQuest(questDataBundle);

            _mainQuest.UpdateMainQuestProgress(
                questDataBundle.StageIcon,
                questDataBundle.StageDescription,
                questDataBundle.ListQuestCurrentIndex,
                questDataBundle.ListQuestMaxIndex,
                questDataBundle.QuestClearCount,
                questDataBundle.QuestTotalCount,
                questDataBundle.ProgressRatio,
                questDataBundle.RewardSprite,
                questDataBundle.RewardCount
            );

            _mainQuest.UpdateQuestInfoItems(questDataBundle.UiQuestInfoItems);
        }

        public void UpdateStageLastQuestPanel()
        {
            ThumbnailQuest.GetComponent<Button>().onClick.RemoveAllListeners();
            ThumbnailQuest.GetComponent<Button>().onClick.AddListener(() => UIManager.Instance.UI_Panel_Main.UI_Panel_StageMap.Activate());
            
            _thumbnailQuest.UpdateLastQuest();
        }
    }
}