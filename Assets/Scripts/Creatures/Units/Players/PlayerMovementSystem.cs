using Externals.Joystick.Scripts.Base;
using UnityEngine;

namespace Creatures.Units.Players
{
    public class PlayerMovementSystem : MonoBehaviour
    {
        [SerializeField] private Joystick _joystick;
        [SerializeField] private float _moveSpeed = 50f;
        
        private Rigidbody2D _rigidbody2D;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            Vector2 movement = _joystick.direction * _moveSpeed * Time.deltaTime;
            _rigidbody2D.MovePosition(_rigidbody2D.position + movement);
        }
    }
}