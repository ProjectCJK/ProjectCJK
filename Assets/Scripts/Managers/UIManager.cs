using System;
using Modules.DesignPatterns.Singletons;
using TMPro;
using UI;
using UI.UI;
using Units.Stages.UI;
using UnityEngine;

namespace Managers
{
    public class UIManager : SingletonMono<UIManager>
    {
        public UI_Panel_Quest UI_Panel_Quest;
        public UI_Panel_BuildingEnhancement UI_Panel_BuildingEnhancement;
        public UI_Panel_Currency UI_Panel_Currency;
        
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