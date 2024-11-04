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
    public interface IAttackTrigger : IRegisterReference<IBattleProperty, LayerMask, List<EBattleTag>>, IInitializable<bool>
    {
        public event Action OnHitSuccessful;
    }

    public class AttackTrigger : MonoBehaviour, IAttackTrigger
    {
        public event Action OnHitSuccessful;
        
        private int _damage => _battleProperty.Damage;
        private float _attackDelay => _battleProperty.AttackDelay;
        
        private IBattleProperty _battleProperty;
        
        private BoxCollider2D _boxCollider2D;
        private LayerMask _targetLayerMask;
        private List<EBattleTag> _targetTags;
        private Coroutine _attackCoroutine;
        private bool _attackFlag;
        
        public void RegisterReference(IBattleProperty battleProperty, LayerMask targetLayer, List<EBattleTag> targetTags)
        {
            _battleProperty = battleProperty;
            _targetLayerMask = targetLayer;
            _targetTags = targetTags;
            
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }
        
        public void Initialize(bool value)
        {
            // 활성화 상태가 변경되었을 때만 처리
            if (_attackFlag != value)
            {
                _attackFlag = value;

                // 코루틴이 진행 중이면 중지
                if (_attackCoroutine != null)
                {
                    CoroutineManager.Instance.StopCoroutine(_attackCoroutine);
                    _attackCoroutine = null;
                }

                // 활성화 플래그가 true일 때 코루틴 시작
                if (_attackFlag)
                {
                    _attackCoroutine = CoroutineManager.Instance.StartCoroutine(ActivateCollider());
                }
                else
                {
                    _boxCollider2D.enabled = false; // 비활성화
                }
            }
        }

        private IEnumerator ActivateCollider()
        {
            while (_attackFlag)
            {
                // 콜라이더를 활성화
                _boxCollider2D.enabled = true;
                yield return new WaitForSeconds(0.1f); // 0.1초 동안 활성화 유지

                // 콜라이더를 비활성화
                _boxCollider2D.enabled = false;
                yield return new WaitForSeconds(_attackDelay); // 공격 딜레이 동안 대기
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & _targetLayerMask) != 0) // LayerMask로 충돌 대상 검출
            {
                // 대상 태그와 일치하는 경우에만 처리
                if (_targetTags.Exists(obj => other.gameObject.CompareTag(obj.ToString())))
                {
                    if (other.gameObject.TryGetComponent(out ITakeDamage target))
                    {
                        if (target.TakeDamage(_damage))
                        {
                            OnHitSuccessful?.Invoke();
                        }
                    }
                }
            }
        }
    }
} 