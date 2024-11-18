using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Modules.DesignPatterns.ObjectPools
{
    /// <summary>
    /// 제네릭 오브젝트 풀 클래스. 오브젝트를 생성하고 관리하며, 객체의 생명 주기를 처리합니다.
    /// </summary>
    public class ObjectPool<T> : ICustomObjectPool<T> where T : IPoolable
    {
        private readonly bool isFlexible;
        private readonly int maxSize;
        private readonly Func<T> objectFactory;
        private readonly Transform parentTransform;
        private readonly Queue<T> poolQueue = new();

        public ObjectPool(int initialSize, int maxSize, bool isFlexible, Func<T> objectFactory, Transform parentTransform = null)
        {
            this.objectFactory = objectFactory;
            this.maxSize = maxSize;
            this.isFlexible = isFlexible;
            this.parentTransform = parentTransform;

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

                if (obj is MonoBehaviour monoObject)
                {
                    if (parentTransform != null)
                        monoObject.transform.SetParent(parentTransform);

                    // monoObject.gameObject.SetActive(true);
                }

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

                if (obj is MonoBehaviour monoObject)
                {
                    if (parentTransform != null)
                        monoObject.transform.SetParent(parentTransform);

                    // monoObject.gameObject.SetActive(false);
                }

                poolQueue.Enqueue(obj);
            }
            else
            {
                DestroyObject(obj);
            }
        }

        private T CreateObject()
        {
            T obj = objectFactory();
            obj.Create();

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
            if (obj is MonoBehaviour monoObject)
                Object.Destroy(monoObject.gameObject);
        }
    }
}