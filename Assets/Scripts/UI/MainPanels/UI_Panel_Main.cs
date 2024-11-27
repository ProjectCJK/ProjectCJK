using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        
        public List<UI_Item_QuestGuide> UI_Item_QuestGuide;

        public void RegisterReference()
        {
            foreach (GameObject item in UI_Item_QuestGuide.SelectMany(items => items.items))
            {
                item.gameObject.SetActive(false);
            }
            
            foreach (Button button in UI_Panel_MainButtons.UI_Button_LockButton)
            {
                button.gameObject.SetActive(true);
            }
            
            UI_Panel_MainButtons.UI_Button_CostumeGachaPanel.gameObject.SetActive(false);
            UI_Panel_MainButtons.UI_Button_CostumePanel.gameObject.SetActive(false);
            UI_Panel_MainButtons.UI_Button_StageMap.gameObject.SetActive(false);
        }
        
        public void OnActivateUIByCurrentTutorialIndex(int index)
        {
            foreach (GameObject item in UI_Item_QuestGuide.SelectMany(items => items.items))
            {
                item.gameObject.SetActive(false);
            }
            
            for (var i = 0; i < UI_Item_QuestGuide.Count; i++)
            {
                if (UI_Item_QuestGuide[i].index == index)
                {
                    foreach (GameObject item in UI_Item_QuestGuide[i].items)
                    {
                        item.gameObject.SetActive(true);
                    }
                }
            }

            switch (index)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    UI_Panel_MainButtons.UI_Button_CostumeGachaPanel.gameObject.SetActive(true);
                    break;
                case 10:
                    UI_Panel_MainButtons.UI_Button_CostumeGachaPanel.gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_CostumePanel.gameObject.SetActive(true);

                    UI_Panel_MainButtons.UI_Button_LockButton[0].gameObject.SetActive(false);
                    UI_Panel_MainButtons.UI_Button_LockButton[1].gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_LockButton[2].gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_LockButton[3].gameObject.SetActive(true);

                    break;
                case 15:
                    UI_Panel_MainButtons.UI_Button_CostumeGachaPanel.gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_CostumePanel.gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_StageMap.gameObject.SetActive(true);

                    UI_Panel_MainButtons.UI_Button_LockButton[0].gameObject.SetActive(false);
                    UI_Panel_MainButtons.UI_Button_LockButton[1].gameObject.SetActive(false);
                    UI_Panel_MainButtons.UI_Button_LockButton[2].gameObject.SetActive(true);
                    UI_Panel_MainButtons.UI_Button_LockButton[3].gameObject.SetActive(true);

                    break;
            }
        }

        public void OnInactivateUIByCurrentTutorialIndex(int index)
        {
            foreach (GameObject item in UI_Item_QuestGuide.Where(guide => index == guide.index).SelectMany(guide => guide.items))
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}
