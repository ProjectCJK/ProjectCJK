using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Items;
using Units.Modules.FactoryModules.Abstract;
using Units.Stages.Controllers;
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
        private Dictionary<string, Sprite> _currencySprites;
     
        private readonly Transform _parentTransform;
        private readonly Dictionary<EMaterialType, EStageMaterialType> _materialMappings;

        private static string PoolKey => "ItemPool";
        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;

        public ItemFactory(Transform parentTransform, List<MaterialMapping> materialMappings)
        {
            _parentTransform = parentTransform;
            _materialMappings = ListParerModule.ConvertListToDictionary(materialMappings, key => key.MaterialType, value => value.StageMaterialType);
            CreateItemPools();
            CreateSpriteDictionary();
        }
        
        public IItem GetItem(string itemType, Vector3 initializePosition)
        {
            var item = ObjectPoolManager.Instance.GetObject<IItem>(PoolKey, null);

            (EItemType? _itemType, EMaterialType? _materialType) = EnumParserModule.ParseStringToEnum<EItemType, EMaterialType>(itemType);

            if (_itemType != null && _materialType != null)
            {
                EStageMaterialType stageMaterialType = _materialMappings[_materialType.Value];
                
                var newKey = $"{_itemType}_{stageMaterialType}";

                Sprite itemSprite = null;
            
                if (_itemSprites.TryGetValue(newKey, out Sprite sprite))
                {
                    itemSprite = sprite;
                }
                else if (_currencySprites.TryGetValue(newKey, out Sprite currencySprite))
                {
                    itemSprite = currencySprite;
                }
            
                item.Initialize(newKey, itemSprite, initializePosition);
            }

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
                var dicKey = EnumParserModule.ParseEnumToString(data.ItemType, data.StageMaterialType);
                _itemSprites.TryAdd(dicKey, data.Sprite);
            }
            
            List<CurrencySprite> currencySprites = ItemDataSo.CurrencySprites;
            _currencySprites = new Dictionary<string, Sprite>();
            
            foreach (CurrencySprite data in currencySprites)
            {
                var dicKey = EnumParserModule.ParseEnumToString(data.CurrencyType);
                _currencySprites.TryAdd(dicKey, data.Sprite);
            }
        }
    }
}