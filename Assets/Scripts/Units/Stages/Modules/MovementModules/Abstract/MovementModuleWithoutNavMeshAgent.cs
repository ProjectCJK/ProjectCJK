using UnityEngine;

namespace Units.Stages.Modules.MovementModules.Abstract
{
    public interface IMovementModuleWithoutNavMeshAgent
    {
    }

    public abstract class MovementModuleWithoutNavMeshAgent : MovementModule, IMovementModuleWithoutNavMeshAgent
    {
        protected abstract BoxCollider2D BoxCollider2D { get; }
        
        private readonly int collisionLayerMask = LayerMaskParserModule.CollisionLayerMask;

        protected void MoveWithCollision(Rigidbody2D rigidbody2D, Vector2 move, ref Vector2 direction)
        {
            Vector2 originalPosition = rigidbody2D.position;
            bool collisionDetected = HandleCollision(BoxCollider2D, originalPosition, move, ref direction);

            if (!collisionDetected)
            {
                rigidbody2D.MovePosition(originalPosition + move);
            }
        }

        private bool HandleCollision(BoxCollider2D collider, Vector2 originalPosition, Vector2 move, ref Vector2 direction)
        {
            Vector2 colliderPosition = originalPosition + collider.offset;
            RaycastHit2D hit = Physics2D.CircleCast(colliderPosition, collider.size.y / 2, move.normalized, move.magnitude, collisionLayerMask);

            if (hit.collider != null)
            {
                direction = Vector2.Reflect(direction, hit.normal);
                return true; // 충돌 발생
            }
            return false; // 충돌 없음
        }
    }
}