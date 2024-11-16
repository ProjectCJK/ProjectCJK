using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace UI.CostumePanels
{
    public class UI_Panel_CurrentEquipmentInfo : MonoBehaviour
    {
        private Dictionary<ECostumeType, CostumeItemData> _currentEquippedCostumeItemDatas;
        
        public void RegisterReference(Dictionary<ECostumeType, CostumeItemData> currentEquippedCostumeItemDatas)
        {
            _currentEquippedCostumeItemDatas = currentEquippedCostumeItemDatas;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }
    }
}