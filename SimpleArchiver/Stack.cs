using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleArchiver
{
    class Stack
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
}
