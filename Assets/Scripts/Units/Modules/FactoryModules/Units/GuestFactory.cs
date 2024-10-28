using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Modules.FactoryModules.Abstract;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Modules.FactoryModules.Units
{
    public interface IGuestFactory
    {
        public GuestDataSO GuestDataSo { get; }
        public Guest GetGuest();
        public void ReturnGuest(Guest guest);
    }
    
    public class GuestFactory : Factory, IGuestFactory
    {
        public GuestDataSO GuestDataSo => DataManager.Instance.GuestData;
        public string PoolKey => "GuestPool";
        
        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;

        public GuestFactory()
        {
            CreateGuestPools();
        }

        public Guest GetGuest()
        {
            var guest = ObjectPoolManager.Instance.GetObject<Guest>(PoolKey, null);
            guest.Initialize(new Vector3(0, 10, 0));
            
            return guest;
        }

        public void ReturnGuest(Guest guest)
        {
            ObjectPoolManager.Instance.ReturnObject(PoolKey, guest);   
        }
        
        private void CreateGuestPools()
        {
            ObjectPoolManager.Instance.CreatePool(PoolKey, DefaultPoolSize, MaxPoolSize, true, () => InstantiateGuest(GuestDataSo.prefab));
        }
        
        private Guest InstantiateGuest(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            var guest = obj.GetComponent<Guest>();
            
            guest.RegisterReference(GuestDataSo);
            
            return guest;
        }
    }
}