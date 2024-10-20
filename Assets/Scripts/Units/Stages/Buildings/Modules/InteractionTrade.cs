using System;
using System.Collections.Generic;
using Interfaces;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.InventoryModules.Units;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Stages.Items.Enums;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units.Stages.Buildings.Modules
{
    public interface IInteractionTrade : IRegisterReference<IBuildingInventoryModule, List<Tuple<EMaterialType, EItemType>>>, IItemReceiver
    {
        public List<Tuple<EMaterialType, EItemType>> InputItemKey { get; }
        public void RegisterItemReceiver(ICreatureItemReceiver itemReceiver);
        public void UnregisterItemReceiver(ICreatureItemReceiver itemReceiver);
    }
    
    [RequireComponent(typeof(TilemapCollider2D))]
    public class InteractionTrade : MonoBehaviour, IInteractionTrade
    {
        public Transform ReceiverTransform => _buildingInventoryModule.ReceiverTransform;
        public List<Tuple<EMaterialType, EItemType>> InputItemKey { get; private set; }
        
        private IBuildingInventoryModule _buildingInventoryModule;
        
        public void RegisterReference(IBuildingInventoryModule buildingInventoryModule, List<Tuple<EMaterialType, EItemType>> inputItemKey)
        {
            _buildingInventoryModule = buildingInventoryModule;
            InputItemKey = inputItemKey;
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

        public bool HasMatchingItem(Tuple<EMaterialType, EItemType> InventoryKey) => _buildingInventoryModule.HasMatchingItem(InventoryKey);

        public bool CanReceiveItem() => _buildingInventoryModule.CanReceiveItem();
        public void RegisterReference(IBuildingInventoryModule instance1, List<Tuple<EMaterialType, EItemType>> instance2, IItemReceiver instance3)
        {
            throw new NotImplementedException();
        }
    }
}
