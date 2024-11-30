using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.FactoryModules.Abstract;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Stages.Modules.FactoryModules.Units
{
    public interface IHunterFactory
    {
        public HunterDataSO HunterDataSo { get; }
        public HunterDataSO SuperHunterDataSo { get; }
        public IHunter GetHunter(Vector3 startPosition);
        public IHunter GetSuperHunter(Vector3 startPosition);
    }

    public class HunterFactory : NPCFactory, IHunterFactory
    {
        private const int DefaultPoolSize = 5;
        private const int MaxPoolSize = 5;

        private readonly IItemFactory _itemFactory;

        public HunterFactory(IItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            CreateHunterPools();
        }

        private static string PoolKey_Normal => "HunterPool";
        private static string PoolKey_Super => "HunterPool";
        public HunterDataSO HunterDataSo => DataManager.Instance.HunterDataSo;
        public HunterDataSO SuperHunterDataSo => DataManager.Instance.SuperHunterDataSo;

        public IHunter GetHunter(Vector3 startPosition)
        {
            var hunter = ObjectPoolManager.Instance.GetObject<IHunter>(PoolKey_Normal, null);

            hunter.Initialize(startPosition, GetRandomSprites(HunterDataSo.CreatureSprites));

            return hunter;
        }
        
        public IHunter GetSuperHunter(Vector3 startPosition)
        {
            var hunter = ObjectPoolManager.Instance.GetObject<IHunter>(PoolKey_Super, null);

            hunter.Initialize(startPosition, GetRandomSprites(SuperHunterDataSo.CreatureSprites));

            return hunter;
        }

        private void CreateHunterPools()
        {
            ObjectPoolManager.Instance.CreatePool(PoolKey_Normal, DefaultPoolSize, MaxPoolSize, true,
                () => InstantiateHunter(HunterDataSo.prefab));
        }
        
        private void CreateSuperHunterPools()
        {
            ObjectPoolManager.Instance.CreatePool(PoolKey_Super, DefaultPoolSize, MaxPoolSize, true,
                () => InstantiateSuperHunter(SuperHunterDataSo.prefab));
        }

        private IHunter InstantiateHunter(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            var deliveryMan = obj.GetComponent<IHunter>();

            deliveryMan.RegisterReference(HunterDataSo, _itemFactory);

            return deliveryMan;
        }
        
        private IHunter InstantiateSuperHunter(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            var deliveryMan = obj.GetComponent<IHunter>();

            deliveryMan.RegisterReference(SuperHunterDataSo, _itemFactory);

            return deliveryMan;
        }
    }
}