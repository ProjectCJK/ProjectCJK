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

        private Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> _frontGroundImageCache;
        private List<CostumeItemData> _currentCostumeItemData;
        private UI_Panel_CostumeInfo _uiPanelCostumeInfo;

        private readonly List<UI_Panel_CostumeInventoryItem> _inventoryItems = new();

        private static string PoolKey => "CostumeInventoryItemPool";

        public void RegisterReference(
            Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCache,
            List<CostumeItemData> currentCostumeItemData,
            UI_Panel_CostumeInfo uiPanelCostumeInfo)
        {
            _frontGroundImageCache = frontGroundImageCache;
            _currentCostumeItemData = currentCostumeItemData;
            _uiPanelCostumeInfo = uiPanelCostumeInfo;

            ObjectPoolManager.Instance.CreatePool(PoolKey, 5, 99999, true, () => InstantiateInventoryItem(_costumeInventoryItemPrefab), _itemPrefabInstancePosition);
        }

        private UI_Panel_CostumeInventoryItem InstantiateInventoryItem(GameObject prefab)
        {
            GameObject obj = Instantiate(prefab, _itemPrefabInstancePosition);
            obj.transform.localScale = Vector3.one;
            var item = obj.GetComponent<UI_Panel_CostumeInventoryItem>();
            item.RegisterReference(_frontGroundImageCache, _uiPanelCostumeInfo);
            return item;
        }

        public void Activate()
        {
            foreach (var costumeItem in _currentCostumeItemData)
            {
                var item = ObjectPoolManager.Instance.GetObject<UI_Panel_CostumeInventoryItem>(PoolKey, null);
                _inventoryItems.Add(item);
                item.Activate(costumeItem);
            }

            gameObject.SetActive(true);
        }

        public void UpdateItems()
        {
            foreach (var item in _inventoryItems)
            {
                ObjectPoolManager.Instance.ReturnObject(PoolKey, item);
            }

            _inventoryItems.Clear();

            foreach (var costumeItem in _currentCostumeItemData)
            {
                var item = ObjectPoolManager.Instance.GetObject<UI_Panel_CostumeInventoryItem>(PoolKey, null);
                _inventoryItems.Add(item);

                item.transform.SetParent(_itemPrefabInstancePosition, false);
                item.transform.SetSiblingIndex(_inventoryItems.Count - 1);

                item.Activate(costumeItem);
            }
        }

        private void OnDisable()
        {
            foreach (var item in _inventoryItems)
            {
                ObjectPoolManager.Instance.ReturnObject(PoolKey, item);
            }

            _inventoryItems.Clear();
        }
    }
}