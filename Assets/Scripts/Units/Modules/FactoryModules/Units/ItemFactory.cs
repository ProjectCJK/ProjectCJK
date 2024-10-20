using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Modules.FactoryModules.Abstract;
using Units.Stages.Items.Units;
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
        public ItemDataSO ItemDataSo { get; }
        string IItemFactory.PoolKey => "ItemPool";
        
        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;

        public ItemFactory(ItemDataSO itemDataSo)
        {
            ItemDataSo = itemDataSo;
        }

        public void CreateItem()
        {
            ObjectPoolManager.Instance.CreatePool(((IItemFactory)this).PoolKey, DefaultPoolSize, MaxPoolSize, true, () => InstantiateItem(ItemDataSo.itemPrefab));
        }
        
        private Item InstantiateItem(Item itemPrefab)
        {
            GameObject obj = Object.Instantiate(itemPrefab.gameObject);
            var item = obj.GetComponent<Item>();
            
            item.RegisterReference(ItemDataSo);
            
            return item;
        }
    }
}