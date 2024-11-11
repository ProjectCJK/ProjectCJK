using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.DataStructures
{
    public class PriorityQueue<T>
    {
        private readonly SortedDictionary<int, Queue<T>> _queue = new();

        public int Count { get; private set; }

        // 항목을 우선순위에 따라 큐에 추가
        public void Enqueue(T item, int priority)
        {
            if (!_queue.ContainsKey(priority)) _queue[priority] = new Queue<T>();

            _queue[priority].Enqueue(item);
            Count++;
        }

        // 가장 우선순위가 높은 항목을 큐에서 제거하고 반환
        public T Dequeue()
        {
            if (_queue.Count == 0) throw new InvalidOperationException("Queue is empty");

            KeyValuePair<int, Queue<T>> firstPair = _queue.First();
            T item = firstPair.Value.Dequeue();

            if (firstPair.Value.Count == 0) _queue.Remove(firstPair.Key);

            Count--;
            return item;
        }

        // 가장 우선순위가 높은 항목을 반환하지만 큐에서 제거하지 않음
        public T Peek()
        {
            if (_queue.Count == 0) throw new InvalidOperationException("Queue is empty");

            return _queue.First().Value.Peek();
        }

        // 큐에 특정 항목이 포함되어 있는지 확인
        public bool Contains(T item)
        {
            return _queue.Any(pair => pair.Value.Contains(item));
        }

        // 큐를 비움
        public void Clear()
        {
            _queue.Clear();
            Count = 0;
        }

        // 항목을 큐에서 제거하지 않고 반환, 실패 시 null을 반환
        public bool TryPeek(out T item)
        {
            if (_queue.Count > 0)
            {
                item = _queue.First().Value.Peek();
                return true;
            }

            item = default;
            return false;
        }

        // 항목을 큐에서 안전하게 제거하고 반환, 실패 시 false 반환
        public bool TryDequeue(out T item)
        {
            if (_queue.Count > 0)
            {
                KeyValuePair<int, Queue<T>> firstPair = _queue.First();
                item = firstPair.Value.Dequeue();

                if (firstPair.Value.Count == 0) _queue.Remove(firstPair.Key);

                Count--;
                return true;
            }

            item = default;
            return false;
        }

        public bool Remove(T item)
        {
            foreach (var priority in _queue.Keys.ToList()) // 우선순위를 반복하기 위해 키를 목록으로 복사합니다.
            {
                Queue<T> queue = _queue[priority];

                if (queue.Contains(item))
                {
                    // 항목을 제거한 새로운 큐를 만들어 기존 큐를 대체
                    var newQueue = new Queue<T>(queue.Where(x => !x.Equals(item)));
                    if (newQueue.Count > 0)
                        _queue[priority] = newQueue; // 수정된 큐로 대체
                    else
                        _queue.Remove(priority); // 큐가 비어있으면 우선순위 키 제거

                    Count--;
                    return true;
                }
            }

            return false;
        }
    }
}