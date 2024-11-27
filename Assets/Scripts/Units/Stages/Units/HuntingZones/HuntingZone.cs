using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Zones;
using Units.Stages.Controllers;
using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Units.HuntingZoneInventoryModules;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using Units.Stages.Modules.UnlockModules.Interfaces;
using Units.Stages.Modules.UnlockModules.Units;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Units.Stages.Units.HuntingZones
{
    public interface IHuntingZone : IRegisterReference<ICreatureController, IItemFactory, Action<IItem>>,
        IInitializable, IUnlockZoneProperty
    {
        public HashSet<IMonster> CurrentSpawnedMonsters { get; }
        public void SpawnMonsters();
    }

    [Serializable]
    public struct HuntingZoneDefaultSetting
    {
        [Header("### HuntingZoneDefaultSetting ###")] [SerializeField] [Header("MonsterSpawnPoint")]
        public TilemapCollider2D monsterSpawnPoint;

        [Space(10)] [Header("UnlockZone_Player")]
        public Transform UnlockZone_Player;
    }

    [Serializable]
    public struct HuntingZoneCustomSetting
    {
        [Header("### HuntingZoneCustomSetting ###")] [SerializeField] [Header("소환할 몬스터 타입")]
        public EMaterialType _materialType;

        [SerializeField] [Header("최대 몬스터 소환 개수 제한")]
        public int _monsterSpawnCount;
    }

    public class HuntingZone : MonoBehaviour, IHuntingZone
    {
        [SerializeField] private HuntingZoneDefaultSetting HuntingZoneDefaultSetting;
        [Space(20)] [SerializeField] private HuntingZoneCustomSetting huntingZoneCustomSetting;
        private ICreatureController _creatureController;
        private HuntingZoneDataSO _huntingZoneDataSo;

        private IHuntingZoneInventoryModule _huntingZoneInventoryModule;

        public string HuntingZoneKey;
        private string _itemKey;
        private string effectKey;

        private ITradeZone _unlockZonePlayer;
        private IItemFactory itemFactory;

        public UnlockZoneModule UnlockZoneModule { get; private set; }
        public HashSet<IMonster> CurrentSpawnedMonsters { get; } = new();
        public EUnlockZoneType UnlockZoneType => UnlockZoneModule.UnlockZoneType;
        public EActiveStatus ActiveStatus => UnlockZoneModule.ActiveStatus;
        public int RequiredGoldForUnlock => UnlockZoneModule.RequiredGoldForUnlock;
        public int CurrentGoldForUnlock { get; set; }

        public void RegisterReference(ICreatureController creatureController, IItemFactory itemController,
            Action<IItem> action)
        {
            _huntingZoneDataSo = DataManager.Instance.HuntingZoneDataSo;
            _creatureController = creatureController;
            itemFactory = itemController;

            _itemKey = ParserModule.ParseEnumToString(EItemType.Material, huntingZoneCustomSetting._materialType);

            _huntingZoneInventoryModule = new HuntingZoneInventoryModule(HuntingZoneDefaultSetting.UnlockZone_Player,
                HuntingZoneDefaultSetting.UnlockZone_Player, itemController, null, string.Empty, string.Empty, HuntingZoneKey);
            HuntingZoneKey = $"HuntingZone_{huntingZoneCustomSetting._materialType.ToString()}";

            UnlockZoneModule = GetComponent<HuntingZoneUnlockZoneModule>();
            UnlockZoneModule.RegisterReference($"HuntingZone_{huntingZoneCustomSetting._materialType}");

            OnDroppedItem += action;

            _unlockZonePlayer = HuntingZoneDefaultSetting.UnlockZone_Player.GetComponent<ITradeZone>();
            _unlockZonePlayer.RegisterReference(this, HuntingZoneDefaultSetting.UnlockZone_Player,
                _huntingZoneInventoryModule, null, HuntingZoneKey, $"{ECurrencyType.Money}");

            _huntingZoneInventoryModule.OnMoneyReceived += HandleOnMoneyReceived;
        }

        public void Initialize()
        {
            UnlockZoneModule.UpdateViewModel();
        }

        public void SpawnMonsters()
        {
            if (CurrentSpawnedMonsters.Count < huntingZoneCustomSetting._monsterSpawnCount)
            {
                IMonster monster = _creatureController.GetMonster(GetRandomSpawnPoint(),
                    huntingZoneCustomSetting._materialType, ReturnMonster);

                if (monster != null) CurrentSpawnedMonsters.Add(monster);
            }
        }

        private event Action<IItem> OnDroppedItem;
        private event Action HandleOnPlayerEncounter;

        private void ReturnMonster(IMonster monster)
        {
            CurrentSpawnedMonsters.Remove(monster);
            SetEffects(monster.Transform.position);
            DropItems(monster.Transform.position);
        }

        private void DropItems(Vector3 senderPosition)
        {
            Vector3 receiverPosition = GetRandomItemDropPoint(senderPosition);

            IItem item = itemFactory.GetItem(_itemKey, 1, senderPosition, false);
            item.Transfer(senderPosition, receiverPosition, () => OnDroppedItem?.Invoke(item));
        }
        
        private void SetEffects(Vector3 transformPosition)
        {
            //  item = itemFactory.GetItem(_itemKey, 1, senderPosition, false);
            // item.Transfer(senderPosition, receiverPosition, () => OnDroppedItem?.Invoke(item));
        }

        private Vector3 GetRandomItemDropPoint(Vector3 senderPosition)
        {
            // 최소/최대 범위를 정의
            var minimumRange = _huntingZoneDataSo.ItemDropMinimumRange;
            var maxRange = _huntingZoneDataSo.ItemDropMaximumRange;

            // 각도는 0에서 360도까지 랜덤
            var angle = Random.Range(0f, Mathf.PI * 2); // 라디안 단위로 각도 계산

            // 최소, 최대 반경 범위 내에서 무작위 반경 값을 선택
            var radius = Random.Range(minimumRange, maxRange);

            // 극좌표계에서 X, Y 좌표로 변환
            var randomX = radius * Mathf.Cos(angle);
            var randomY = radius * Mathf.Sin(angle);

            // senderPosition을 기준으로 좌표 계산
            Vector3 randomPosition = new Vector3(randomX, randomY, 0) + senderPosition;

            return randomPosition;
        }

        private Vector3 GetRandomSpawnPoint()
        {
            Bounds bounds = HuntingZoneDefaultSetting.monsterSpawnPoint.bounds;

            var randomX = Random.Range(bounds.min.x, bounds.max.x);
            var randomY = Random.Range(bounds.min.y, bounds.max.y);

            return new Vector3(randomX, randomY, 0);
        }

        private void HandleOnMoneyReceived(int value)
        {
            CurrentGoldForUnlock += value;
            UnlockZoneModule.CurrentGoldForUnlock = CurrentGoldForUnlock;
            UnlockZoneModule.UpdateViewModel();

            if (CurrentGoldForUnlock >= RequiredGoldForUnlock) UnlockZoneModule.SetCurrentState(EActiveStatus.Active);
        }
    }
}