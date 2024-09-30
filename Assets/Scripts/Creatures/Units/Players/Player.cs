using Creatures.Abstract;

namespace Creatures.Units.Players
{
    public class Player : Creature
    {
        private PlayerMovementSystem _playerMovementSystem;
        
        private void Awake()
        {
            _playerMovementSystem = GetComponent<PlayerMovementSystem>();
        }
    }
}