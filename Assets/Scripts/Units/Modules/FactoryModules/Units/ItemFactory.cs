using System;
using System.Collections.Generic;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Items;
using Units.Modules.FactoryModules.Abstract;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Units.Modules.FactoryModules.Units
{
    public interface IItemFactory
    {
        public ItemDataSO ItemDataSo { get; }
        IItem GetItem(Tuple<EMaterialType, EItemType> itemType, Vector3 initializePosition);
        void ReturnItem(IItem item);
    }
    
    public class ItemFactory : Factory, IItemFactory
    {
        public ItemDataSO ItemDataSo => DataManager.Instance.ItemData;
        public string PoolKey => "ItemPool";
        
        private Dictionary<Tuple<EMaterialType, EItemType>, Sprite> _itemSprites;
        
        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;

        public ItemFactory()
        {
            CreateItemPools();
            CreateSpriteDictionary();
        }

        public IItem GetItem(Tuple<EMaterialType, EItemType> itemType, Vector3 initializePosition)
        {
            var item = ObjectPoolManager.Instance.GetObject<IItem>(PoolKey, null);
            item.Initialize(itemType, _itemSprites[itemType], initializePosition);

            return item;
        }

        public void ReturnItem(IItem item)
        {
            ObjectPoolManager.Instance.ReturnObject(PoolKey, item);
        }
        
        private void CreateItemPools()
        {
            ObjectPoolManager.Instance.CreatePool(PoolKey, DefaultPoolSize, MaxPoolSize, true, () => OnInstantiateItem(ItemDataSo.prefab));
        }
        
        private IItem OnInstantiateItem(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            var item = obj.GetComponent<IItem>();
            
            item.RegisterReference(ItemDataSo);
            
            return item;
        }
        
        private void CreateSpriteDictionary()
        {
            List<ItemSprite> itemSprites = ItemDataSo.ItemSprites;
            _itemSprites = new Dictionary<Tuple<EMaterialType, EItemType>, Sprite>();
            
            foreach (ItemSprite data in itemSprites)
            {
                var dicKey = new Tuple<EMaterialType, EItemType>(data.MaterialType, data.ItemType);
                _itemSprites.TryAdd(dicKey, data.Sprite);
            }
        }
    }
}