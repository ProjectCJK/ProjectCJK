using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [Serializable]
    public class UIQuestItem
    {
        public Sprite QuestIconBackgroundImage;
        public Sprite QuestIconImage;
        public string QuestDescriptionText;
        public string Reward1CountText;
        public string Reward2CountText;
        public string QuestProgressText;

        public int CurrentProgressCount;
        public int MaxProgressCount;
        
        public List<UIQuestInfoItem> UIQuestInfoItems;
    }
    
    public class UI_Panel_Quest : MonoBehaviour
    {
        [SerializeField] private UI_QuestThumnail _thumbnailQuest;
        [SerializeField] private UI_QuestMain _mainQuest;
        
        public UI_QuestThumnail ThumbnailQuest => _thumbnailQuest;
        public UI_QuestMain MainQuest => _mainQuest;
        
        public void Activate(UIQuestItem uiQuestItem)
        {
            
        }
        
        public void Inactivate()
        {
            
        }
    }
}
