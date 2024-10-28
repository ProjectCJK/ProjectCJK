using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Items;
using Units.Modules.FactoryModules.Abstract;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Modules.FactoryModules.Units
{
    public interface IItemFactory
    {
        public ItemDataSO ItemDataSo { get; }
        public string PoolKey { get; }
        public void CreateItem();
    }
    
    public class ItemFactory : Factory, IItemFactory
    {
        public ItemDataSO ItemDataSo => DataManager.Instance.ItemData;
        public string PoolKey => "ItemPool";
        
        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;

        public void CreateItem()
        {
            ObjectPoolManager.Instance.CreatePool(PoolKey, DefaultPoolSize, MaxPoolSize, true, () => InstantiateItem(ItemDataSo.prefab));
        }
        
        private IItem InstantiateItem(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            var item = obj.GetComponent<IItem>();
            
            item.RegisterReference(ItemDataSo);
            
            return item;
        }
    }
}