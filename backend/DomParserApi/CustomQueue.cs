using DomParserApi.Models;

namespace DomParserApi.DataStructures
{
    internal class QueueNode
    {
        public HtmlNode Data { get; set; }
        public QueueNode Next { get; set; }
    }

    public class CustomQueue
    {
        private QueueNode _head;
        private QueueNode _tail;

        public void Enqueue(HtmlNode node)
        {
            QueueNode newNode = new QueueNode { Data = node, Next = null };
            if (_tail != null)
            {
                _tail.Next = newNode;
            }
            _tail = newNode;
            if (_head == null)
            {
                _head = _tail;
            }
        }

        public HtmlNode Dequeue()
        {
            if (_head == null) return null;
            HtmlNode node = _head.Data;
            _head = _head.Next;
            if (_head == null)
            {
                _tail = null;
            }
            return node;
        }

        public bool IsEmpty()
        {
            return _head == null;
        }
    }
}