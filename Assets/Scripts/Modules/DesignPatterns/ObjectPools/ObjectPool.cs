using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.DesignPatterns.ObjectPools
{
    /// <summary>
    /// 제네릭 오브젝트 풀 클래스. 오브젝트를 생성하고 관리하며, 객체의 생명 주기를 처리합니다.
    /// 오브젝트 풀링, 재사용성, 파괴 기능을 제공합니다.
    /// </summary>
    /// <typeparam name="T">풀링할 객체 타입 (IPoolable을 구현해야 함)</typeparam>
    public class ObjectPool<T> : ICustomObjectPool<T> where T : IPoolable
    {
        /// <summary>
        /// 풀 내부에서 관리되는 큐. 오브젝트를 보관합니다.
        /// </summary>
        private readonly Queue<T> poolQueue = new();

        /// <summary>
        /// 오브젝트를 생성하는 팩토리 메서드. 오브젝트가 부족할 경우 새로 생성합니다.
        /// </summary>
        private readonly Func<T> objectFactory;

        /// <summary>
        /// 풀의 최대 크기
        /// </summary>
        private readonly int maxSize;

        /// <summary>
        /// 풀의 크기가 Flexible하면, 최대 크기를 넘어설 수 있습니다.
        /// </summary>
        private readonly bool isFlexible;

        /// <summary>
        /// 지정된 초기 크기와 최대 크기, 유연성 여부로 오브젝트 풀을 생성합니다.
        /// </summary>
        /// <param name="initialSize">초기 풀 크기</param>
        /// <param name="maxSize">풀의 최대 크기</param>
        /// <param name="isFlexible">true면 최대 크기를 넘어설 수 있고, false면 최대 크기를 넘지 않습니다.</param>
        /// <param name="objectFactory">오브젝트 팩토리 함수</param>
        public ObjectPool(int initialSize, int maxSize, bool isFlexible, Func<T> objectFactory)
        {
            this.objectFactory = objectFactory;
            this.maxSize = maxSize;
            this.isFlexible = isFlexible;

            // 초기 풀 크기만큼 오브젝트를 생성하여 큐에 추가합니다.
            for (var i = 0; i < initialSize; i++)
            {
                T obj = objectFactory();
                obj.Create();  // 객체 생성 시 초기화
                poolQueue.Enqueue(obj);
            }
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져옵니다. 
        /// 오브젝트가 부족하면 Flexible 여부에 따라 새로 생성하거나 null을 반환합니다.
        /// </summary>
        /// <returns>풀에서 가져온 오브젝트</returns>
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
                T obj = objectFactory();
                obj.GetFromPool();
                return obj;
            }

            // 풀에 오브젝트가 없고, 유연하지 않다면 null 반환
            return default;
        }
        
        /// <summary>
        /// 풀에 있는 모든 객체를 리스트로 반환합니다.
        /// </summary>
        /// <returns>풀에 있는 모든 객체 리스트</returns>
        public List<T> GetAllObjects()
        {
            return new List<T>(poolQueue);  // Queue를 리스트로 변환하여 반환
        }

        /// <summary>
        /// 오브젝트를 풀로 반환합니다.
        /// 반환된 오브젝트는 재사용할 수 있도록 초기화됩니다.
        /// </summary>
        /// <param name="obj">반환할 오브젝트</param>
        public void ReturnObject(T obj)
        {
            if (poolQueue.Count < maxSize || isFlexible)
            {
                obj.ReturnToPool();
                obj.Reset();
                poolQueue.Enqueue(obj);
            }
            else
            {
                // 최대 크기를 넘어서면 오브젝트를 제거
                DestroyObject(obj);
            }
        }

        /// <summary>
        /// 모든 풀링된 오브젝트를 파괴하고 풀을 비웁니다.
        /// </summary>
        public void DestroyPool()
        {
            while (poolQueue.Count > 0)
            {
                T obj = poolQueue.Dequeue();
                DestroyObject(obj);
            }
        }

        /// <summary>
        /// 오브젝트를 파괴합니다. GameObject인 경우 Destroy 호출.
        /// </summary>
        /// <param name="obj">파괴할 오브젝트</param>
        private void DestroyObject(T obj)
        {
            obj.ReturnToPool();
            var gameObject = obj as GameObject;
            if (gameObject != null)
            {
                UnityEngine.Object.Destroy(gameObject);
            }
        }
    }
}