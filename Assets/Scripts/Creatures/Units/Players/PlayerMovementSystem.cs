using Externals.Joystick.Scripts.Base;
using UnityEngine;

namespace Creatures.Units.Players
{
    public class PlayerMovementSystem : MonoBehaviour
    {
        [SerializeField] private Joystick _joystick;
        
        private PlayerStatSystem _playerStatSystem;
        
        private Rigidbody2D _rigidbody2D;
        
        private float _movementSpeed;
        
        public void RegisterReference(PlayerStatSystem playerStatSystem)
        {
            _playerStatSystem = playerStatSystem;
            
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
        
        public void Initialize()
        {
            _movementSpeed = _playerStatSystem.MovementSpeed;
        }

        private void Update()
        {
            Vector2 movement = _joystick.direction * (_movementSpeed * Time.deltaTime);
            _rigidbody2D.MovePosition(_rigidbody2D.position + movement);
        }
    }
}