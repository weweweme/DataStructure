public class CustomHashSet<T>
{
    private struct Entry
    {
        public int HashCode;
        public T Value;
        public int Next;
    }
    
    private readonly IEqualityComparer<T>? _comparer;

    private Entry[] _entries;
    private int[] _container;
    private const int DefaultCapacity = 4;
    private int _count;
    
    public int Count => _count;

    public CustomHashSet(int capacity = DefaultCapacity, IEqualityComparer<T>? comparer = null)
    {
        _entries = new Entry[capacity];
        _container = new int[capacity];
        for (int i = 0; i < _container.Length; i++)
        {
            _container[i] = -1;
        }

        _comparer = comparer;
    }


    public bool Add(T item)
    {
        int hashCode = item.GetHashCode();
        if (hashCode < 0)
        {
            hashCode = -hashCode;
        }
        
        int index = hashCode % _container.Length;

        for (int i = _container[index]; i >= 0; i = _entries[i].Next)
        {
            if (_entries[i].HashCode == hashCode && _entries[i].Value.Equals(item))
            {
                return false;
            }
        }

        if (_count == _entries.Length)
        {
            ResizeContainer();
            
            index = hashCode % _container.Length;
        }

        _entries[_count] = new Entry
        {
            HashCode = hashCode,
            Value = item,
            Next = _container[index]
        };
        
        _container[index] = _count;
        ++_count;
        
        return true;
    }

    public void Clear()
    {
        if (_count > 0)
        {
            for (int i = 0; i < _container.Length; ++i)
            {
                _container[i] = default;
            }

            for (int i = 0; i < _entries.Length; ++i)
            {
                _entries[i] = default;
            }
            
            _count = 0;
        }
    }

    private void CustomArrayCopy<T>(T[] sourceArray, int sourceIndex, T[] destinationArray, int destinationIndex, int length)
    {
        if (sourceIndex < destinationIndex)
        {
            int srcIdx = sourceIndex + length - 1;
            int destIdx = destinationIndex + length - 1;
            for (int i = 0; i < length; i++)
            {
                destinationArray[destIdx - i] = sourceArray[srcIdx - i];
            }
        }
        else
        {
            int srcIdx = sourceIndex;
            int destIdx = destinationIndex;
            for (int i = 0; i < length; i++)
            {
                destinationArray[destIdx + i] = sourceArray[srcIdx + i];
            }
        }
    }
    
    public bool Contains(T value)
    {
        int hashCode = value.GetHashCode();
        if (hashCode < 0)
        {
            hashCode = -hashCode;
        }
        
        int index = hashCode % _container.Length;

        for (int i = _container[index]; i >= 0; i = _entries[i].Next)
        {
            if (_entries[i].HashCode == hashCode && EqualityComparer<T>.Default.Equals(_entries[i].Value, value))
            {
                return true;
            }
        }

        return false;
    }
    
    private void ResizeContainer()
    {
        int newSize = _count * 2;
        Entry[] newEntries = new Entry[newSize];
        int[] newContainer = new int[newSize];
        for (int i = 0; i < newContainer.Length; ++i)
        {
            newContainer[i] = -1;
        }

        CustomArrayCopy(_entries, 0, newEntries, 0, _count);

        for (int i = 0; i < _count; ++i)
        {
            int index = newEntries[i].HashCode % newSize;
            newEntries[i].Next = newContainer[index];
            newContainer[index] = i;
        }

        _entries = newEntries;
        _container = newContainer;
    }

    public bool Remove(T value)
    {
        int hashCode = value.GetHashCode();
        if (hashCode < 0)
        {
            hashCode = -hashCode;
        }
    
        int index = hashCode % _container.Length;
        int last = -1;

        for (int i = _container[index]; i >= 0; last = i, i = _entries[i].Next)
        {
            if (_entries[i].HashCode == hashCode && EqualityComparer<T>.Default.Equals(_entries[i].Value, value))
            {
                if (last < 0)
                {
                    _container[index] = _entries[i].Next;
                }
                else
                {
                    _entries[last].Next = _entries[i].Next;
                }

                _entries[i] = default;

                --_count;

                return true;
            }
        }

        return false;
    }

    public bool TryGetValue(T equalValue, out T actualValue)
    {
        int hashCode = equalValue.GetHashCode();
        if (hashCode < 0)
        {
            hashCode = -hashCode;
        }
    
        int index = hashCode % _container.Length;

        for (int i = _container[index]; i >= 0; i = _entries[i].Next)
        {
            if (_entries[i].HashCode == hashCode && EqualityComparer<T>.Default.Equals(_entries[i].Value, equalValue))
            {
                actualValue = _entries[i].Value;
                return true;
            }
        }

        actualValue = default(T);
        return false;
    }
    
    public IEqualityComparer<T> Comparer
    {
        get
        {
            if (typeof(T) == typeof(string) && _comparer is IEqualityComparer<string>)
            {
                return (IEqualityComparer<T>)EqualityComparer<string>.Default;
            }
            else
            {
                return _comparer ?? EqualityComparer<T>.Default;
            }
        }
    }
}