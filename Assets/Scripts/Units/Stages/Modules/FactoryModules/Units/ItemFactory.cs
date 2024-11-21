using System;
using System.Collections.Generic;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Items;
using Units.Stages.Controllers;
using Units.Stages.Modules.FactoryModules.Abstract;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Units.Stages.Modules.FactoryModules.Units
{
    public interface IItemFactory
    {
        public ItemDataSO ItemDataSo { get; }
        public IItem GetItem(string itemType, int count, Vector3 initializePosition, bool atRoot);
        public void ReturnItem(IItem item);
    }

    public class ItemFactory : Factory, IItemFactory
    {
        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;
        private readonly Dictionary<EMaterialType, EStageMaterialType> _materialMappings;

        private readonly Transform _parentTransform;
        private Dictionary<string, Sprite> _currencySprites;
        private Dictionary<string, Sprite> _itemSprites;

        public ItemFactory(Transform parentTransform, List<MaterialMapping> materialMappings)
        {
            _parentTransform = parentTransform;
            _materialMappings = ListParerModule.ConvertListToDictionary(materialMappings, key => key.MaterialType,
                value => value.StageMaterialType);
            CreateItemPools();
            CreateSpriteDictionary();
        }

        private static string PoolKey => "ItemPool";
        public ItemDataSO ItemDataSo => DataManager.Instance.ItemDataSo;

        public IItem GetItem(string itemType, int count, Vector3 initializePosition, bool atRoot)
        {
            var item = ObjectPoolManager.Instance.GetObject<IItem>(PoolKey, null);

            Sprite itemSprite = null;
            var split = itemType.Split('_');

            if (Enum.TryParse(split[0], out ECurrencyType currencyType))
            {
                if (_currencySprites.TryGetValue($"{currencyType}", out Sprite currencySprite))
                    itemSprite = currencySprite;
            }
            else
            {
                (EItemType? _itemType, EMaterialType? _materialType) =
                    ParserModule.ParseStringToEnum<EItemType, EMaterialType>(itemType);

                if (_itemType != null && _materialType != null)
                {
                    EStageMaterialType stageMaterialType = _materialMappings[_materialType.Value];

                    var newKey = $"{_itemType}_{stageMaterialType}";

                    if (_itemSprites.TryGetValue(newKey, out Sprite sprite)) itemSprite = sprite;
                }
            }

#if UNITY_EDITOR
            if (itemSprite == null) Debug.LogError($"{itemType} is not a valid item type");
#endif

            item.Initialize(itemType, count, itemSprite, initializePosition, atRoot);

            return item;
        }

        public void ReturnItem(IItem item)
        {
            if (!ReferenceEquals(item.Transform.parent, _parentTransform)) item.Transform.SetParent(_parentTransform);
            ObjectPoolManager.Instance.ReturnObject(PoolKey, item);
        }

        private void CreateItemPools()
        {
            ObjectPoolManager.Instance.CreatePool(PoolKey, DefaultPoolSize, MaxPoolSize, true,
                () => OnInstantiateItem(ItemDataSo.prefab));
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
                var dicKey = ParserModule.ParseEnumToString(data.ItemType, data.StageMaterialType);
                _itemSprites.TryAdd(dicKey, data.Sprite);
            }

            List<CurrencySprite> currencySprites = ItemDataSo.CurrencySprites;
            _currencySprites = new Dictionary<string, Sprite>();

            foreach (CurrencySprite data in currencySprites)
            {
                var dicKey = ParserModule.ParseEnumToString(data.CurrencyType);
                _currencySprites.TryAdd(dicKey, data.Sprite);
            }
        }
    }
}