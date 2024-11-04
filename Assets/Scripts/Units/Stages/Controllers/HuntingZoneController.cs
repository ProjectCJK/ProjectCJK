using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using Units.Stages.Enums;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using Units.Stages.Units.Zones.Units.HuntingZones;
using Unity.VisualScripting;
using UnityEngine;
using IInitializable = Interfaces.IInitializable;

namespace Units.Stages.Controllers
{
    public interface IHuntingZoneController : IRegisterReference<ICreatureController, IItemFactory, IPlayer>, IInitializable
    {
        public void HandleOnRegisterPlayer(IPlayer player, bool register);
    }
    
    public class HuntingZoneController : MonoBehaviour, IHuntingZoneController
    {
        private readonly Dictionary<IHuntingZoneProperty, EActiveStatus> huntingZones = new();
        
        private ICreatureController _creatureController;
        private IItemFactory itemFactory;
        private IPlayer _player;
        
        private float _itemPickupRange;
        
        private readonly List<IItem> _droppedItems = new(); 
        
        public void RegisterReference(ICreatureController creatureController, IItemFactory itemController, IPlayer player)
        {
            _creatureController = creatureController;
            itemFactory = itemController;
            _player = player;
            _itemPickupRange = DataManager.Instance.PlayerDataSo.ItemPickupRange;
            
            CreateHuntingZoneDictionary();
        }

        public void Initialize()
        {
            foreach (KeyValuePair<IHuntingZoneProperty, EActiveStatus> obj in huntingZones)
            {
                obj.Key.Initialize();
            }
        }

        private void Update()
        {
            SendItem(); 
        }

        private void CreateHuntingZoneDictionary()
        {
            foreach (Transform child in transform)
            {
                var huntingZone = child.GetComponent<HuntingZoneProperty>();
                huntingZone.RegisterReference(_creatureController, itemFactory, item => _droppedItems.Add(item));
                huntingZones.TryAdd(huntingZone, huntingZone.ActiveStatus);
            }
        }
        
        private void SendItem()
        {
            if (_player == null || _droppedItems.Count <= 0) return;
            
            for (var i = _droppedItems.Count - 1; i >= 0; i--)
            {
                if (!_player.PlayerInventoryModule.CanReceiveItem()) return; 
                
                IItem item = _droppedItems[i];
                Vector3 currentItemPosition = item.Transform.position;

                if (Vector3.Distance(currentItemPosition, _player.Transform.position) <= _itemPickupRange)
                {
                    _player.PlayerInventoryModule.ReceiveItemThroughTransfer(item.Type, item.Count, currentItemPosition);
                    itemFactory.ReturnItem(item);
                    _droppedItems.RemoveAt(i);
                }
            }
        }
        
        public void HandleOnRegisterPlayer(IPlayer player, bool register)
        {
            _player = register ? player : null;
        }
    }
}