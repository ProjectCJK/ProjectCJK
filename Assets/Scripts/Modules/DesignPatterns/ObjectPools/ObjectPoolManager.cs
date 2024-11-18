using System;
using System.Collections.Generic;
using Modules.DesignPatterns.Singletons;
using UnityEngine;

namespace Modules.DesignPatterns.ObjectPools
{
    /// <summary>
    /// ObjectPoolManager - 여러 오브젝트 풀을 관리하는 클래스.
    /// MonoBehaviour에 의존하지 않으며, 싱글톤 패턴을 사용하여 인스턴스를 관리합니다.
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        private const int DefaultPoolSize = 10;
        private const int MaxPoolSize = 10;

        /// <summary>
        /// 키와 풀을 연결하는 딕셔너리.
        /// </summary>
        private readonly Dictionary<string, object> poolDictionary = new();

        /// <summary>
        /// 요청한 키에 해당하는 풀에서 오브젝트를 가져옵니다. 풀을 찾을 수 없으면 새로 생성합니다.
        /// </summary>
        public T GetObject<T>(string key, Func<T> objectFactory) where T : IPoolable
        {
            if (!poolDictionary.ContainsKey(key)) CreatePool(key, DefaultPoolSize, MaxPoolSize, true, objectFactory);

            if (poolDictionary[key] is ObjectPool<T> pool) return pool.GetObject();
            throw new InvalidCastException($"The pool with key '{key}' does not match the requested type.");
        }
        
        /// <summary>
        /// 특정 키에 해당하는 모든 객체를 가져옵니다.
        /// </summary>
        public List<T> GetAllObjects<T>(string key) where T : IPoolable
        {
            if (poolDictionary.TryGetValue(key, out var pool) && pool is ObjectPool<T> typedPool)
            {
                return typedPool.GetAllObjects(); // ObjectPool에서 모든 객체를 가져옴
            }
            else
            {
                throw new Exception($"Pool with key '{key}' does not exist or is of a different type.");
            }
        }

        /// <summary>
        /// 요청한 키에 해당하는 풀로 오브젝트를 반환합니다.
        /// </summary>
        public void ReturnObject<T>(string key, T obj) where T : IPoolable
        {
            if (poolDictionary.TryGetValue(key, out var pool) && pool is ObjectPool<T> typedPool)
                typedPool.ReturnObject(obj);
            else
                throw new Exception($"Pool with key '{key}' does not exist or is of a different type.");
        }

        /// <summary>
        /// 특정 키에 해당하는 모든 객체들을 반환합니다.
        /// </summary>
        public void ReturnAllObjects<T>(string key) where T : IPoolable
        {
            if (poolDictionary.TryGetValue(key, out var pool) && pool is ObjectPool<T> typedPool)
            {
                List<T> allObjects = typedPool.GetAllObjects();
                foreach (var obj in allObjects)
                {
                    typedPool.ReturnObject(obj);
                }
            }
            else
            {
                throw new Exception($"Pool with key '{key}' does not exist or is of a different type.");
            }
        }

        /// <summary>
        /// 새로운 오브젝트 풀을 생성합니다.
        /// </summary>
        public void CreatePool<T>(string key, int initialSize, int maxSize, bool isFlexible, Func<T> objectFactory)
            where T : IPoolable
        {
            var newPool = new ObjectPool<T>(initialSize, maxSize, isFlexible, objectFactory);
            poolDictionary[key] = newPool;
        }

        /// <summary>
        /// 부모 Transform을 지정하여 새로운 오브젝트 풀을 생성합니다.
        /// </summary>
        public void CreatePool<T>(string key, int initialSize, int maxSize, bool isFlexible, Func<T> objectFactory, Transform parentTransform)
            where T : IPoolable
        {
            var newPool = new ObjectPool<T>(initialSize, maxSize, isFlexible, objectFactory, parentTransform);
            poolDictionary[key] = newPool;
        }

        /// <summary>
        /// 요청한 키에 해당하는 오브젝트 풀을 제거하고 풀 내부의 모든 오브젝트를 파괴합니다.
        /// </summary>
        public void DestroyPool<T>(string key) where T : IPoolable
        {
            if (poolDictionary.TryGetValue(key, out var pool) && pool is ObjectPool<T> typedPool)
            {
                typedPool.DestroyPool();
                poolDictionary.Remove(key);
            }
            else
            {
                throw new Exception($"Pool with key '{key}' does not exist or is of a different type.");
            }
        }
    }
}