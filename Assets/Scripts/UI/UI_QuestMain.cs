using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_QuestMain : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI TMP_EntireStageQuestProgress;
        [SerializeField] private TextMeshProUGUI TMP_MainQuestProgress;
        [SerializeField] private Slider Slider_MainQuestProgress;
        
        public void Activate(UIQuestInfoItem uiQuestInfoItem)
        {
            
        }
        
        public void Inactivate()
        {
            
        }
    }
}