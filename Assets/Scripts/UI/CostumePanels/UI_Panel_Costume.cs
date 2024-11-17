using System;
using System.Collections.Generic;
using Managers;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace UI.CostumePanels
{
    public class UI_Panel_Costume : MonoBehaviour
    {
        [SerializeField] private UI_Panel_CurrentEquippedCostumeInfo uiPanelCurrentEquippedCostumeInfo;
        [SerializeField] private UI_Panel_CostumeInventory uiPanelCostumeInventory;
        [SerializeField] private UI_Panel_CostumeInfo uiPanelCostumeInfo;
        
        public void RegisterReference(
            Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCache,
            List<CostumeItemData> currentCostumeItemData)
        {
            uiPanelCostumeInventory.RegisterReference(frontGroundImageCache, currentCostumeItemData, uiPanelCostumeInfo);
        }
        
        public void Activate()
        {
            uiPanelCurrentEquippedCostumeInfo.Activate();
            uiPanelCostumeInventory.Activate();
            gameObject.SetActive(true);
        }
    }
}