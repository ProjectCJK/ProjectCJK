using ScriptableObjects.Scripts.Creatures.Units;
using Units.Modules.MovementModules.Abstract;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;

namespace Units.Modules.StatsModules.Units.Creatures.Units
{
    public interface IGuestStatModule : IMovementProperty, ICreatureTypeProperty
    {
        
    }
    
    public class GuestStatModule : IGuestStatModule
    {
        public ECreatureType Type => _guestDataSo.type;
        public float MovementSpeed => _guestDataSo.BaseMovementSpeed;
        
        private readonly GuestDataSO _guestDataSo;
        
        public GuestStatModule(GuestDataSO guestDataSo)
        {
            _guestDataSo = guestDataSo;
        }
    }
}