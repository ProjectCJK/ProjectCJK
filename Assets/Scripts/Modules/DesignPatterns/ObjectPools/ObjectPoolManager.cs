using System;
using System.Collections.Generic;
using Modules.DesignPatterns.Singletons;

namespace Modules.DesignPatterns.ObjectPools
{
    /// <summary>
    /// ObjectPoolManager - 여러 오브젝트 풀을 관리하는 클래스. 
    /// MonoBehaviour에 의존하지 않으며, 싱글톤 패턴을 사용하여 인스턴스를 관리합니다.
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        /// <summary>
        /// 키와 풀을 연결하는 딕셔너리.
        /// </summary>
        private readonly Dictionary<string, object> poolDictionary = new();

        private const int DefaultPoolSize = 10;
        private const int MaxPoolSize = 10;

        /// <summary>
        /// 요청한 키에 해당하는 풀에서 오브젝트를 가져옵니다. 풀을 찾을 수 없으면 새로 생성합니다.
        /// </summary>
        /// <typeparam name="T">풀링할 객체 타입 (IPoolable을 구현해야 함)</typeparam>
        /// <param name="key">풀의 고유 키</param>
        /// <param name="objectFactory">오브젝트 생성자 함수</param>
        /// <returns>풀에서 가져온 오브젝트</returns>
        public T GetObject<T>(string key, Func<T> objectFactory) where T : IPoolable
        {
            if (!poolDictionary.ContainsKey(key))
            {
                CreatePool(key, DefaultPoolSize, MaxPoolSize, true, objectFactory);
            }

            if (poolDictionary[key] is ObjectPool<T> pool)
            {
                return pool.GetObject();
            }
            throw new InvalidCastException($"The pool with key '{key}' does not match the requested type.");
        }

        public List<T> GetAllObjectsFromPool<T>(string key) where T : IPoolable
        {
            if (poolDictionary.TryGetValue(key, out var pool) && pool is ObjectPool<T> typedPool)
            {
                return typedPool.GetAllObjects();
            }
            throw new Exception($"Pool with key '{key}' does not exist or is of a different type.");
        }

        /// <summary>
        /// 요청한 키에 해당하는 풀로 오브젝트를 반환합니다.
        /// </summary>
        /// <typeparam name="T">풀로 반환할 객체 타입</typeparam>
        /// <param name="key">풀의 고유 키</param>
        /// <param name="obj">반환할 오브젝트</param>
        public void ReturnObject<T>(string key, T obj) where T : IPoolable
        {
            if (poolDictionary.TryGetValue(key, out var pool) && pool is ObjectPool<T> typedPool)
            {
                typedPool.ReturnObject(obj);
            }
            else
            {
                throw new Exception($"Pool with key '{key}' does not exist or is of a different type.");
            }
        }

        /// <summary>
        /// 새로운 오브젝트 풀을 생성합니다.
        /// </summary>
        /// <typeparam name="T">풀링할 객체 타입</typeparam>
        /// <param name="key">풀의 고유 키</param>
        /// <param name="initialSize">초기 풀 크기</param>
        /// <param name="maxSize">풀의 최대 크기</param>
        /// <param name="isFlexible">true면 최대 크기를 넘어설 수 있고, false면 최대 크기를 넘지 않습니다.</param>
        /// <param name="objectFactory">오브젝트 팩토리 함수</param>
        public void CreatePool<T>(string key, int initialSize, int maxSize, bool isFlexible, Func<T> objectFactory) where T : IPoolable
        {
            if (poolDictionary.ContainsKey(key))
            {
                throw new Exception($"Pool with key '{key}' already exists.");
            }

            var newPool = new ObjectPool<T>(initialSize, maxSize, isFlexible, objectFactory);
            poolDictionary[key] = newPool;
        }

        /// <summary>
        /// 요청한 키에 해당하는 오브젝트 풀을 제거하고 풀 내부의 모든 오브젝트를 파괴합니다.
        /// </summary>
        /// <param name="key">풀의 고유 키</param>
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