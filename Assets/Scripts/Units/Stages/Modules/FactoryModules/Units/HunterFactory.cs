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
        public IHunter GetHunter(Vector3 startPosition);
    }

    public class HunterFactory : NPCFactory, IHunterFactory
    {
        private const int DefaultPoolSize = 1;
        private const int MaxPoolSize = 5;

        private readonly IItemFactory _itemFactory;

        public HunterFactory(IItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            CreateHunterPools();
        }

        private static string PoolKey => "HunterPool";
        public HunterDataSO HunterDataSo => DataManager.Instance.HunterDataSo;

        public IHunter GetHunter(Vector3 startPosition)
        {
            var hunter = ObjectPoolManager.Instance.GetObject<IHunter>(PoolKey, null);

            hunter.Initialize(startPosition, GetRandomSprites(HunterDataSo.CreatureSprites));

            return hunter;
        }

        private void CreateHunterPools()
        {
            ObjectPoolManager.Instance.CreatePool(PoolKey, DefaultPoolSize, MaxPoolSize, true,
                () => InstantiateHunter(HunterDataSo.prefab));
        }

        private IHunter InstantiateHunter(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            var deliveryMan = obj.GetComponent<IHunter>();

            deliveryMan.RegisterReference(HunterDataSo, _itemFactory);

            return deliveryMan;
        }
    }
}