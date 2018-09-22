using System;

namespace SimpleArchiver
{
    class FrequencyTable //для хранения частоты встречи байта в данных
    {
        private long[] frequency;

        public FrequencyTable()
        {
            frequency = new long[256];
        }

        public void Add(byte _value)
        {
            frequency[_value]++;
        }

        public long this[int index]
        {
            get
            {
                return frequency[index];
            }
            set
            {
                frequency[index] = value;
            }
        }

        public int Length
        {
            get
            {
                return frequency.Length;
            }
        }

    }
}
