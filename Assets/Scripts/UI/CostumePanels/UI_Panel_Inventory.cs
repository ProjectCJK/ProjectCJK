using System;
using System.Collections.Generic;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using UnityEngine;

namespace UI.CostumePanels
{
    public class UI_Panel_Inventory : MonoBehaviour
    {
        [SerializeField] private GameObject _costumeItemPrefab;
        [SerializeField] private Transform _itemPrefabInstancePosition;
        
        private Dictionary<ECostumeType, CostumeItemData> _currentEquippedCostumeItemDatas;
        private Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> _frontGroundImageCache = new();
        
        private readonly List<UI_Panel_CostumeItem> _ui_Panel_CostumeItems = new();
        private UI_Panel_Popup _uiPanelPopup;
        
        private static string PoolKey => "CostumeItemPool";
        
        private List<CostumeItemData> _currentCostumeItemData;
        
        public void RegisterReference(
            Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCache,
            List<CostumeItemData> currentCostumeItemData,
            Dictionary<ECostumeType, CostumeItemData> currentEquippedCostumeItemDatas,
            UI_Panel_Popup uiPanelPopup)
        {
            _frontGroundImageCache = frontGroundImageCache;
            _currentCostumeItemData = currentCostumeItemData;
            _uiPanelPopup = uiPanelPopup;
            
            ObjectPoolManager.Instance.CreatePool(PoolKey, 5, 99999, true, () => InstantiateCostumeItem(_costumeItemPrefab), _itemPrefabInstancePosition);
            _currentEquippedCostumeItemDatas = currentEquippedCostumeItemDatas;
        }

        private UI_Panel_CostumeItem InstantiateCostumeItem(GameObject costumeItemPrefab)
        {
            GameObject obj = Instantiate(costumeItemPrefab, _itemPrefabInstancePosition, true);
            obj.transform.localScale = Vector3.one;
            var costumeItem = obj.GetComponent<UI_Panel_CostumeItem>();
            costumeItem.RegisterReference(_frontGroundImageCache, _uiPanelPopup);

            return costumeItem;
        }

        public void Activate()
        {
            foreach (CostumeItemData costumeItem in _currentCostumeItemData)
            {
                var item = ObjectPoolManager.Instance.GetObject<UI_Panel_CostumeItem>(PoolKey, null);
                _ui_Panel_CostumeItems.Add(item);
                item.Initialize(costumeItem);
            }
            
            gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            foreach (UI_Panel_CostumeItem uiPanelCostumeItem in _ui_Panel_CostumeItems)
            {
                ObjectPoolManager.Instance.ReturnObject(PoolKey, uiPanelCostumeItem);
            }
            
            _ui_Panel_CostumeItems.Clear();
        }
    }
}