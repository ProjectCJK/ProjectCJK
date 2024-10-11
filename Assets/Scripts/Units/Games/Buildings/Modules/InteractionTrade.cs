using System;
using Enums;
using Interfaces;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.InventoryModules.Units;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units.Games.Buildings.Modules
{
    public interface IInteractionTrade : IRegisterReference<IBuildingInventoryModule>, IItemReceiver
    {
        public void RegisterItemReceiver(ICreatureItemReceiver itemReceiver);
        public void UnregisterItemReceiver(ICreatureItemReceiver itemReceiver);
    }
    
    [RequireComponent(typeof(TilemapCollider2D))]
    public class InteractionTrade : MonoBehaviour, IInteractionTrade
    {
        public Tuple<EMaterialType, EItemType> InputItemKey => _buildingInventoryModule.InputItemKey;
        
        private IBuildingInventoryModule _buildingInventoryModule;
        
        
        public void RegisterReference(IBuildingInventoryModule buildingInventoryModule)
        {
            _buildingInventoryModule = buildingInventoryModule;
        }

        public void RegisterItemReceiver(ICreatureItemReceiver itemReceiver)
        {
            Debug.Log($"{itemReceiver}과 연결");
            
            _buildingInventoryModule.RegisterItemReceiver(itemReceiver);
        }

        public void UnregisterItemReceiver(ICreatureItemReceiver itemReceiver)
        {
            Debug.Log($"{itemReceiver}과 연결 종료");
            
            _buildingInventoryModule.UnRegisterItemReceiver(itemReceiver);
        }

        public void ReceiveItem(Tuple<EMaterialType, EItemType> itemType)
        {
            _buildingInventoryModule.ReceiveItem(itemType);
        }

        public bool HasMatchingItem(Tuple<EMaterialType, EItemType> InventoryKey)
        {
            return _buildingInventoryModule.HasMatchingItem(InventoryKey);
        }

        public bool CanReceiveItem() => _buildingInventoryModule.CanReceiveItem();
    }
}
