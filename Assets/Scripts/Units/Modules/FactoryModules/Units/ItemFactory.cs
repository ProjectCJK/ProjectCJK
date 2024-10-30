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
        public IItem GetItem(string itemType, Vector3 initializePosition);
        public void ReturnItem(IItem item);
    }
    
    public class ItemFactory : Factory, IItemFactory
    {
        public ItemDataSO ItemDataSo => DataManager.Instance.ItemData;
        
        private Dictionary<string, Sprite> _itemSprites;
     
        private readonly Transform _parentTransform;
        
        private static string PoolKey => "ItemPool";
        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;

        public ItemFactory(Transform parentTransform)
        {
            _parentTransform = parentTransform;
            CreateItemPools();
            CreateSpriteDictionary();
        }

        public IItem GetItem(string itemType, Vector3 initializePosition)
        {
            var item = ObjectPoolManager.Instance.GetObject<IItem>(PoolKey, null);
            item.Initialize(itemType, _itemSprites[itemType], initializePosition);

            return item;
        }

        public void ReturnItem(IItem item)
        {
            if (!ReferenceEquals(item.Transform.parent, _parentTransform)) item.Transform.SetParent(_parentTransform);
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
            _itemSprites = new Dictionary<string, Sprite>();
            
            foreach (ItemSprite data in itemSprites)
            {
                var dicKey = EnumParserModule.ParseEnumToString(data.ItemType, data.MaterialType);
                _itemSprites.TryAdd(dicKey, data.Sprite);
            }
        }
    }
}