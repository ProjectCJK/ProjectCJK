using System;
using System.Collections.Generic;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using UnityEngine;

namespace UI.CostumePanels
{
    public class UI_Panel_CostumeInventory : MonoBehaviour
    {
        [SerializeField] private GameObject _costumeInventoryItemPrefab;
        [SerializeField] private Transform _itemPrefabInstancePosition;
        
        private Dictionary<ECostumeType, CostumeItemData> _currentEquippedCostumeItemDatas;
        private Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> _frontGroundImageCache = new();
        
        private readonly List<UI_Panel_CostumeInventoryItem> _ui_Panel_CostumeInventoryItems = new();
        private UI_Panel_CostumeInfo _uiPanelCostumeInfo;
        
        private static string PoolKey => "CostumeInventoryItemPool";
        
        private List<CostumeItemData> _currentCostumeItemData;
        
        public void RegisterReference(
            Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCache,
            List<CostumeItemData> currentCostumeItemData,
            UI_Panel_CostumeInfo uiPanelCostumeInfo)
        {
            _frontGroundImageCache = frontGroundImageCache;
            _currentCostumeItemData = currentCostumeItemData;
            _uiPanelCostumeInfo = uiPanelCostumeInfo;

            _uiPanelCostumeInfo.RegisterReference(_frontGroundImageCache, currentCostumeItemData);
            
            ObjectPoolManager.Instance.CreatePool(PoolKey, 5, 99999, true, () => InstantiateCostumeInventoryItem(_costumeInventoryItemPrefab), _itemPrefabInstancePosition);
        }

        private UI_Panel_CostumeInventoryItem InstantiateCostumeInventoryItem(GameObject costumeItemPrefab)
        {
            GameObject obj = Instantiate(costumeItemPrefab, _itemPrefabInstancePosition, true);
            obj.transform.localScale = Vector3.one;
            var costumeItem = obj.GetComponent<UI_Panel_CostumeInventoryItem>();
            costumeItem.RegisterReference(_frontGroundImageCache, _uiPanelCostumeInfo);

            return costumeItem;
        }

        public void Activate()
        {
            foreach (CostumeItemData costumeItem in _currentCostumeItemData)
            {
                var item = ObjectPoolManager.Instance.GetObject<UI_Panel_CostumeInventoryItem>(PoolKey, null);
                _ui_Panel_CostumeInventoryItems.Add(item);
                item.Activate(costumeItem);
            }
            
            gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            foreach (UI_Panel_CostumeInventoryItem uiPanelCostumeItem in _ui_Panel_CostumeInventoryItems)
            {
                ObjectPoolManager.Instance.ReturnObject(PoolKey, uiPanelCostumeItem);
            }
            
            _ui_Panel_CostumeInventoryItems.Clear();
        }

        public void UpdateItems()
        {
            for (var i = 0; i < _currentCostumeItemData.Count; i++)
            {
                _ui_Panel_CostumeInventoryItems[i].Activate(_currentCostumeItemData[i]);
            }
        }
    }
}