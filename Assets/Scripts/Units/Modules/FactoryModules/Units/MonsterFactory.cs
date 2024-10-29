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
        public Dictionary<EMaterialType, Sprite> MonsterSprites { get; }
        public IMonster GetMonster(EMaterialType type, Action<IMonster> onReturn);
    }
    
    public class MonsterFactory : Factory, IMonsterFactory
    {
        public MonsterDataSO MonsterDataSo { get; } = DataManager.Instance.MonsterData;
        public Dictionary<EMaterialType, Sprite> MonsterSprites { get; } = new();

        private static string PoolKey => "MonsterPool";
        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;

        public MonsterFactory()
        {
            CreateMonsterPools();
            CreateSpriteDictionary();
        }

        public IMonster GetMonster(EMaterialType type, Action<IMonster> onReturn)
        {
            var monster = ObjectPoolManager.Instance.GetObject<IMonster>(PoolKey, null);
            
            monster.Initialize(MonsterSprites[type], () =>
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
                EMaterialType dicKey = data.MaterialType;
                MonsterSprites.TryAdd(dicKey, data.Sprite);
            }
        }
    }
}