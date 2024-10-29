using System;
using System.Collections.Generic;
using Units.Modules.CollisionModules.Abstract;
using Units.Stages.Units.Buildings.Modules;
using UnityEngine;

namespace Units.Modules.CollisionModules.Units
{
    public interface IGuestCollisionModule
    {
        public event Func<Transform, bool, bool> OnTriggerTradeZone;
        public void OnTriggerEnter2D(Collider2D other);
        public void OnTriggerStay2D(Collider2D other);
        public void OnTriggerExit2D(Collider2D other);
    }

    public class GuestCollisionModule : CollisionModule, IGuestCollisionModule
    {
        public event Func<Transform, bool, bool> OnTriggerTradeZone;
        
        private readonly Queue<Transform> _tradeZones = new();
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
                    if (OnTriggerTradeZone != null && OnTriggerTradeZone(other.transform, true))
                    {
                        _targetTransform = other.transform;
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