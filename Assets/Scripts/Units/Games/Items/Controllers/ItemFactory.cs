using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Games.Items.Units;
using UnityEngine;

namespace Units.Games.Items.Controllers
{
    public interface IItemFactory
    {
        public ItemDataSO ItemDataSo { get; }
        public string PoolKey { get; }
        public void CreateItem();
    }
    
    public class ItemFactory : IItemFactory
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