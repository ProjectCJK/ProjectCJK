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
            Vector2 remainingMove = move;

            // 한 축씩 나누어 충돌 검사
            Vector2 adjustedMoveX = new Vector2(remainingMove.x, 0);
            Vector2 adjustedMoveY = new Vector2(0, remainingMove.y);

            // X축 방향 충돌 검사
            if (HandleCollision(BoxCollider2D, originalPosition, adjustedMoveX, ref direction))
            {
                // X축 충돌이 발생하면 X축 이동 차단
                adjustedMoveX = Vector2.zero;
            }

            // Y축 방향 충돌 검사
            if (HandleCollision(BoxCollider2D, originalPosition + adjustedMoveX, adjustedMoveY, ref direction))
            {
                // Y축 충돌이 발생하면 Y축 이동 차단
                adjustedMoveY = Vector2.zero;
            }

            // 최종 이동 벡터 적용
            Vector2 finalMove = adjustedMoveX + adjustedMoveY;
            rigidbody2D.MovePosition(originalPosition + finalMove);
        }

        private bool HandleCollision(BoxCollider2D collider, Vector2 originalPosition, Vector2 move, ref Vector2 direction)
        {
            Vector2 colliderPosition = originalPosition + collider.offset;
            RaycastHit2D hit = Physics2D.CircleCast(colliderPosition, collider.size.y / 2, move.normalized, move.magnitude, CollisionLayerMask);

            if (hit.collider != null)
            {
                direction = Vector2.Reflect(direction, hit.normal);
                return true; // 충돌 발생
            }
            return false; // 충돌 없음
        }
    }
}