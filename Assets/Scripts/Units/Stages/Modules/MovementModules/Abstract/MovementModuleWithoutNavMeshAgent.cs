using UnityEngine;

namespace Units.Stages.Modules.MovementModules.Abstract
{
    public interface IMovementModuleWithoutNavMeshAgent
    {
    }

    public abstract class MovementModuleWithoutNavMeshAgent : MovementModule, IMovementModuleWithoutNavMeshAgent
    {
        protected abstract BoxCollider2D BoxCollider2D { get; }
        
        protected int CollisionLayerMask { get; private set; }

        // 충돌 레이어 마스크 설정 메서드
        public void SetCollisionLayerMask(int layerMask)
        {
            CollisionLayerMask = layerMask;
        }

        protected void MoveWithCollision(Rigidbody2D rigidbody2D, Vector2 move, ref Vector2 direction)
        {
            Vector2 originalPosition = rigidbody2D.position;
            bool collisionDetected = HandleCollision(BoxCollider2D, originalPosition, move, ref direction);

            if (!collisionDetected)
            {
                rigidbody2D.MovePosition(originalPosition + move); // 충돌이 없을 경우 전체 이동 허용
            }
            // else
            // {
            //     Vector2 adjustedMove = AdjustMoveForCollision(move, direction);
            //     rigidbody2D.MovePosition(originalPosition + adjustedMove); // 충돌 시 남은 축으로만 이동
            // }
        }

        private bool HandleCollision(BoxCollider2D collider, Vector2 originalPosition, Vector2 move, ref Vector2 direction)
        {
            Vector2 colliderPosition = originalPosition + collider.offset;
            RaycastHit2D hit = Physics2D.CircleCast(colliderPosition, collider.size.y / 2, move.normalized, move.magnitude, CollisionLayerMask);

            if (hit.collider != null)
            {
                direction = Vector2.Reflect(direction, hit.normal);
                return true;
            }
            return false;
        }

        private Vector2 AdjustMoveForCollision(Vector2 move, Vector2 collisionNormal)
        {
            // 충돌면의 수직 벡터로 이동 벡터를 투영하여 벽을 따라 이동하도록 조정
            Vector2 perpendicularToCollision = Vector2.Perpendicular(collisionNormal);
            float projectionMagnitude = Vector2.Dot(move, perpendicularToCollision);

            // 충돌면을 기준으로 이동 벡터를 투영한 결과를 반환
            return perpendicularToCollision.normalized * projectionMagnitude;
        }
    }
}