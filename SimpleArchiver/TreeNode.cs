using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleArchiver
{
    class TreeNode : IComparable<TreeNode>
    {
        public TreeNode childA;
        public TreeNode childB;

        public byte value;
        public long weight;
        public bool hasValue = false;

        public TreeNode(long _weight, byte _value)
        {
            hasValue = true;
            weight = _weight;
            value = _value;
        }

        public TreeNode(long _weight, TreeNode _childA, TreeNode _childB)
        {
            weight = _weight;
            childA = _childA;
            childB = _childB;
        }

        public int CompareTo(TreeNode node)
        {
            if (this.weight > node.weight)
                return 1;
            else if (this.weight < node.weight)
                return -1;
            return 0;
        }
    }
}
