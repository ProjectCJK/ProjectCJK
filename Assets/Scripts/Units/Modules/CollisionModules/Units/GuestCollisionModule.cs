using System;
using System.Collections.Generic;
using Units.Modules.CollisionModules.Abstract;
using Units.Stages.Units.Buildings.Modules;
using UnityEngine;

namespace Units.Modules.CollisionModules.Units
{
    public interface IGuestCollisionModule
    {
        public event Action<IInteractionTrade, bool> OnTriggerTradeZone;
        public event Func<string, bool, bool> OnCheckValidTransform;
        public void OnTriggerEnter2D(Collider2D other);
        public void OnTriggerStay2D(Collider2D other);
        public void OnTriggerExit2D(Collider2D other);
    }

    public class GuestCollisionModule : CollisionModule, IGuestCollisionModule
    {
        public event Action<IInteractionTrade, bool> OnTriggerTradeZone;
        public event Func<string, bool, bool> OnCheckValidTransform;
        
        private readonly float _waitingTime;
        
        private Transform _targetTransform;
        private Transform _previousTargetTransform; // 이전 트리거 존을 저장
        private float _elapsedTime;  // 시간 측정 변수
        private bool _isWaitingForTrigger; // 대기 상태 여부
        
        public GuestCollisionModule(IInteractionProperty guestStatModule)
        {
            _waitingTime = guestStatModule.WaitingTime;    
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            switch (CheckLayer(other.gameObject.layer))
            {
                case ECollisionType.None:
                    break;
                case ECollisionType.TradeZone:
                    var tradeZone = other.transform.GetComponent<IInteractionTrade>();
                    
                    if (tradeZone != null && OnCheckValidTransform != null && OnCheckValidTransform(tradeZone.BuildingKey, true))
                    {
                        Debug.Log($"[CompleteWaitForTrigger] Entering trade zone: {tradeZone}");
                        OnTriggerTradeZone?.Invoke(tradeZone, true);
                    }
                    break;
            }
        }

        public void OnTriggerStay2D(Collider2D other)
        {
            
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            switch (CheckLayer(other.gameObject.layer))
            {
                case ECollisionType.None:
                    break;
                case ECollisionType.TradeZone:
                    if (_targetTransform != null && ReferenceEquals(_targetTransform, other.transform))
                    {
                        _targetTransform = null;
                    }
                    break;
            }
        }
    }
}