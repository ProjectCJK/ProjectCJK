using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Stages.Modules.FactoryModules.Units
{
    public interface IDeliveryManFactory
    {
        public DeliveryManDataSO DeliveryManDataSo { get; }
        public IDeliveryMan GetDeliveryMan(Vector3 startPosition);
    }
    
    public class DeliveryManFactory : IDeliveryManFactory
    {
        public DeliveryManDataSO DeliveryManDataSo => DataManager.Instance.DeliveryManDataSo;
        
        private static string PoolKey => "DeliveryManPool";
        private const int DefaultPoolSize = 1;
        private const int MaxPoolSize = 5;
        
        private readonly IItemFactory _itemFactory;

        public DeliveryManFactory(IItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            CreateDeliveryManPools();
        }

        public IDeliveryMan GetDeliveryMan(Vector3 startPosition)
        {
            var deliveryMan = ObjectPoolManager.Instance.GetObject<IDeliveryMan>(PoolKey, null);

            deliveryMan.Initialize(startPosition);

            return deliveryMan;
        }
      
        private void CreateDeliveryManPools()
        {
            ObjectPoolManager.Instance.CreatePool(PoolKey, DefaultPoolSize, MaxPoolSize, true, () => InstantiateDeliveryMan(DeliveryManDataSo.prefab));
        }

        private IDeliveryMan InstantiateDeliveryMan(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            var guest = obj.GetComponent<IDeliveryMan>();
            
            guest.RegisterReference(DeliveryManDataSo, _itemFactory);
            
            return guest;
        }
    }
}