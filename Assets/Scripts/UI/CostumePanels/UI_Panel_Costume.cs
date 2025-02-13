using System;
using System.Collections.Generic;
using Managers;
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
            uiPanelCostumeInfo.RegisterReference(frontGroundImageCache, currentCostumeItemData, uiPanelCostumeInventory, uiPanelCurrentEquippedCostumeInfo);
        }

        public void Activate()
        {
            uiPanelCurrentEquippedCostumeInfo.Activate();
            uiPanelCostumeInventory.Activate();
            gameObject.SetActive(true);
        }
    }
}