using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Interfaces;
using Units.Buildings.Abstract;
using Units.Creatures.Interfaces;
using Units.Systems.InventorySystems.Abstract;
using UnityEngine;

namespace Units.Systems.InventorySystems.Units
{
    public class PlayerInventorySystem : BaseInventorySystem, IReferenceRegisterable<IInventoryProperty>, IInitializable
    {
        public int MaxInventorySize => _inventoryProperty.InventorySize;

        public int CurrentInventorySize => _inventory.Sum(item => item.Value);
    
        private readonly Dictionary<Tuple<EMaterialType, EItemType>, int> _inventory = new();
    
        private IInventoryProperty _inventoryProperty;

        public void RegisterReference(IInventoryProperty inventoryProperty)
        {
            _inventoryProperty = inventoryProperty;
        }
    
        public void Initialize()
        {
            _inventory.Clear();
        }

        public bool HasItem(Tuple<EMaterialType, EItemType> InventoryKey) => _inventory.ContainsKey(InventoryKey) && _inventory[InventoryKey] > 0;
        
        public void TransferItem(BaseBuilding building)
        {
            Tuple<EMaterialType, EItemType> key = building.InventoryKey;
            
            Debug.Log($"{gameObject.name}이 {building.gameObject.name}에게 {key.Item1} {key.Item2} 전달 시도");
        
            if (_inventory.ContainsKey(key) && _inventory[key] > 0)
            {
                building.ReceiveItem();
                _inventory[key]--;

                if (_inventory[key] == 0)
                {
                    _inventory.Remove(key);
                }
            }
        }

        public void AddItem(EItemType itemType, EMaterialType materialType, int count = 1)
        {
            Tuple<EMaterialType, EItemType> key = Tuple.Create(materialType, itemType);
            
            if (!_inventory.TryAdd(key, count))
            {
                _inventory[key] += count;
            }
        }

        public bool InventoryIsFull()
        {
            return CurrentInventorySize >= MaxInventorySize;
        }

        public int GetItemCount(EItemType itemType, EMaterialType materialType)
        {
            Tuple<EMaterialType, EItemType> key = Tuple.Create(materialType, itemType);
            
            return _inventory.GetValueOrDefault(key, 0);
        }
    }
}