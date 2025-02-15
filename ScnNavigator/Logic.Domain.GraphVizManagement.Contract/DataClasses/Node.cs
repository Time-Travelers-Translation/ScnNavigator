namespace Logic.Domain.GraphVizManagement.Contract.DataClasses
{
    public class Node
    {
        private readonly List<Node> _children = new();
        private readonly List<Node> _parents = new();

        public string Text { get; set; }

        public IList<Node> Children => _children;
        public IList<Node> Parents => _parents;

        public void AddChild(Node child)
        {
            if (!Children.Contains(child))
                Children.Add(child);

            if (!child.Parents.Contains(this))
                child.Parents.Add(this);
        }
    }

    public class Node<TData> : Node
    {
        public TData Data { get; }

        public Node(TData data)
        {
            Data = data;
        }
    }
}
