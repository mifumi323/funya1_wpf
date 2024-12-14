using System.Collections;

namespace funya1_wpf
{
    public class RangeArray<T> : IEnumerable<KeyValuePair<int, T>>
    {
        private readonly T[] array;

        public int MinIndex { get; }
        public int MaxIndex { get; }
        public int Count { get; }

        public RangeArray(int minIndex, int maxIndex)
        {
            MinIndex = minIndex;
            MaxIndex = maxIndex;
            Count = MaxIndex - MinIndex + 1;
            array = new T[Count];
        }

        public RangeArray(int minIndex, int maxIndex, Func<int, T> generator)
        {
            MinIndex = minIndex;
            MaxIndex = maxIndex;
            Count = MaxIndex - MinIndex + 1;
            array = new T[Count];
            for (int i = 0; i < Count; i++)
            {
                array[i] = generator(i + MinIndex);
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < MinIndex || MaxIndex < index)
                {
                    throw new IndexOutOfRangeException();
                }
                return array[index - MinIndex];
            }
            set
            {
                if (index < MinIndex || MaxIndex < index)
                {
                    throw new IndexOutOfRangeException();
                }
                array[index - MinIndex] = value;
            }
        }

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return new KeyValuePair<int, T>(i + MinIndex, array[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
