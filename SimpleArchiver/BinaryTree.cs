using System;
using System.Collections.Generic;

namespace Archiver
{
    class BinaryTree //класс для хранения кодов сжатия
    {
        private List<TreeNode> nodeList;
        private TreeNode decodeNode;

        private class TreeNode: IComparable<TreeNode>
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

        private class Stack
        {
            short size;
            short[] dump;
            short index;
            public Stack(short _size)
            {
                dump = new short[_size];
                size = _size;
                index = 0;
            }

            public void Push(short value)
            {
                if (index >= size)
                    return;
                dump[index] = value;
                index++;
            }

            public short Pop()
            {
                if (index < 1)
                    return -1;
                return dump[--index];
            }
            
            public short[] GetDump(out short position)
            {
                position = index;
                return dump;
            }

            public void Reset()
            {
                index = 0;
            }
        }

        //формируем коллекцию нод с указанием их "веса", для последующей постройки дерева
        public BinaryTree(FrequencyTable frequency)
        {
            nodeList = new List<TreeNode>();
            for (short i = 0; i < frequency.Length; i++)
            {
                if (frequency[i] > 0)
                    nodeList.Add(new TreeNode(frequency[i], (byte)i));
            }
            nodeList.Sort();
        }

        //функция постройки полного древа на основании листа нод
        //создаем родительскую ноду, присваивая две самые малые к ней как родители, затем сортируем
        public int BuildTree() 
        {
            while (nodeList.Count > 1)
            {
                TreeNode A = nodeList[0];
                TreeNode B = nodeList[1];
                nodeList.RemoveRange(0, 2);
                nodeList.Add(new TreeNode(A.weight + B.weight, A, B));
                nodeList.Sort();
            }
            return 0;
        }

        //рекурсивный поиск пути к нужной ноде и запись в стек
        private int SearchNode(Stack stack, TreeNode node, short childA, byte value)
        {
            if (node == null)
                return -1;
            if (childA >= 0)
                stack.Push(childA);
            if ((node.value == value && node.hasValue) || SearchNode(stack, node.childA, 1, value) > 0 || SearchNode(stack, node.childB, 0, value) > 0)
                return 1;
            stack.Pop();
            return -1;
        }

        //кодирование знака в биты, возвращаем биты в буфере и количество бит в результате
        public Dictionary<byte, short[]> GetCodeTable(FrequencyTable frequency)
        {
            Stack treePath = new Stack(256);
            //в path хранится путь до значения представляющий собой двоичный код байта
            Dictionary<byte, short[]> codeTable = new Dictionary<byte, short[]>();

            for (short i = 0; i < frequency.Length; i++)
            {
                if (frequency[i] > 0)
                {
                    SearchNode(treePath, nodeList[0], -1, (byte)i);
                    short length = 0;
                    short[] dump = treePath.GetDump(out length);
                    short[] resizedDump = new short[length];
                    for (short t = 0; t < length; t++)
                    {
                        resizedDump[t] = dump[t];
                    }
                    codeTable.Add((byte)i, resizedDump);
                    treePath.Reset();
                }
            }
            //возвращаем таблицу с кодами
            return codeTable;
        }

        //обход всего дерева для составления кода. самая медленная часть распаковки
        //выполняется 1 раз на каждый бит
        //с каждым вызовом идем в дерево дальше, выбирая направление в зависмости от полученного параметра
        //входные параметры 1 и 0
        public short Decode(byte childA)
        {
            if (decodeNode == null)
            {
                decodeNode = nodeList[0];
            }
            if (childA == 1)
            {
                decodeNode = decodeNode.childA;
                if (decodeNode.childA == null || decodeNode.childB == null)
                {
                    short value = decodeNode.value;
                    decodeNode = null;
                    return value;
                }
            }
            else
            {
                decodeNode = decodeNode.childB;
                if (decodeNode.childA == null && decodeNode.childB == null)
                {
                    short value = decodeNode.value;
                    decodeNode = null;
                    return value;
                }
            }
            return -1;
        }
    }
}
