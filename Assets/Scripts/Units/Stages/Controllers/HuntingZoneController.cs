using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Interfaces;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.HuntingZones;
using Units.Stages.Units.Items.Units;
using UnityEngine;
using IInitializable = Interfaces.IInitializable;

namespace Units.Stages.Controllers
{
    public interface IHuntingZoneController : IRegisterReference<ICreatureController, IItemFactory, IPlayer>,
        IInitializable
    {
        public void HandleOnRegisterPlayer(IPlayer player, bool register);
        public void SpawnMonsters();
        public void ControlDroppedItem();
        public void SendDroppedItem(HashSet<IHunter> currentSpawnedHunters);
    }
    
    [Serializable]
    public struct HuntingZoneSpawnData
    {
        [Header("=== HuntingZone Position ===")]
        public List<ObjectTrackingTargetModule> HuntingZoneSpawners;
    }

    public class DropItem
    {
        public float Timer;
        public IItem Item;

        public DropItem(float timer, IItem item)
        {
            Timer = timer;
            Item = item;
        }
    }
    
    public class HuntingZoneController : MonoBehaviour, IHuntingZoneController
    {
        [SerializeField] private HuntingZoneSpawnData _huntingZoneSpawnData;

        public HuntingZoneSpawnData HuntingZoneSpawnData => _huntingZoneSpawnData;
        
        public Dictionary<string, HuntingZone> HuntingZones { get; } = new();

        private ICreatureController _creatureController;
        private float _hunterItemPickupRange;
        private IPlayer _player;

        private float _playerItemPickupRange;
        private IItemFactory itemFactory;

        private const float maintainedTime = 60f;
        private readonly List<DropItem> _droppedItems = new();

        public void RegisterReference(ICreatureController creatureController, IItemFactory itemController, IPlayer player)
        {
            _creatureController = creatureController;
            itemFactory = itemController;
            _player = player;
            _playerItemPickupRange = DataManager.Instance.PlayerDataSo.ItemPickupRange;
            _hunterItemPickupRange = DataManager.Instance.HunterDataSo.ItemPickupRange;

            for (var i = 0; i < DataManager.Instance.levelPrefabSo.Levels[GameManager.Instance.ES3Saver.CurrentStageLevel - 1].HuntingZones.Count; i++)
            {
                GameObject huntingZonePrefab = DataManager.Instance.levelPrefabSo.Levels[GameManager.Instance.ES3Saver.CurrentStageLevel - 1].HuntingZones[i];
                InstantiateHuntingZones(HuntingZoneSpawnData.HuntingZoneSpawners[i].transform, huntingZonePrefab);
            }
        }
        
        public void Initialize()
        {
            foreach (var obj in HuntingZones) obj.Value.Initialize();
        }

        public void SpawnMonsters()
        {
            foreach (var obj in HuntingZones) obj.Value.SpawnMonsters();
        }

        public void ControlDroppedItem()
        {
            // 리스트를 역순으로 순회하여 안전하게 요소를 제거
            for (var i = _droppedItems.Count - 1; i >= 0; i--)
            {
                DropItem dropItem = _droppedItems[i];
                dropItem.Timer += Time.deltaTime;

                if (dropItem.Timer >= maintainedTime)
                {
                    itemFactory.ReturnItem(dropItem.Item); // 아이템 반환
                    _droppedItems.RemoveAt(i); // 리스트에서 제거
                }
            }
        }

        public void SendDroppedItem(HashSet<IHunter> currentSpawnedHunters)
        {
            if (_droppedItems.Count <= 0) return;

            // 아이템을 가장 가까운 플레이어나 헌터에게 전달
            for (var i = _droppedItems.Count - 1; i >= 0; i--)
            {
                IItem item = _droppedItems[i].Item;
                Vector3 currentItemPosition = item.Transform.position;

                // 플레이어와 헌터들을 모두 대상으로 순회하여 가장 가까운 대상을 찾음
                var closestDistance = float.MaxValue;
                IItemReceiver closestReceiver = null;

                // 플레이어와의 거리 확인
                if (_player != null && Vector3.Distance(currentItemPosition, _player.Transform.position) <=
                    _playerItemPickupRange)
                {
                    closestDistance = Vector3.Distance(currentItemPosition, _player.Transform.position);
                    closestReceiver = _player.PlayerInventoryModule;
                }

                // 각 헌터와의 거리 확인
                foreach (IHunter hunter in currentSpawnedHunters)
                {
                    var distanceToHunter = Vector3.Distance(currentItemPosition, hunter.Transform.position);

                    if (distanceToHunter <= _hunterItemPickupRange && distanceToHunter < closestDistance)
                    {
                        closestDistance = distanceToHunter;
                        closestReceiver = hunter.HunterInventoryModule;
                    }
                }

                // 가장 가까운 대상에게 아이템 전달
                if (closestReceiver != null && closestReceiver.CanReceiveItem())
                {
                    closestReceiver.ReceiveItemThroughTransfer(item.Type, item.Count, currentItemPosition);
                    itemFactory.ReturnItem(item);
                    _droppedItems.RemoveAt(i);
                }
            }
        }
        
        public void HandleOnRegisterPlayer(IPlayer player, bool register)
        {
            _player = register ? player : null;
        }

        private void InstantiateHuntingZones(Transform spawnTransform, GameObject prefab)
        {
            if (spawnTransform == null || prefab == null) return;

            GameObject instance = Instantiate(prefab, spawnTransform);
            instance.transform.localPosition = Vector3.zero;
            
            var huntingZone = instance.GetComponent<HuntingZone>();
            huntingZone.RegisterReference(_creatureController, itemFactory, item => _droppedItems.Add(new DropItem(0, item)));
            HuntingZones.TryAdd(huntingZone.HuntingZoneKey, huntingZone);
            
            VolatileDataManager.Instance.HuntingZoneActiveStatuses.TryAdd(huntingZone, huntingZone.ActiveStatus);
        }
    }
}