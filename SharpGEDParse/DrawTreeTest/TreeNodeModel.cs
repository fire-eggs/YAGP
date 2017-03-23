using System.Collections.Generic;

namespace DrawTreeTest
{
    public class TreeNodeModel<T> where T : class
    {
        public float X { get; set; }
        public int Y { get; set; }
        public float Mod { get; set; }
        public TreeNodeModel<T> Parent { get; set; }
        public List<TreeNodeModel<T>> Children { get; set; }

        public float Width { get; set; }
        public int Height { get; set; }

        public T Item { get; set; }

        public TreeNodeModel(T item, TreeNodeModel<T> parent)
        {
            Item = item;
            Parent = parent;
            Children = new List<TreeNodeModel<T>>();
        }

        public bool IsLeaf()
        {
            return Children.Count == 0;
        }

        public bool IsLeftMost()
        {
            if (Parent == null)
                return true;

            return Parent.Children[0] == this;
        }

        public bool IsRightMost()
        {
            if (Parent == null)
                return true;

            return Parent.Children[Parent.Children.Count - 1] == this;
        }

        public TreeNodeModel<T> GetPreviousSibling()
        {
            if (Parent == null || IsLeftMost())
                return null;

            return Parent.Children[Parent.Children.IndexOf(this) - 1];
        }

        public TreeNodeModel<T> GetNextSibling()
        {
            if (Parent == null || IsRightMost())
                return null;

            return Parent.Children[Parent.Children.IndexOf(this) + 1];
        }

        public TreeNodeModel<T> GetLeftMostSibling()
        {
            if (Parent == null)
                return null;

            if (IsLeftMost())
                return this;

            return Parent.Children[0];
        }

        public TreeNodeModel<T> GetLeftMostChild()
        {
            if (Children.Count == 0)
                return null;

            return Children[0];
        }

        public TreeNodeModel<T> GetRightMostChild()
        {
            if (Children.Count == 0)
                return null;

            return Children[Children.Count - 1];
        }

        public override string ToString()
        {
            return Item.ToString();
        }
    }
}
