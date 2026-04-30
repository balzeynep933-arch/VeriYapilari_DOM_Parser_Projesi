using DomParserProject.Core;

namespace DomParserProject.DataStructures
{
    internal class StackNode
    {
        public HtmlNode Data { get; set; }
        public StackNode Next { get; set; }
    }

    public class CustomStack
    {
        private StackNode _top;

        public void Push(HtmlNode node)
        {
            StackNode newNode = new StackNode { Data = node, Next = _top };
            _top = newNode;
        }

        public HtmlNode Pop()
        {
            if (_top == null) return null;
            HtmlNode node = _top.Data;
            _top = _top.Next;
            return node;
        }

        public bool IsEmpty()
        {
            return _top == null;
        }
    }
}