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
        
        private static string PoolKey => "CostumeItemPool";
        
        private List<CostumeItemData> _currentCostumeItemData;
        
        public void RegisterReference(
            List<CostumeItemData> currentCostumeItemData,
            Dictionary<ECostumeType, CostumeItemData> currentEquippedCostumeItemDatas)
        {
            _currentCostumeItemData = currentCostumeItemData;
            
            ObjectPoolManager.Instance.CreatePool(PoolKey, 5, 10, true, () => InstantiateCostumeItem(_costumeItemPrefab));
            _currentEquippedCostumeItemDatas = currentEquippedCostumeItemDatas;
        }

        private UI_Panel_CostumeItem InstantiateCostumeItem(GameObject costumeItemPrefab)
        {
            GameObject obj = Instantiate(costumeItemPrefab, _itemPrefabInstancePosition, true);
            obj.transform.localScale = Vector3.one;
            var costumeItem = obj.GetComponent<UI_Panel_CostumeItem>();

            return costumeItem;
        }

        public void Activate()
        {
            foreach (var costumeItem in _currentCostumeItemData)
            {
                var item = ObjectPoolManager.Instance.GetObject<UI_Panel_CostumeItem>(PoolKey, null);
                item.Initialize(costumeItem);
            }
            
            gameObject.SetActive(true);
        }
    }
}