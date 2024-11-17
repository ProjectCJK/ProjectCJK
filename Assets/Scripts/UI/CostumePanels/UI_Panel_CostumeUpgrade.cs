using System;
using System.Collections.Generic;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CostumePanels
{
    public class UI_Panel_CostumeUpgrade : MonoBehaviour
    {
        [SerializeField] private GameObject _costumeUpgradeItemPrefab;
        [SerializeField] private Transform _itemPrefabInstancePosition;
        
        [Space(20), SerializeField] private Image costumeBackground;
        [SerializeField] private Image costumeIcon;

        private static string PoolKey => "CostumeUpgradeItemPool";
        private readonly List<UI_Panel_CostumeUpgradeItem> _ui_Panel_CostumeUpgradeItems = new();
        private Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> _frontGroundImageCache;
        private List<CostumeItemData> _currentCostumeItemData;
        
        public void RegisterReference(
            Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCachePara,
            List<CostumeItemData> currentCostumeItemData)
        {
            gameObject.SetActive(false);
            
            _frontGroundImageCache = frontGroundImageCachePara;
            _currentCostumeItemData = currentCostumeItemData;
            ObjectPoolManager.Instance.CreatePool(PoolKey, 5, 99999, true, () => InstantiateCostumeUpgradeItem(_costumeUpgradeItemPrefab), _itemPrefabInstancePosition);
        }
        
        private UI_Panel_CostumeUpgradeItem InstantiateCostumeUpgradeItem(GameObject costumeItemPrefab)
        {
            GameObject obj = Instantiate(costumeItemPrefab, _itemPrefabInstancePosition, true);
            obj.transform.localScale = Vector3.one;
            var costumeItem = obj.GetComponent<UI_Panel_CostumeUpgradeItem>();
            costumeItem.RegisterReference(_frontGroundImageCache);

            return costumeItem;
        }
        
        public void Activate(CostumeItemData costumeItemData)
        {
            List<CostumeItemData> filteredCostumes = _currentCostumeItemData.FindAll(item => item.CostumeType == costumeItemData.CostumeType);

            foreach (CostumeItemData costumeItem in filteredCostumes)
            {
                var item = ObjectPoolManager.Instance.GetObject<UI_Panel_CostumeUpgradeItem>(PoolKey, null);
                _ui_Panel_CostumeUpgradeItems.Add(item);
                item.Activate(costumeItem, null);
            }

            costumeBackground.sprite = _frontGroundImageCache[new Tuple<ECostumeType, ECostumeGrade>(costumeItemData.CostumeType, costumeItemData.CostumeGrade)];
            costumeIcon.sprite = costumeItemData.CostumeSprites[0];

            gameObject.SetActive(true);
        }
        
        private void OnDisable()
        {
            foreach (UI_Panel_CostumeUpgradeItem uiPanelCostumeItem in _ui_Panel_CostumeUpgradeItems)
            {
                ObjectPoolManager.Instance.ReturnObject(PoolKey, uiPanelCostumeItem);
            }
            
            _ui_Panel_CostumeUpgradeItems.Clear();
        }
    }
}