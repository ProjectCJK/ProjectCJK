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

        public void UpdateThumbnailQuest(UIListQuestInfoItem questDataBundle)
        {
            currentTargetQuestImage.sprite = questDataBundle.UiQuestInfoItems[0].QuestIconImage;
            
            if (questDataBundle.UiQuestInfoItems[0].CurrentProgressCount >= questDataBundle.UiQuestInfoItems[0].MaxProgressCount)
            {
                currentTargetQuestImage_Clear.gameObject.SetActive(true);
                currentTargetQuestImage_NotClear.gameObject.SetActive(false);
                
                if (questDataBundle.QuestClearCount + 1 >= questDataBundle.QuestTotalCount)
                {
                    currentMainQuestClearText.gameObject.SetActive(true);
                    currentTargetQuestImage.gameObject.SetActive(false);
                    currentTargetQuestDescriptionText.gameObject.SetActive(false);
                    currentTargetQuestDescriptionProgressText.gameObject.SetActive(false);
                    
                    currentMainQuestClearText.text = "Get your work reward!";   
                }
                else
                {
                    currentTargetQuestImage_Clear.gameObject.SetActive(true);
                    currentTargetQuestImage_NotClear.gameObject.SetActive(false);
                    
                    currentMainQuestClearText.gameObject.SetActive(false);
                    currentTargetQuestImage.gameObject.SetActive(true);
                    currentTargetQuestDescriptionText.gameObject.SetActive(true);
                    currentTargetQuestDescriptionProgressText.gameObject.SetActive(true);

                    currentTargetQuestDescriptionText.text = questDataBundle.UiQuestInfoItems[0].QuestDescriptionText;
                    currentTargetQuestDescriptionProgressText.text = $"( {questDataBundle.UiQuestInfoItems[0].CurrentProgressCount} / {questDataBundle.UiQuestInfoItems[0].MaxProgressCount} )";
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

                currentTargetQuestDescriptionText.text = questDataBundle.UiQuestInfoItems[0].QuestDescriptionText;
                currentTargetQuestDescriptionProgressText.text = $"( {questDataBundle.UiQuestInfoItems[0].CurrentProgressCount} / {questDataBundle.UiQuestInfoItems[0].MaxProgressCount} )";
            }
        }

        public void UpdateLastQuest()
        {
            currentTargetQuestImage_Clear.gameObject.SetActive(true);
            currentTargetQuestImage_NotClear.gameObject.SetActive(false);
            currentMainQuestClearText.gameObject.SetActive(true);
            currentTargetQuestImage.gameObject.SetActive(false);
            currentTargetQuestDescriptionText.gameObject.SetActive(false);
            currentTargetQuestDescriptionProgressText.gameObject.SetActive(false);

            currentMainQuestClearText.text = "Let's go to the next stage!";
        }
    }
}