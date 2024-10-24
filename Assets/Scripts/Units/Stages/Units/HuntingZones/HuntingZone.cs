using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.HuntingZones;
using Units.Stages.Controllers;
using Units.Stages.Enums;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Units.Stages.Units.HuntingZones
{
    public interface IHuntingZone : IRegisterReference<string, Dictionary<EMaterialType, Sprite>, IItemController>, IInitializable
    {
        public EActiveStatus ActiveStatus { get; }
    }
    
    [Serializable]
    public struct HuntingZoneDefaultSetting
    {
        [Header("### HuntingZoneDefaultSetting ###")]
        [SerializeField, Header("MonsterSpawnPoint")] public TilemapCollider2D monsterSpawnPoint;
    }
    
    [Serializable]
    public struct HuntingZoneCustomSetting
    {
        [Header("### HuntingZoneCustomSetting ###")]
        [SerializeField, Header("생성 시 기본 상태")] public EActiveStatus _baseActiveStatus;
        [SerializeField, Header("소환할 몬스터 타입")] public EMaterialType _materialType;
        [SerializeField, Header("최대 몬스터 소환 개수 제한")] public int _monsterSpawnCount;
    }
    
    public class HuntingZone : MonoBehaviour, IHuntingZone
    {
        [SerializeField] private HuntingZoneDefaultSetting HuntingZoneDefaultSetting;
        [Space(20), SerializeField] private HuntingZoneCustomSetting huntingZoneCustomSetting;

        public EActiveStatus ActiveStatus => huntingZoneCustomSetting._baseActiveStatus;
        
        private readonly HashSet<Monster> currentSpawnedMonsters = new();

        private IItemController _itemController;
        
        private Tuple<EMaterialType, EItemType> _itemType;
        private Dictionary<EMaterialType, Sprite> _monsterSprites;

        private HuntingZoneDataSO _huntingZoneDataSo; 
        
        private string _poolKey;
        
        public void RegisterReference(string poolKey, Dictionary<EMaterialType, Sprite> monsterSprites, IItemController itemController)
        {
            _huntingZoneDataSo = DataManager.Instance.HuntingZoneData;
            _poolKey = poolKey;
            _monsterSprites = monsterSprites;
            _itemController = itemController;

            _itemType = new Tuple<EMaterialType, EItemType>(huntingZoneCustomSetting._materialType, EItemType.Material);
        }
        
        public void Initialize()
        {
            SpawnMonsters();
        }

        private void SpawnMonsters()
        {
            while (currentSpawnedMonsters.Count < huntingZoneCustomSetting._monsterSpawnCount)
            {
                var monster = ObjectPoolManager.Instance.GetObject<Monster>(_poolKey, null);
                monster.Initialize(_monsterSprites[huntingZoneCustomSetting._materialType], () => ReturnMonster(monster));
                
                if (monster != null)
                {
                    monster.transform.position = GetRandomSpawnPoint();
                    currentSpawnedMonsters.Add(monster);
                }
            }
        }

        private void ReturnMonster(Monster monster)
        {
            ObjectPoolManager.Instance.ReturnObject(_poolKey, monster);
            currentSpawnedMonsters.Remove(monster);

            DropItems(monster.transform);
            SpawnMonsters();
        }

        private void DropItems(Transform senderTransform) 
        {
            Vector3 receiverPosition = GetRandomItemDropPoint(senderTransform.position);
            
            IItem item = _itemController.GetItem(_itemType, senderTransform.position);
            item.Transfer(senderTransform, receiverPosition, null);
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