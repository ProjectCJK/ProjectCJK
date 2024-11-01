using Interfaces;
using Units.Modules.PaymentModule.Units;
using Units.Stages.Units.Creatures.Abstract;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units.Stages.Units.Buildings.Modules.PaymentZones.Abstract
{
    public interface IPaymentZone : IRegisterReference<IManagementDeskPaymentModule, string>
    {
        public string BuildingKey { get; }
        public bool RegisterPaymentTarget(ICreature creature, bool register);
    }
    
    [RequireComponent(typeof(TilemapCollider2D))]
    public class PaymentZone : MonoBehaviour, IPaymentZone
    {
        public string BuildingKey { get; private set; }
        
        private IManagementDeskPaymentModule _managementDeskPaymentModule;

        public void RegisterReference(IManagementDeskPaymentModule managementDeskPaymentModule, string buildingKey)
        {
            _managementDeskPaymentModule = managementDeskPaymentModule;
            BuildingKey = buildingKey;
        }

        public bool RegisterPaymentTarget(ICreature creature, bool register)
        {
            return _managementDeskPaymentModule.RegisterPaymentTarget(creature, register);
        }
    }
}