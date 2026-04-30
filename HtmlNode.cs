namespace DomParserProject.Core
{
    public class HtmlNode
    {
        public string Tag { get; set; }
        public string Id { get; set; }
        public string ClassName { get; set; }
        public HtmlNode Parent { get; set; }

        private HtmlNode[] _children;
        public int ChildCount { get; private set; }

        public HtmlNode(string tag, string id = "", string className = "")
        {
            Tag = tag;
            Id = id;
            ClassName = className;
            _children = new HtmlNode[2]; // Initial capacity
            ChildCount = 0;
        }

        public void AddChild(HtmlNode child)
        {
            child.Parent = this;
            
            // Dynamic array expansion
            if (ChildCount == _children.Length)
            {
                HtmlNode[] newArray = new HtmlNode[_children.Length * 2];
                for (int i = 0; i < _children.Length; i++)
                {
                    newArray[i] = _children[i];
                }
                _children = newArray;
            }
            _children[ChildCount] = child;
            ChildCount++;
        }

        public HtmlNode[] GetChildren()
        {
            HtmlNode[] result = new HtmlNode[ChildCount];
            for (int i = 0; i < ChildCount; i++)
            {
                result[i] = _children[i];
            }
            return result;
        }
    }
}