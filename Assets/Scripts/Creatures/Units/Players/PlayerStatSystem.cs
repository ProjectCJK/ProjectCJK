using UnityEngine;

namespace Creatures.Units.Players
{
    public class PlayerStatSystem : IMovementStat
    {
        public float MovementSpeed => _movementSpeed;
        
        [SerializeField] private float _movementSpeed;
    }

    public interface IMovementStat
    {
        public float MovementSpeed { get; }
    }
}