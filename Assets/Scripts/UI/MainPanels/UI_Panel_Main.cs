using System;
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

        public void OnActivateUIByCurrentTutorialIndex(int index)
        {
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
            }
        }
    }
}
