using System;
using System.Collections.Generic;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Modules.FactoryModules.Abstract;
using Units.Stages.Controllers;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Units.Modules.FactoryModules.Units
{
    public interface IMonsterFactory
    {
        public MonsterDataSO MonsterDataSo { get; }
        public Dictionary<EStageMaterialType, List<Sprite>> MonsterSprites { get; }
        public IMonster GetMonster(Vector3 randomSpawnPoint, EMaterialType type, Action<IMonster> onReturn);
    }
    
    public class MonsterFactory : Factory, IMonsterFactory
    {
        public MonsterDataSO MonsterDataSo { get; } = DataManager.Instance.MonsterDataSo;
        public Dictionary<EStageMaterialType, List<Sprite>> MonsterSprites { get; } = new();
        private readonly Dictionary<EMaterialType, EStageMaterialType> _materialMappings;
        
        private static string PoolKey => "MonsterPool";
        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;
        
        public MonsterFactory(List<MaterialMapping> materialMappings)
        {
            _materialMappings = ListParerModule.ConvertListToDictionary(materialMappings, key => key.MaterialType, value => value.StageMaterialType);
            CreateMonsterPools();
            CreateSpriteDictionary();
        }

        public IMonster GetMonster(Vector3 randomSpawnPoint, EMaterialType type, Action<IMonster> onReturn)
        {
            var monster = ObjectPoolManager.Instance.GetObject<IMonster>(PoolKey, null);
            
            monster.Initialize(randomSpawnPoint, MonsterSprites[_materialMappings[type]], () =>
            {
                ObjectPoolManager.Instance.ReturnObject(PoolKey, monster);
                onReturn?.Invoke(monster);
            });

            return monster;
        }
        
        private void CreateMonsterPools()
        {
            ObjectPoolManager.Instance.CreatePool(PoolKey, DefaultPoolSize, MaxPoolSize, true, () => InstantiateMonster(MonsterDataSo.prefab));
        }
        
        private IMonster InstantiateMonster(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            var monster = obj.GetComponent<IMonster>();
            
            monster.RegisterReference(MonsterDataSo);
            
            return monster;
        }
        
        private void CreateSpriteDictionary()
        {
            List<MonsterSprite> monsterSpritesList = MonsterDataSo.MonsterSprites;
            
            foreach (MonsterSprite data in monsterSpritesList)
            {
                EStageMaterialType dicKey = data.StageMaterialType;

                var monsterSprites = new List<Sprite>
                {
                    data.EmotionIdleSprite,
                    data.EmotionScaredSprite,
                    data.BodySprite,
                    data.LegLeftSprite,
                    data.LegRightSprite
                };
                MonsterSprites.TryAdd(dicKey, monsterSprites);
            }
        }
    }
}