using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.QuestPanels
{
    public class UI_QuestThumnail : MonoBehaviour
    {
        [SerializeField] private Image currentTargetQuestImage_NotClear;
        [SerializeField] private Image currentTargetQuestImage_Clear;
        [SerializeField] private Image currentTargetQuestImage;
        [SerializeField] private TextMeshProUGUI currentTargetQuestDescriptionText;
        [SerializeField] private TextMeshProUGUI currentTargetQuestDescriptionProgressText;
        [SerializeField] private TextMeshProUGUI currentMainQuestClearText;

        public void UpdateThumbnailQuest(QuestDataBundle questDataBundle)
        {
            if (questDataBundle.ThumbnailCurrentGoal >= questDataBundle.ThumbnailMaxGoal)
            {
                currentTargetQuestImage_Clear.gameObject.SetActive(true);
                currentTargetQuestImage_NotClear.gameObject.SetActive(false);
                
                if (questDataBundle.ClearedCount + 1 >= questDataBundle.TotalCount)
                {
                    currentMainQuestClearText.gameObject.SetActive(true);
                    currentTargetQuestImage.gameObject.SetActive(false);
                    currentTargetQuestDescriptionText.gameObject.SetActive(false);
                    currentTargetQuestDescriptionProgressText.gameObject.SetActive(false);
                    
                    currentMainQuestClearText.text = "모든 퀘스트 클리어!";   
                }
                else
                {
                    currentTargetQuestImage_Clear.gameObject.SetActive(true);
                    currentTargetQuestImage_NotClear.gameObject.SetActive(false);
                    
                    currentMainQuestClearText.gameObject.SetActive(false);
                    currentTargetQuestImage.gameObject.SetActive(true);
                    currentTargetQuestDescriptionText.gameObject.SetActive(true);
                    currentTargetQuestDescriptionProgressText.gameObject.SetActive(true);
                    
                    currentTargetQuestDescriptionText.text = questDataBundle.ThumbnailDescription;
                    currentTargetQuestDescriptionProgressText.text = $"( {questDataBundle.ThumbnailCurrentGoal} / {questDataBundle.ThumbnailMaxGoal} )";
                }
            }
            else
            {
                currentTargetQuestImage_Clear.gameObject.SetActive(false);
                currentTargetQuestImage_NotClear.gameObject.SetActive(true);
                
                currentMainQuestClearText.gameObject.SetActive(false);
                currentTargetQuestImage.gameObject.SetActive(true);
                currentTargetQuestDescriptionText.gameObject.SetActive(true);
                currentTargetQuestDescriptionProgressText.gameObject.SetActive(true);

                currentTargetQuestDescriptionText.text = questDataBundle.ThumbnailDescription;
                currentTargetQuestDescriptionProgressText.text = $"( {questDataBundle.ThumbnailCurrentGoal} / {questDataBundle.ThumbnailMaxGoal} )";
            }
        }
    }
}