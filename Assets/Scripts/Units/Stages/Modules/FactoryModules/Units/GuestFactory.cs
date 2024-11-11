using System;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.FactoryModules.Abstract;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Units.Stages.Modules.FactoryModules.Units
{
    public interface IGuestFactory
    {
        public GuestDataSO GuestDataSo { get; }
        public IGuest GetGuest(Vector3 startPosition, Action<IGuest> onReturn);
        public void ReturnGuest(Guest guest);
    }

    public class GuestFactory : NPCFactory, IGuestFactory
    {
        private const int DefaultPoolSize = 20;
        private const int MaxPoolSize = 20;

        private readonly IItemFactory _itemFactory;

        public GuestFactory(IItemFactory itemFactory)
        {
            _itemFactory = itemFactory;

            CreateGuestPools();
        }

        private static string PoolKey => "GuestPool";
        public GuestDataSO GuestDataSo => DataManager.Instance.GuestDataSo;

        public IGuest GetGuest(Vector3 startPosition, Action<IGuest> onReturn)
        {
            var guest = ObjectPoolManager.Instance.GetObject<IGuest>(PoolKey, null);

            guest.Initialize(startPosition, () =>
                {
                    ObjectPoolManager.Instance.ReturnObject(PoolKey, guest);

                    onReturn?.Invoke(guest);
                },
                GetRandomSprites(GuestDataSo.CreatureSprites));

            return guest;
        }

        public void ReturnGuest(Guest guest)
        {
            ObjectPoolManager.Instance.ReturnObject(PoolKey, guest);
        }

        private void CreateGuestPools()
        {
            ObjectPoolManager.Instance.CreatePool(PoolKey, DefaultPoolSize, MaxPoolSize, true,
                () => InstantiateGuest(GuestDataSo.prefab));
        }

        private IGuest InstantiateGuest(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            var guest = obj.GetComponent<IGuest>();

            guest.RegisterReference(GuestDataSo, _itemFactory);

            return guest;
        }
    }
}