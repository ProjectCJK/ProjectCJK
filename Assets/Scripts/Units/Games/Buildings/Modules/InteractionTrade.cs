using System;
using Interfaces;
using Units.Games.Items.Enums;
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
        public Transform ReceiverTransform => _buildingInventoryModule.ReceiverTransform;
        public Tuple<EMaterialType, EProductType> InputItemKey => _buildingInventoryModule.InputItemKey;
        
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

        public void ReceiveItem(Tuple<EMaterialType, EProductType> itemType)
        {
            _buildingInventoryModule.ReceiveItem(itemType);
        }

        public bool HasMatchingItem(Tuple<EMaterialType, EProductType> InventoryKey)
        {
            return _buildingInventoryModule.HasMatchingItem(InventoryKey);
        }

        public bool CanReceiveItem() => _buildingInventoryModule.CanReceiveItem();
    }
}
