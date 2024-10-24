using System;
using System.Collections.Generic;
using Interfaces;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Enums;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.HuntingZones;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface IHuntingZoneController : IRegisterReference<IMonsterFactory, IItemController>
    {
        
    }
    
    public class HuntingZoneController : MonoBehaviour, IHuntingZoneController
    {
        private readonly Dictionary<IHuntingZone, EActiveStatus> huntingZones = new();
        private readonly Dictionary<EMaterialType, Sprite> _monsterSprites = new();
        
        private IMonsterFactory _monsterFactory;
        private IItemController _itemController;
        
        public void RegisterReference(IMonsterFactory monsterFactory, IItemController itemController)
        {
            _monsterFactory = monsterFactory;
            _itemController = itemController;

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
                huntingZone.RegisterReference(_monsterFactory.PoolKey, _monsterSprites, _itemController);
                huntingZones.TryAdd(huntingZone, huntingZone.ActiveStatus);
            }
        }
        
        private void InstantiateMonster()
        {
            _monsterFactory.CreateMonster();
        }
    }
}