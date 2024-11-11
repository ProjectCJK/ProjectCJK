using System;
using Interfaces;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Items;
using Units.Stages.Units.Items.Modules;
using UnityEngine;

namespace Units.Stages.Units.Items.Units
{
    public interface IItemTransfer
    {
        public void Transfer(Vector3 pointA, Vector3 pointB, Action onArrived);
        public void Transfer(Vector3 pointA, Transform pointB, Action onArrived);
    }

    public interface IItem : IRegisterReference<ItemDataSO>, IInitializable<string, int, Sprite, Vector3>, IPoolable,
        IItemTransfer
    {
        public string Type { get; }
        public int Count { get; }
        public Transform Transform { get; }
    }

    public class Item : MonoBehaviour, IItem
    {
        private BezierCurveMover _bezierCurveMover;
        private bool _isInitialized;
        private SpriteRenderer _spriteRenderer;

        private void Reset()
        {
            SetSprite(null);
            SetActive(false);
            Type = null;
            Count = 0;
            _isInitialized = false;
        }

        private void Update()
        {
            if (!_isInitialized) return;

            _bezierCurveMover.Transfer();
        }

        public string Type { get; private set; }
        public int Count { get; private set; }

        public Transform Transform => transform;

        public void RegisterReference(ItemDataSO _itemDataSo)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _bezierCurveMover = new BezierCurveMover(this, _itemDataSo.BaseMovementSpeed);
        }

        public void Initialize(string type, int count, Sprite sprite, Vector3 initialPosition)
        {
            Type = type;
            Count = count;
            transform.position = initialPosition;
            SetSprite(sprite);
            SetActive(true);
            _isInitialized = true;
        }

        public void Transfer(Vector3 pointA, Vector3 pointB, Action onArrived)
        {
            _bezierCurveMover.Transfer(pointA, pointB, onArrived);
        }

        public void Transfer(Vector3 pointA, Transform pointBTransform, Action onArrived)
        {
            _bezierCurveMover.Transfer(pointA, pointBTransform, onArrived);
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