using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GEDWrap;

namespace DrawTreeTest
{
    public class DataSet
    {
        private Person _root;
        private List<UnionData> _tree;
        private int _dex;
        private TreeNodeModel<UnionData> _rootTreeNode;

        public DataSet(Person root)
        {
            _root = root;
        }

        public void GetDescendants()
        {
            _tree = new List<UnionData>();
            _dex = 0;
            GetDescendants(_root, -1);
        }

        private void GetDescendants(Person thisroot, int parentNodeId)
        {
            UnionData thisNode = new UnionData();
            thisNode.Id = _dex;
            _dex = _dex + 1;
            thisNode.PersonId = thisroot.Id;
            thisNode.ParentId = parentNodeId;
            thisNode.Who = thisroot;
            _tree.Add(thisNode);

            // Navigation to parents - for root only
            if (parentNodeId == -1 && thisroot.ChildIn.Count > 0)
            {
                foreach (var union1 in thisroot.ChildIn)
                {
                    thisNode.Parents.Add(union1.DadId);
                    thisNode.Parents.Add(union1.MomId);
                }
            }

            // Non-root node: multiple parents
            if (thisroot.ChildIn.Count > 1 && parentNodeId != -1)
            {
                thisNode.CurrentParents = 0;
                foreach (var union1 in thisroot.ChildIn)
                {
                    thisNode.Parents.Add(union1.DadId);
                    thisNode.Parents.Add(union1.MomId);
                }
            }

            // Is this node a union or a person?
            if (thisroot.SpouseIn.Count < 1)
            {
                // Not married: no descendants, no spouse
                thisNode.IsUnion = false;
                return;
            }

            // Is a union. Initialize to the first marriage.
            thisNode.IsUnion = true;
            var union = thisroot.SpouseIn.First();
            thisNode.CurrentMarriage = thisroot.SpouseIn.Count > 1 ? 0 : -1;
            thisNode.UnionId = union.Id;
            Person spouse = union.Spouse(thisroot);
            thisNode.Spouse = spouse;
            thisNode.SpouseId = spouse == null ? "??" : spouse.Id;

            // Marriage may appear more than once. If already exists,
            // link this node to the first one, and do NOT fetch children
            foreach (var item in _tree)
            {
                if (item.UnionId == thisNode.UnionId &&
                    item.Id != thisNode.Id)
                {
                    thisNode.Link = item.Id;
                    return;
                }
            }

            // Fetch children of union
            foreach (var achild in union.Childs)
            {
                GetDescendants(achild, thisNode.Id);
            }
        }

        public TreeNodeModel<UnionData> GetTree()
        {
            var root = _tree[0];
            Debug.Assert(root.ParentId == -1);

            _rootTreeNode = new TreeNodeModel<UnionData>(root, null);
            _rootTreeNode.Children = GetChildNodes(_rootTreeNode);

            return _rootTreeNode;
        }

        private List<TreeNodeModel<UnionData>> GetChildNodes(TreeNodeModel<UnionData> parent)
        {
            var nodes = new List<TreeNodeModel<UnionData>>();

            foreach (var item in _tree.Where(p => p.ParentId == parent.Item.Id))
            {
                var treeNode = new TreeNodeModel<UnionData>(item, parent);
                treeNode.Children = GetChildNodes(treeNode);
                nodes.Add(treeNode);
            }

            return nodes;
        }

        public void Remove(UnionData node)
        {
            // Remove this node AND all its children
            List<UnionData> toDel = new List<UnionData>();
            foreach (var cnode in _tree)
            {
                if (cnode.ParentId == node.Id)
                    toDel.Add(cnode);
            }
            foreach (var unionData in toDel)
            {
                Remove(unionData);
            }
            _tree.Remove(node);
        }

        public void Replace(UnionData node, Union u)
        {
            // A node is being re-created (alternate union).
            // Replace the existing one and add the new child sub-trees.
            // NOTE: a replace is necessary to preserve the order of
            // the person in the tree!

            UnionData old = null;
            for (int i = 0; i < _tree.Count; i++)
            {
                if (_tree[i].Id == node.Id)
                {
                    old = _tree[i];
                    _tree[i] = node;
                }
            }
            Remove(old); // TODO is this even necessary?

            foreach (var achild in u.Childs)
            {
                GetDescendants(achild, node.Id);
            }
        }
    }
}
