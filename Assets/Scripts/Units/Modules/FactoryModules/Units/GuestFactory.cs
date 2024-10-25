using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures;
using Units.Modules.FactoryModules.Abstract;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Modules.FactoryModules.Units
{
    public interface IGuestFactory
    {
        public GuestDataSO GuestDataSo { get; }
        public string PoolKey { get; }
        public void CreateGuest();
    }
    
    public class GuestFactory : Factory, IGuestFactory
    {
        public GuestDataSO GuestDataSo => DataManager.Instance.GuestData;
        public string PoolKey => "GuestPool";
        
        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;
        
        public void CreateGuest()
        {
            ObjectPoolManager.Instance.CreatePool(((IGuestFactory)this).PoolKey, DefaultPoolSize, MaxPoolSize, true, () => InstantiateGuest(GuestDataSo.prefab));
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