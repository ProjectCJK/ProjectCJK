using System;

namespace Modules.DataStructures
{
    public class Node<T>
    {
        public T Data { get; set; }
        public Node<T> Next { get; set; }

        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }

    public class PriorityLinkedList<T>
    {
        private Node<T> head;

        public void Add(T data, int priority)
        {
            var newNode = new Node<T>(data);

            if (head == null || priority < GetPriority(head.Data)) // 우선순위가 더 높은 경우 맨 앞에 삽입
            {
                newNode.Next = head;
                head = newNode;
            }
            else
            {
                Node<T> current = head;

                // 우선순위가 같은 경우 혹은 더 낮은 경우 적절한 위치를 찾음
                while (current.Next != null && priority >= GetPriority(current.Next.Data))
                {
                    current = current.Next;
                }

                newNode.Next = current.Next;
                current.Next = newNode;
            }
        }

        public T Dequeue() // 가장 우선순위 높은 항목을 가져옴
        {
            if (head == null) throw new InvalidOperationException("Queue is empty");

            Node<T> temp = head;
            head = head.Next;
            return temp.Data;
        }

        // 우선순위를 비교하는 메서드 (임의로 구현)
        private int GetPriority(T data)
        {
            // 실제 우선순위 값을 가져오는 로직을 구현해야 함
            return (int)(object)data; // enum을 int로 변환했다고 가정
        }
    }
}