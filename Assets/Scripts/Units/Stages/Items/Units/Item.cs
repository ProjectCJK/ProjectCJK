using System;
using Interfaces;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Stages.Items.Modules;
using UnityEngine;

namespace Units.Stages.Items.Units
{
    public interface IItem : IRegisterReference<ItemDataSO>, IInitializable<Sprite, Vector2, Transform, Action>
    {
        
    }
    
    public class Item : MonoBehaviour, IItem, IPoolable
    {
        private SpriteRenderer _spriteRenderer;
        private BezierCurveMover _bezierCurveMover;

        public void RegisterReference(ItemDataSO _itemDataSo)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _bezierCurveMover = new BezierCurveMover(this, _itemDataSo.BaseMovementSpeed);
        }

        public void Initialize(Sprite sprite, Vector2 pointA, Transform pointB, Action action)
        {
            SetSprite(sprite);
            transform.position = pointA;
            _bezierCurveMover.Initialize(pointA, pointB, action);
        }
        
        private void Update()
        {
            _bezierCurveMover.Transfer();
        }

        public void Create()
        {
            Reset();
        }

        public void GetFromPool()
        {
            SetActive(true);
        }

        public void ReturnToPool()
        {
            SetActive(true);
            Reset();
        }

        public void Reset()
        {
            SetActive(false);
            SetSprite(null);
        }
        
        private void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        private void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
    }
}