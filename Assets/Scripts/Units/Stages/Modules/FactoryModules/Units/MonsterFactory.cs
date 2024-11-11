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

        public MonsterFactory(List<MaterialMapping> materialMappings)
        {
            _materialMappings = ListParerModule.ConvertListToDictionary(materialMappings, key => key.MaterialType,
                value => value.StageMaterialType);
            CreateMonsterPools();
            CreateSpriteDictionary();
        }

        private static string PoolKey => "MonsterPool";
        public MonsterDataSO MonsterDataSo { get; } = DataManager.Instance.MonsterDataSo;
        public Dictionary<EStageMaterialType, MonsterSprite> MonsterSprites { get; } = new();

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
            ObjectPoolManager.Instance.CreatePool(PoolKey, DefaultPoolSize, MaxPoolSize, true,
                () => InstantiateMonster(MonsterDataSo.prefab));
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

                MonsterSprites.TryAdd(dicKey, data);
            }
        }
    }
}