using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Managers;
using UI.BuildingEnhancementPanel;
using UI.CostumeGachaPanels;
using UI.CostumePanels;
using UI.CurrencyPanel;
using UI.InventoryPanels;
using UI.Level;
using UI.LevelPanels;
using UI.QuestPanels;
using UI.StageMapPanel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainPanels
{
    [Serializable]
    public struct UI_Item_QuestGuide
    {
        public int index;
        public List<GameObject> items; 
    }
    
    public class UI_Panel_Main : MonoBehaviour
    {
        public LevelView LevelView;
        public CurrencyView CurrencyView;
        public InventoryView InventoryView;

        public UI_Panel_Quest UI_Panel_Quest;
        public UI_Panel_Costume UI_Panel_Costume;
        public UI_Panel_Costume_Gacha UI_Panel_Costume_Gacha;
        public UI_Panel_StageMap UI_Panel_StageMap;
        public UI_Panel_QuestClear UI_Panel_QuestClear;
        public UI_Panel_LevelUp UI_Panel_LevelUp;
        public UI_Panel_BuildingEnhancement UI_Panel_BuildingEnhancement;
        public UI_Panel_MainButtons UI_Panel_MainButtons;
        public UI_Panel_PopUp_Reward UI_Panel_PopUp_Reward;
        public UI_Panel_QuestClearThumbnail_PopUp UI_Panel_QuestClearThumbnail_PopUp;
        
        public List<UI_Item_QuestGuide> UI_Item_QuestGuide;

        public void RegisterReference()
        {
            if (GameManager.Instance.ES3Saver.CurrentStageLevel != 2)
            {
                foreach (GameObject item in UI_Item_QuestGuide.SelectMany(items => items.items))
                {
                    item.gameObject.SetActive(false);
                }
            
                foreach (Button button in UI_Panel_MainButtons.UI_Button_LockButton)
                {
                    button.gameObject.SetActive(true);
                }
            
                UI_Panel_MainButtons.UI_Button_CostumeGacha.gameObject.SetActive(false);
                UI_Panel_MainButtons.UI_Button_CostumePanel.gameObject.SetActive(false);
                UI_Panel_MainButtons.UI_Button_StageMap.gameObject.SetActive(false);
            }

            UI_Panel_MainButtons.RegisterReference();
        }
    }
}
