using System;
using Interfaces;
using JetBrains.Annotations;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Items;
using Units.Stages.Units.Items.Modules;
using UnityEngine;

namespace Units.Stages.Units.Items.Units
{
    public interface IItemTransfer
    {
        public void Transfer(Vector3 pointA, Vector3 pointB, Action onArrived);
        public void Transfer(Transform pointA, Transform pointB, Action onArrived);
        public void Transfer(Transform pointA, Vector3 pointB, Action onArrived);
    }
    
    public interface IItem : IRegisterReference<ItemDataSO>, IInitializable<Sprite, Vector3>, IPoolable, IItemTransfer
    {
        public Transform CurrentTransform { get; }
    }

    public class Item : MonoBehaviour, IItem
    {
        public Transform CurrentTransform => transform;
        private SpriteRenderer _spriteRenderer;
        private BezierCurveMover _bezierCurveMover;
        private bool _isInitialized;

        public void RegisterReference(ItemDataSO _itemDataSo)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _bezierCurveMover = new BezierCurveMover(this, _itemDataSo.BaseMovementSpeed);
        }

        public void Initialize(Sprite sprite, Vector3 initialPosition)
        {
            transform.position = initialPosition;
            SetSprite(sprite);
            SetActive(true);
            _isInitialized = true;
        }

        public void Transfer(Vector3 pointA, Vector3 pointB, Action onArrived)
        {
            _bezierCurveMover.Transfer(pointA, pointB, onArrived);
        }

        public void Transfer(Transform pointATransform, Transform pointBTransform, Action onArrived)
        {
            _bezierCurveMover.Transfer(pointATransform, pointBTransform, onArrived);
        }

        public void Transfer(Transform pointA, Vector3 pointB, Action onArrived)
        {
            _bezierCurveMover.Transfer(pointA, pointB, onArrived);
        }

        private void Update()
        {
            if (!_isInitialized) return;
            
            _bezierCurveMover.Transfer();
        }

        public void Create()
        {
            Reset();
        }

        public void GetFromPool()
        {
            Reset();
        }

        public void ReturnToPool()
        {
            Reset();
        }

        private void Reset()
        {
            SetSprite(null);
            SetActive(false);
            _isInitialized = false;
        }
        
        private void SetActive(bool value)
        {
            if (gameObject.activeInHierarchy != value) gameObject.SetActive(value);
        }

        private void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
    }
}