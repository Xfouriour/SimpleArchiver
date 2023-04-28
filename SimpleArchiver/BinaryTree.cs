using System;
using System.Collections.Generic;

namespace SimpleArchiver
{
    class BinaryTree //класс для хранения кодов сжатия
    {
        private List<TreeNode> nodeList;
        private TreeNode decodeNode;

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
            BuildTree();
        }

        //функция постройки полного древа на основании листа нод
        //создаем родительскую ноду, присваивая две самые малые к ней как родители, затем сортируем
        private int BuildTree() 
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
        private int SearchNode(Stack<short> stack, TreeNode node, short childA, byte value)
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
            Stack<short> treePath = new Stack<short>(256);
            //в path хранится путь до значения представляющий собой двоичный код байта
            Dictionary<byte, short[]> codeTable = new Dictionary<byte, short[]>();

            for (short i = 0; i < frequency.Length; i++)
            {
                if (frequency[i] > 0)
                {
                    SearchNode(treePath, nodeList[0], -1, (byte)i);
                    short[] dump = treePath.ToArray();
                    short[] reversedDump = new short[dump.Length];
                    for (int t = 0; t < dump.Length; t++)
                    {
                        reversedDump[t] = dump[dump.Length - 1 - t];
                    }
                    codeTable.Add((byte)i, reversedDump);
                    treePath.Clear();
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
            }
            else
            {
                decodeNode = decodeNode.childB;
            }

            if (decodeNode.hasValue)
            {
                short value = decodeNode.value;
                decodeNode = null;
                return value;
            }
            return -1;
        }
    }
}
