using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.QuestPanels
{
    [Serializable]
    public class UIQuestInfoItem
    {
        public Sprite QuestIconBackgroundImage;
        public Sprite QuestIconImage;
        public string QuestDescriptionText;
        public Sprite Reward1IconImage;
        public string Reward1CountText;
        public Sprite Reward2IconImage;
        public string Reward2CountText;
        public string QuestProgressText;
        public int CurrentProgressCount;
        public int MaxProgressCount;
    }

    public class UI_Panel_QuestInfoItem : MonoBehaviour
    {
        [SerializeField] private Image _questIconBackgroundImage;
        [SerializeField] private Image _questIconImage;
        [SerializeField] private TextMeshProUGUI _questDescriptionText;
        [SerializeField] private Image _reward1IconImage;
        [SerializeField] private TextMeshProUGUI _reward1CountText;
        [SerializeField] private Image _reward2IconImage;
        [SerializeField] private TextMeshProUGUI _reward2CountText;
        [SerializeField] private TextMeshProUGUI _questProgressText;
        [SerializeField] private Button _buttonClearActive;
        [SerializeField] private Button _buttonClearInactive;
        [SerializeField] private Button _buttonNotClear;

        private int _questIndex;

        public void Activate(UIQuestInfoItem uiQuestInfoItem, int questIndex)
        {
            // _questIconBackgroundImage.sprite = uiQuestInfoItem.QuestIconBackgroundImage;
            _questIconImage.sprite = uiQuestInfoItem.QuestIconImage;
            _questDescriptionText.text = uiQuestInfoItem.QuestDescriptionText;
            _reward1IconImage.sprite = uiQuestInfoItem.Reward1IconImage;
            _reward1CountText.text = $"x{uiQuestInfoItem.Reward1CountText}";
            _reward2IconImage.sprite = uiQuestInfoItem.Reward2IconImage;
            _reward2CountText.text = $"x{uiQuestInfoItem.Reward2CountText}";
            _questProgressText.text = uiQuestInfoItem.QuestProgressText;

            _questIndex = questIndex;

            if (uiQuestInfoItem.CurrentProgressCount >= uiQuestInfoItem.MaxProgressCount)
            {
                if (QuestManager.Instance.IsQuestClear[_questIndex] == false)
                {
                    _buttonClearActive.gameObject.SetActive(true);
                    _buttonClearInactive.gameObject.SetActive(false);
                }
                else
                {
                    _buttonClearActive.gameObject.SetActive(false);
                    _buttonClearInactive.gameObject.SetActive(true);
                }
       
                _buttonNotClear.gameObject.SetActive(false);
            }
            else
            {
                _buttonClearActive.gameObject.SetActive(false);
                _buttonClearInactive.gameObject.SetActive(false);
                _buttonNotClear.gameObject.SetActive(true);
            }
        }

        public void OnClearButtonClick()
        {
            _buttonClearActive.gameObject.SetActive(false);
            _buttonNotClear.gameObject.SetActive(false);
            _buttonClearInactive.gameObject.SetActive(true);
            QuestManager.Instance.MarkQuestAsCleared(_questIndex);
        }
    }
}