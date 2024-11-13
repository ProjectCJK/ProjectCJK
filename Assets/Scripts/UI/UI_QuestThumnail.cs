using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_QuestThumnail : MonoBehaviour
    {
        [SerializeField] private Image currentTargetQuestImage;
        [SerializeField] private TextMeshProUGUI currentTargetQuestDescriptionText;
        [SerializeField] private TextMeshProUGUI currentTargetQuestDescriptionProgressText;
        [SerializeField] private TextMeshProUGUI currentMainQuestClearText;

        public void UpdateThumbnailQuest(string description, int currentGoal, int maxGoal)
        {
            if (currentGoal >= maxGoal)
            {
                currentMainQuestClearText.text = "모든 퀘스트 클리어!";
                currentMainQuestClearText.gameObject.SetActive(true);
                currentTargetQuestImage.gameObject.SetActive(false);
                currentTargetQuestDescriptionText.gameObject.SetActive(false);
                currentTargetQuestDescriptionProgressText.gameObject.SetActive(false);
            }
            else
            {
                currentMainQuestClearText.gameObject.SetActive(false);
                currentTargetQuestImage.gameObject.SetActive(true);
                currentTargetQuestDescriptionText.gameObject.SetActive(true);
                currentTargetQuestDescriptionProgressText.gameObject.SetActive(true);

                currentTargetQuestDescriptionText.text = description;
                currentTargetQuestDescriptionProgressText.text = $"( {currentGoal} / {maxGoal} )";
            }
        }
    }
}