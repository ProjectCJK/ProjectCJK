using System;
using System.Collections.Generic;

namespace Modules
{
    public class PriorityQueue<T>
    {
        private List<(T Element, int Priority)> _heap = new();

        public int Count => _heap.Count;

        public void Enqueue(T item, int priority)
        {
            _heap.Add((item, priority));
            HeapifyUp(_heap.Count - 1);
        }

        public T Dequeue()
        {
            if (_heap.Count == 0) throw new InvalidOperationException("Queue is empty");
        
            T top = _heap[0].Element;
            _heap[0] = _heap[^1];
            _heap.RemoveAt(_heap.Count - 1);
            HeapifyDown(0);
            return top;
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                var parentIndex = (index - 1) / 2;
                if (_heap[index].Priority >= _heap[parentIndex].Priority) break;
            
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void HeapifyDown(int index)
        {
            var lastIndex = _heap.Count - 1;
            while (index < lastIndex)
            {
                var leftChildIndex = 2 * index + 1;
                var rightChildIndex = 2 * index + 2;

                if (leftChildIndex > lastIndex) break;

                var smallerChildIndex = rightChildIndex > lastIndex || _heap[leftChildIndex].Priority < _heap[rightChildIndex].Priority
                    ? leftChildIndex
                    : rightChildIndex;

                if (_heap[index].Priority <= _heap[smallerChildIndex].Priority) break;

                Swap(index, smallerChildIndex);
                index = smallerChildIndex;
            }
        }

        private void Swap(int i, int j)
        {
            (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
        }
    }
}