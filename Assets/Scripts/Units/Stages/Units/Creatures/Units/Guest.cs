using Interfaces;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures;
using Units.Modules.FSMModules.Units;
using Units.Modules.MovementModules.Abstract;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using UnityEngine;

namespace Units.Stages.Units.Creatures.Units
{
    public interface IGuest : IPoolable, IRegisterReference<GuestDataSO>, IInitializable
    {
    }
    
    public class Guest : Creature, IGuest
    {
        public override ECreatureType CreatureType { get; }
        public override Animator Animator { get; protected set; }
        protected override CreatureStateMachine creatureStateMachine { get; set; }
        
        private GuestStatModule _guestStatModule;
        private GuestMovementMule _guestMovementMule;
        
        public void RegisterReference(GuestDataSO guestDataSo)
        {
            _guestStatModule = new GuestStatModule(guestDataSo);
            _guestMovementMule = new GuestMovementMule(_guestStatModule);
        }
        
        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        private void Update()
        {
            
        }
        
        public void Create()
        {
            
        }

        public void GetFromPool()
        {
            
        }

        public void ReturnToPool()
        {
            
        }
    }

    public interface IGuestStatModule : IMovementProperty
    {
        
    }
    
    public class GuestStatModule : IGuestStatModule
    {
        public float MovementSpeed { get; }
        
        public GuestStatModule(GuestDataSO guestDataSo)
        {
            MovementSpeed = guestDataSo.BaseMovementSpeed;
        }
    }

    public class GuestMovementMule
    {
        public GuestMovementMule(IMovementProperty guestStatModule)
        {
            
        }
    }
}