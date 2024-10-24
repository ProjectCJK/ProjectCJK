using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures;
using Units.Modules.FactoryModules.Abstract;
using Units.Stages.Controllers;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Modules.FactoryModules.Units
{
    public interface IMonsterFactory
    {
        public MonsterDataSO MonsterDataSo { get; }
        public string PoolKey { get; }
        public void CreateMonster();
    }
    
    public class MonsterFactory : Factory, IMonsterFactory
    {
        public MonsterDataSO MonsterDataSo { get; } = DataManager.Instance.MonsterData;
        public string PoolKey => "MonsterPool";
        
        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;

        public void CreateMonster()
        {
            ObjectPoolManager.Instance.CreatePool(((IMonsterFactory)this).PoolKey, DefaultPoolSize, MaxPoolSize, true, () => InstantiateMonster(MonsterDataSo.prefab));
        }

        private Monster InstantiateMonster(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            var monster = obj.GetComponent<Monster>();
            
            monster.RegisterReference(MonsterDataSo);
            
            return monster;
        }
    }
}