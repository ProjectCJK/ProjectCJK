using System;
using Modules.DesignPatterns.Singletons;
using TMPro;
using UI;
using UI.BuildingEnhancementPanel;
using UI.BuildingEnhancementPanel.UI;
using UI.CostumeGachaPanels;
using UI.CostumePanels;
using UI.CurrencyPanel;
using UI.QuestPanels;
using UI.StageMapPanel;
using Units.Stages.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : SingletonMono<UIManager>
    {
        public Button Button_CostumeGachaPanel;
        public Button Button_CostumePanel;
        public Button Button_StageMap;
        
        public UI_Panel_Quest UI_Panel_Quest;
        public UI_Panel_BuildingEnhancement UI_Panel_BuildingEnhancement;
        public UI_Panel_Costume UI_Panel_Costume;
        public UI_Panel_Currency UI_Panel_Currency;
        public UI_Panel_Costume_Gacha UI_Panel_CostumeGacha;
        public UI_Panel_StageMap UI_Panel_StageMap;
        
        public void GetPanelBuildingEnhancement(UIBuildingEnhancementData uiBuildingEnhancementData)
        {
            UI_Panel_BuildingEnhancement.Activate(uiBuildingEnhancementData);
        }

        public void ReturnPanelBuildingEnhancement()
        {
            UI_Panel_BuildingEnhancement.Inactivate();
        }
    }
}