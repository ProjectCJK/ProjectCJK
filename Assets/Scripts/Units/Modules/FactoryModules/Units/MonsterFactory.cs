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

namespace Units.Modules.FactoryModules.Units
{
    public interface IMonsterFactory
    {
        public MonsterDataSO MonsterDataSo { get; }
        public string PoolKey { get; }
        public Dictionary<EMaterialType, Sprite> MonsterSprites { get; }
        public void CreateMonster();
    }
    
    public class MonsterFactory : Factory, IMonsterFactory
    {
        public MonsterDataSO MonsterDataSo { get; } = DataManager.Instance.MonsterData;
        public string PoolKey => "MonsterPool";
        public Dictionary<EMaterialType, Sprite> MonsterSprites { get; } = new();

        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;

        public void CreateMonster()
        {
            ObjectPoolManager.Instance.CreatePool(PoolKey, DefaultPoolSize, MaxPoolSize, true, () => InstantiateMonster(MonsterDataSo.prefab));

            CreateSpriteDictionary();
        }

        private Monster InstantiateMonster(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            var monster = obj.GetComponent<Monster>();
            
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