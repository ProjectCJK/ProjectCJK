using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace UI.CostumePanels
{
    public class UI_Panel_Costume : MonoBehaviour
    {
        [SerializeField] private UI_Panel_CurrentEquipmentInfo _uiPanelCurrentEquipmentInfo;
        [SerializeField] private UI_Panel_Inventory _uiPanelInventory;
        [SerializeField] private UI_Panel_Popup _uiPanelPopup;
        
        public void Activate()
        {
            _uiPanelCurrentEquipmentInfo.Activate();
            _uiPanelInventory.Activate();
            gameObject.SetActive(true);
        }

        public void RegisterReference(
            List<CostumeItemData> currentCostumeItemData,
            Dictionary<ECostumeType, CostumeItemData> currentEquippedCostumeItemDatas)
        {
            _uiPanelCurrentEquipmentInfo.RegisterReference(currentEquippedCostumeItemDatas);
            _uiPanelInventory.RegisterReference(currentCostumeItemData, currentEquippedCostumeItemDatas);
        }
    }
}