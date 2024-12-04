using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Managers;
using Units.Stages.Modules.BattleModules.Abstract;
using Units.Stages.Units.Creatures.Interfaces;
using UnityEngine;

namespace Units.Stages.Modules.BattleModules
{
    public interface IAttackTrigger : IRegisterReference<IBattleProperty, LayerMask, List<EBattleTag>, Action>,
        IInitializable<bool>
    {
        public event Action OnHitSuccessful;
    }

    public class AttackTrigger : MonoBehaviour, IAttackTrigger
    {
        public Animator Animator { get; private set; }

        private IBattleProperty _battleProperty;
        private BoxCollider2D _boxCollider2D;
        private LayerMask _targetLayerMask;
        private bool _attackFlag;
        private Coroutine _attackCoroutine;

        private int _damage => _battleProperty.Damage;
        private float _attackDelay => _battleProperty.AttackDelay;

        public event Action OnHitSuccessful;
        public event Action OnInvokeAnimationEvent;

        private readonly Collider2D[] _targetsBuffer = new Collider2D[50]; // 물리 탐지 결과 버퍼

        private const string targetTag = "Monster";

        public void RegisterReference(IBattleProperty battleProperty, LayerMask targetLayerMask,
            List<EBattleTag> targetTags, Action handleOnInvokeAnimationEvent)
        {
            _battleProperty = battleProperty;
            _targetLayerMask = targetLayerMask;

            Animator = GetComponent<Animator>();
            _boxCollider2D = GetComponent<BoxCollider2D>();

            OnInvokeAnimationEvent += handleOnInvokeAnimationEvent;
        }

        public void Initialize(bool value)
        {
            if (_attackFlag != value)
            {
                _attackFlag = value;

                // 기존 코루틴 중지
                if (_attackCoroutine != null)
                {
                    CoroutineManager.Instance.StopCoroutine(_attackCoroutine);
                    _attackCoroutine = null;
                }

                // 새 공격 루틴 시작 또는 종료
                if (_attackFlag)
                    _attackCoroutine = CoroutineManager.Instance.StartCoroutine(AttackRoutine());
                else
                    _boxCollider2D.enabled = false;
            }
        }

        private IEnumerator AttackRoutine()
        {
            while (_attackFlag)
            {
                // 범위 내 모든 적 탐지
                AttackAllTargetsInRange();

                // 딜레이 동안 대기
                yield return new WaitForSeconds(_attackDelay);
            }
        }

        private void AttackAllTargetsInRange()
        {
            var targetCount = Physics2D.OverlapBoxNonAlloc(
                transform.position,
                _boxCollider2D.size,
                0,
                _targetsBuffer,
                _targetLayerMask // 필요한 레이어만 탐지
            );

            for (var i = 0; i < targetCount; i++)
            {
                Collider2D target = _targetsBuffer[i];

                // 대상 태그 및 인터페이스 확인
                if (target.CompareTag(targetTag) && target.TryGetComponent(out ITakeDamage damageable) && damageable.TakeDamage(_damage))
                {
                    // 성공적으로 데미지를 입혔을 때 이벤트 발생
                    OnHitSuccessful?.Invoke();
                }
            }
        }

        public void OnAnimationEvent()
        {
            // 애니메이션 이벤트 발생 시 처리
            OnInvokeAnimationEvent?.Invoke();
        }

        private void OnDrawGizmos()
        {
            // 디버그용으로 공격 범위 시각화
            if (_boxCollider2D != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position, _boxCollider2D.size);
            }
        }
    }
}