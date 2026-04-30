namespace VeriYapilariProje
{
    // Hocanın "from scratch" (sıfırdan) istediği kuyruk yapısı
    public class MyQueue<T>
    {
        private class Node
        {
            public T Data;
            public Node Next;
            public Node(T data) { Data = data; }
        }

        private Node head, tail;
        public int Count { get; private set; }

        // Kuyruğun sonuna eleman ekleme
        public void Enqueue(T item)
        {
            Node newNode = new Node(item);
            if (tail != null) tail.Next = newNode;
            tail = newNode;
            if (head == null) head = newNode;
            Count++;
        }

        // Kuyruğun başından eleman çıkarma
        public T Dequeue()
        {
            if (head == null) throw new System.InvalidOperationException("Kuyruk boş!");
            T data = head.Data;
            head = head.Next;
            if (head == null) tail = null;
            Count--;
            return data;
        }
    }
}