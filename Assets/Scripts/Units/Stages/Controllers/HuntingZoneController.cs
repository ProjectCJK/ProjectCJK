using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Enums;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.HuntingZones;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEditor.UIElements;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface IHuntingZoneController : IRegisterReference<IMonsterFactory, IItemController, IPlayer>
    {
        
    }
    
    public class HuntingZoneController : MonoBehaviour, IHuntingZoneController
    {
        private readonly Dictionary<IHuntingZone, EActiveStatus> huntingZones = new();
        private readonly Dictionary<EMaterialType, Sprite> _monsterSprites = new();
        
        private IMonsterFactory _monsterFactory;
        private IItemController _itemController;
        private IPlayer _player;

        private float _itemPickupRange;

        private readonly List<IItem> _droppedItems = new(); 
        
        public void RegisterReference(IMonsterFactory monsterFactory, IItemController itemController, IPlayer player)
        {
            _monsterFactory = monsterFactory;
            _itemController = itemController;
            _player = player;
            _itemPickupRange = DataManager.Instance.PlayerData.ItemPickupRange;

            CreateSpriteDictionary();
            CreateHuntingZoneDictionary();
            InstantiateMonster();
        }

        public void Initialize()
        {
            foreach (KeyValuePair<IHuntingZone, EActiveStatus> obj in huntingZones)
            {
                obj.Key.Initialize();
            }
        }

        private void Update()
        {
            SendItem();
        }

        private void CreateSpriteDictionary()
        {
            List<MonsterSprite> monsterSpritesList = _monsterFactory.MonsterDataSo.MonsterSprites;
            
            foreach (MonsterSprite data in monsterSpritesList)
            {
                EMaterialType dicKey = data.MaterialType;
                _monsterSprites.TryAdd(dicKey, data.Sprite);
            }
        }

        private void CreateHuntingZoneDictionary()
        {
            foreach (Transform child in transform)
            {
                var huntingZone = child.GetComponent<HuntingZone>();
                huntingZone.RegisterReference(_monsterFactory.PoolKey, _monsterSprites, _itemController, item => _droppedItems.Add(item));
                huntingZones.TryAdd(huntingZone, huntingZone.ActiveStatus);
            }
        }
        
        private void InstantiateMonster()
        {
            _monsterFactory.CreateMonster();
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
                    if (_player.PlayerInventoryModule.ReceiveItemWithDestroy(item.Type, currentItemPosition))
                    {
                        _itemController.ReturnItem(item);
                        _droppedItems.RemoveAt(i);
                    }
                }
            }
        }
    }
}