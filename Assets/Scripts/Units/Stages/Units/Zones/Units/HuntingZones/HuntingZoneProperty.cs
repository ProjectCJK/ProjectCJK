using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Zones;
using Units.Stages.Controllers;
using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Modules.UnlockModules.Enums;
using Units.Stages.Modules.UnlockModules.Interfaces;
using Units.Stages.Modules.UnlockModules.UI;
using Units.Stages.Modules.UnlockModules.Units;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Units.Stages.Units.Zones.Units.HuntingZones
{
    public interface IHuntingZoneProperty : IRegisterReference<ICreatureController, IItemFactory, Action<IItem>>, IInitializable, IUnlockZoneProperty
    {
        
    }
    
    [Serializable]
    public struct HuntingZoneDefaultSetting
    {
        [Header("ActiveStatus UI")]
        public UnlockZoneView unlockZoneView;
        
        [Header("### HuntingZoneDefaultSetting ###")]
        [SerializeField, Header("MonsterSpawnPoint")] public TilemapCollider2D monsterSpawnPoint;
    }
    
    [Serializable]
    public struct HuntingZoneCustomSetting
    {
        [Header("### HuntingZoneCustomSetting ###")]
        [SerializeField, Header("소환할 몬스터 타입")] public EMaterialType _materialType;
        [SerializeField, Header("최대 몬스터 소환 개수 제한")] public int _monsterSpawnCount;
    }
    
    public class HuntingZoneProperty : MonoBehaviour, IHuntingZoneProperty
    {
        public UnlockZoneModule UnlockZoneModule { get; private set; }
        public EUnlockZoneType UnlockZoneType => UnlockZoneModule.UnlockZoneType;
        public EActiveStatus ActiveStatus => UnlockZoneModule.ActiveStatus;
        public int RequiredGoldForUnlock
        {
            get => UnlockZoneModule.RequiredGoldForUnlock;
            set => UnlockZoneModule.RequiredGoldForUnlock = value;
        }

        private event Action<IItem> OnDroppedItem;
        private event Action HandleOnPlayerEncounter;
        
        [SerializeField] private HuntingZoneDefaultSetting HuntingZoneDefaultSetting;
        [Space(20), SerializeField] private HuntingZoneCustomSetting huntingZoneCustomSetting;
        private readonly HashSet<IMonster> currentSpawnedMonsters = new();

        private ICreatureController _creatureController;
        private IItemFactory itemFactory;
        
        private string _itemKey;
        private bool playerEncountered;
        private int CurrentGoldForUnlock;

        private HuntingZoneDataSO _huntingZoneDataSo; 
        
        public void RegisterReference(ICreatureController creatureController, IItemFactory itemController, Action<IItem> action)
        {
            UnlockZoneModule = GetComponent<HuntingZoneUnlockZoneModule>();
            
            _huntingZoneDataSo = DataManager.Instance.HuntingZoneDataSo;
            _creatureController = creatureController;
            itemFactory = itemController;

            OnDroppedItem += action;

            _itemKey = EnumParserModule.ParseEnumToString(EItemType.Material, huntingZoneCustomSetting._materialType);
        }
        
        public void Initialize()
        {
            playerEncountered = false;
            SpawnMonsters();
        }

        private void SpawnMonsters()
        {
            while (currentSpawnedMonsters.Count < huntingZoneCustomSetting._monsterSpawnCount)
            {
                IMonster monster = _creatureController.GetMonster(GetRandomSpawnPoint(), huntingZoneCustomSetting._materialType, ReturnMonster);
                
                if (monster != null)
                {
                    currentSpawnedMonsters.Add(monster);
                }
            }
        }

        private void ReturnMonster(IMonster monster)
        {
            currentSpawnedMonsters.Remove(monster);

            DropItems(monster.Transform.position);
            SpawnMonsters();
        }

        private void DropItems(Vector3 senderPosition) 
        {
            Vector3 receiverPosition = GetRandomItemDropPoint(senderPosition);
            
            IItem item = itemFactory.GetItem(_itemKey, 1, senderPosition);
            item.Transfer(senderPosition, receiverPosition, () => OnDroppedItem?.Invoke(item));
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
    }
}