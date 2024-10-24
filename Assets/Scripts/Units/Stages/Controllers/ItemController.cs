using System;
using System.Collections.Generic;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Items;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface IItemController
    {
        public IItem GetItem(Tuple<EMaterialType, EItemType> itemType, Vector3 initialPosition);
        public void ReturnItem(IItem item);
    }
    
    public class ItemController : IItemController
    {
        private readonly IItemFactory _itemFactory;
        private readonly string PoolKey;
        
        private Dictionary<Tuple<EMaterialType, EItemType>, Sprite> _itemSprites;
        
        public ItemController(IItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            PoolKey = itemFactory.PoolKey;

            CreateSpriteDictionary();
            InstantiateItem();
        }

        private void CreateSpriteDictionary()
        {
            List<ItemSprite> itemSprites = _itemFactory.ItemDataSo.ItemSprites;
            _itemSprites = new Dictionary<Tuple<EMaterialType, EItemType>, Sprite>();
            
            foreach (ItemSprite data in itemSprites)
            {
                var dicKey = new Tuple<EMaterialType, EItemType>(data.MaterialType, data.ItemType);
                _itemSprites.TryAdd(dicKey, data.Sprite);
            }
        }

        private void InstantiateItem()
        {
            _itemFactory.CreateItem();
        }

        public IItem GetItem(Tuple<EMaterialType, EItemType> itemType, Vector3 initializePosition)
        {
            var item = ObjectPoolManager.Instance.GetObject<IItem>(PoolKey, null);
            item.Initialize(_itemSprites[itemType], initializePosition);

            return item;
        }

        public void ReturnItem(IItem item)
        {
            ObjectPoolManager.Instance.ReturnObject(PoolKey, item);
        }
    }
}