using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.FactoryModules.Abstract;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Stages.Modules.FactoryModules.Units
{
    public interface IDeliveryManFactory
    {
        public DeliveryManDataSO DeliveryManDataSo { get; }
        public IDeliveryMan GetDeliveryMan(Vector3 startPosition);
    }

    public class DeliveryManFactory : NPCFactory, IDeliveryManFactory
    {
        private const int DefaultPoolSize = 1;
        private const int MaxPoolSize = 5;

        private readonly IItemFactory _itemFactory;

        public DeliveryManFactory(IItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            CreateDeliveryManPools();
        }

        private static string PoolKey => "DeliveryManPool";
        public DeliveryManDataSO DeliveryManDataSo => DataManager.Instance.DeliveryManDataSo;

        public IDeliveryMan GetDeliveryMan(Vector3 startPosition)
        {
            var deliveryMan = ObjectPoolManager.Instance.GetObject<IDeliveryMan>(PoolKey, null);

            deliveryMan.Initialize(startPosition, GetRandomSprites(DeliveryManDataSo.CreatureSprites));

            return deliveryMan;
        }

        private void CreateDeliveryManPools()
        {
            ObjectPoolManager.Instance.CreatePool(PoolKey, DefaultPoolSize, MaxPoolSize, true,
                () => InstantiateDeliveryMan(DeliveryManDataSo.prefab));
        }

        private IDeliveryMan InstantiateDeliveryMan(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            var deliveryMan = obj.GetComponent<IDeliveryMan>();

            deliveryMan.RegisterReference(DeliveryManDataSo, _itemFactory);

            return deliveryMan;
        }
    }
}