using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Modules.DesignPatterns.ObjectPools
{
    /// <summary>
    ///     제네릭 오브젝트 풀 클래스. 오브젝트를 생성하고 관리하며, 객체의 생명 주기를 처리합니다.
    ///     오브젝트 풀링, 재사용성, 파괴 기능을 제공합니다.
    /// </summary>
    /// <typeparam name="T">풀링할 객체 타입 (IPoolable을 구현해야 함)</typeparam>
    public class ObjectPool<T> : ICustomObjectPool<T> where T : IPoolable
    {
        private readonly bool isFlexible;
        private readonly int maxSize;
        private readonly Func<T> objectFactory;
        private readonly Transform parentTransform;
        private readonly Queue<T> poolQueue = new();

        public ObjectPool(int initialSize, int maxSize, bool isFlexible, Func<T> objectFactory,
            Transform parentTransform = null)
        {
            this.objectFactory = objectFactory;
            this.maxSize = maxSize;
            this.isFlexible = isFlexible;
            this.parentTransform = parentTransform;

            // 초기 풀 크기만큼 오브젝트를 생성하여 큐에 추가합니다.
            for (var i = 0; i < initialSize; i++)
            {
                T obj = CreateObject();
                poolQueue.Enqueue(obj);
            }
        }

        public T GetObject()
        {
            if (poolQueue.Count > 0)
            {
                T obj = poolQueue.Dequeue();
                obj.GetFromPool();
                return obj;
            }

            if (isFlexible || poolQueue.Count < maxSize)
            {
                T obj = CreateObject();
                obj.GetFromPool();
                return obj;
            }

            return default;
        }

        public void ReturnObject(T obj)
        {
            if (poolQueue.Count < maxSize || isFlexible)
            {
                obj.ReturnToPool();

                // MonoBehaviour를 상속한 경우, 부모 Transform 확인 및 설정
                if (obj is MonoBehaviour monoObject && monoObject.transform.parent != parentTransform)
                    monoObject.transform.SetParent(parentTransform);

                poolQueue.Enqueue(obj);
            }
            else
            {
                DestroyObject(obj);
            }
        }

        /// <summary>
        ///     오브젝트를 생성하고, 지정된 부모 Transform이 있다면 그 부모로 설정합니다.
        /// </summary>
        /// <returns>새로 생성된 오브젝트</returns>
        private T CreateObject()
        {
            T obj = objectFactory();
            obj.Create();

            // GameObject인지 체크하고, MonoBehaviour를 상속한 경우 부모 설정
            if (obj is MonoBehaviour monoObject && parentTransform != null)
                monoObject.transform.SetParent(parentTransform);

            return obj;
        }

        public List<T> GetAllObjects()
        {
            return new List<T>(poolQueue);
        }

        public void DestroyPool()
        {
            while (poolQueue.Count > 0)
            {
                T obj = poolQueue.Dequeue();
                DestroyObject(obj);
            }
        }

        private void DestroyObject(T obj)
        {
            obj.ReturnToPool();
            if (obj is MonoBehaviour monoObject) Object.Destroy(monoObject.gameObject);
        }
    }
}