using System;
using System.Collections.Generic;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Controllers;
using Units.Stages.Modules.FactoryModules.Abstract;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Units.Stages.Modules.FactoryModules.Units
{
    public interface IMonsterFactory
    {
        public MonsterDataSO MonsterDataSo { get; }
        public Dictionary<EStageMaterialType, MonsterSprite> MonsterSprites { get; }
        public IMonster GetMonster(Vector3 randomSpawnPoint, EMaterialType type, Action<IMonster> onReturn);
    }

    public class MonsterFactory : Factory, IMonsterFactory
    {
        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;
        private readonly Dictionary<EMaterialType, EStageMaterialType> _materialMappings;
        
        private static string MonsterPoolKey => "MonsterPool";
        private static string MonsterEffectPoolKey => "MonsterDeathEffectPool";
        public MonsterDataSO MonsterDataSo { get; } = DataManager.Instance.MonsterDataSo;
        public Dictionary<EStageMaterialType, MonsterSprite> MonsterSprites { get; } = new();
        
        public MonsterFactory(List<MaterialMapping> materialMappings)
        {
            _materialMappings = ListParerModule.ConvertListToDictionary(materialMappings, key => key.MaterialType, value => value.StageMaterialType);
            
            CreateMonsterPools();
            CreateMonsterDeathEffectPools();
            CreateSpriteDictionary();
        }

        public IMonster GetMonster(Vector3 randomSpawnPoint, EMaterialType type, Action<IMonster> onReturn)
        {
            var monster = ObjectPoolManager.Instance.GetObject<IMonster>(MonsterPoolKey, null);

            monster.Initialize(type, randomSpawnPoint, MonsterSprites[_materialMappings[type]], () =>
            {
                var effect = ObjectPoolManager.Instance.GetObject<IMonsterDeathEffect>($"{MonsterEffectPoolKey}_{type}", null);
                effect.Initialize(monster.Transform.position);
                ObjectPoolManager.Instance.ReturnObject(MonsterPoolKey, monster);
                onReturn?.Invoke(monster);
            });

            return monster;
        }

        private void CreateMonsterPools()
        {
            ObjectPoolManager.Instance.CreatePool(MonsterPoolKey, DefaultPoolSize, MaxPoolSize, true, () => InstantiateMonster(MonsterDataSo.prefab));
        }
        
        private void CreateMonsterDeathEffectPools()
        {
            foreach (var materialMapping in _materialMappings)
            {
                GameObject monsterDeathEffectPrefab = materialMapping.Key switch
                {
                    EMaterialType.A => MonsterDataSo.tomatoDeathEffectPrefab,
                    EMaterialType.B => MonsterDataSo.cucumberDeathEffectPrefab,
                    EMaterialType.C => MonsterDataSo.beanDeathEffectPrefab,
                    _ => null
                };

                ObjectPoolManager.Instance.CreatePool($"{MonsterEffectPoolKey}_{materialMapping.Key}", DefaultPoolSize / 4, MaxPoolSize, true, () => InstantiateMonsterDeathEffect(monsterDeathEffectPrefab, $"{MonsterEffectPoolKey}_{materialMapping.Key}"));
            }
        }

        private IMonster InstantiateMonster(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            var monster = obj.GetComponent<IMonster>();

            monster.RegisterReference(MonsterDataSo);

            return monster;
        }
        
        private IMonsterDeathEffect InstantiateMonsterDeathEffect(GameObject prefab, string value)
        {
            GameObject obj = Object.Instantiate(prefab);
            var effect = obj.GetComponent<IMonsterDeathEffect>();
            
            effect.RegisterReference(() => ObjectPoolManager.Instance.ReturnObject(value, effect));
            
            return effect;
        }

        private void CreateSpriteDictionary()
        {
            List<MonsterSprite> monsterSpritesList = MonsterDataSo.MonsterSprites;

            foreach (MonsterSprite data in monsterSpritesList)
            {
                EStageMaterialType dicKey = data.StageMaterialType;

                MonsterSprites.TryAdd(dicKey, data);
            }
        }
    }
}