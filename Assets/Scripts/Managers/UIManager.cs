using System;
using Modules.DesignPatterns.Singletons;
using TMPro;
using UI;
using UI.UI;
using UnityEngine;

namespace Managers
{
    public class UIManager : SingletonMono<UIManager>
    {
        public UI_BuildingEnhancement UIBuildingEnhancement { get; set; }
        
        public void GetPanelBuildingEnhancement(UIBuildingEnhancementData uiBuildingEnhancementData)
        {
            UIBuildingEnhancement.Activate(uiBuildingEnhancementData);
        }

        public void ReturnPanelBuildingEnhancement()
        {
            UIBuildingEnhancement.Inactivate();
        }
    }
}