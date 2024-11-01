using UnityEngine;

namespace Units.Modules.MovementModules.Abstract
{
    public interface IMovementModuleWithoutNavMeshAgent
    {
        
    }
    
    public abstract class MovementModuleWithoutNavMeshAgent : MovementModule, IMovementModuleWithoutNavMeshAgent
    {
        protected abstract CapsuleCollider2D capsuleCollider2D { get; }

        protected readonly int collisionLayerMask = LayerMaskParserModule.CollisionLayerMask;
        
        protected void MoveWithCollision(Transform transform, Vector3 move, ref Vector3 direction)
        {
            Vector3 originalPosition = transform.position;
            var moveX = new Vector3(move.x, 0, 0);
            var moveY = new Vector3(0, move.y, 0);

            if (HandleCollision(capsuleCollider2D, originalPosition, ref moveX, ref direction))
                transform.position += moveX;

            if (HandleCollision(capsuleCollider2D, originalPosition, ref moveY, ref direction))
                transform.position += moveY;
        }

        protected abstract bool HandleCollision(CapsuleCollider2D collider, Vector3 originalPosition, ref Vector3 move, ref Vector3 direction);

#if UNITY_EDITOR
        protected static void DebugDrawCircle(Vector3 position, float radius, Color color)
        {
            const int segments = 20;
            const float increment = 360f / segments;
            var angle = 0f;

            Vector3 lastPoint = position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;
            angle += increment;

            for (var i = 0; i < segments; i++)
            {
                Vector3 nextPoint = position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;
                Debug.DrawLine(lastPoint, nextPoint, color);
                lastPoint = nextPoint;
                angle += increment;
            }
        }
#endif
    }
}