using System;
using System.Collections.Generic;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Structs;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Items.Enums;
using Units.Stages.Items.Units;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface IItemController
    {
        public void InstantiateItem();
        public void TransferItem(Tuple<EMaterialType, EProductType> itemType, Vector3 sendPosition, Transform receivePosition);
        public void ReturnItem(Item item);
    }
    
    public class ItemController : IItemController
    {
        private readonly IItemFactory _itemFactory;
        private readonly string PoolKey;
        
        private Dictionary<Tuple<EMaterialType, EProductType>, Sprite> _itemSprites;
        
        public ItemController(IItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            PoolKey = itemFactory.PoolKey;

            CreateItemSpriteDictionary(itemFactory.ItemDataSo.itemSprites);
        }

        private void CreateItemSpriteDictionary(List<ProductSprite> itemSprites)
        {
            _itemSprites = new Dictionary<Tuple<EMaterialType, EProductType>, Sprite>();
            
            foreach (ProductSprite data in itemSprites)
            {
                var dicKey = new Tuple<EMaterialType, EProductType>(data.MaterialType, data.ProductType);
                _itemSprites.TryAdd(dicKey, data.Sprite);
            }
        }

        public void InstantiateItem()
        {
            _itemFactory.CreateItem();
        }

        public void TransferItem(Tuple<EMaterialType, EProductType> itemType, Vector3 sendPosition,
            Transform receivePosition)
        {
            var item = ObjectPoolManager.Instance.GetObject<Item>(PoolKey, null);
            item.Initialize(_itemSprites[itemType], sendPosition, receivePosition, () => ReturnItem(item));
        }

        public void ReturnItem(Item item)
        {
            ObjectPoolManager.Instance.ReturnObject(PoolKey, item);
        }
    }
}