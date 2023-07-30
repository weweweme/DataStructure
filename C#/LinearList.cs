using System.Collections;

class LinearList<T> : IEnumerable<T>
{
    private T[] _container = Array.Empty<T>();
    private int _size = 0;

    public LinearList()
    {
        
    }

    private LinearList(int size)
    {
        _size = size;
        _container = new T[_size];
    }

    public T this[int index]
    {
        get 
        {
            if (index < 0 || index >= _size)
                throw new IndexOutOfRangeException();
                
            return _container[index];
        }
        set 
        {
            if (index < 0 || index >= _size)
                throw new IndexOutOfRangeException();

            _container[index] = value;
        }
    }

    public int Count
    {
        get { return _size; }
    }

    private void ContainerResize(int newSize)
    {
        T[] newContainer = new T[newSize];
        CustomArrayCopy(_container, 0, newContainer, 0, _size);
        _container = newContainer;
    }
    
    private void CustomArrayCopy(T[] sourceContainer, int sourceIndex, T[] destinationContainer, int destinationIndex, int length)
    {
        if (sourceIndex < destinationIndex)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                destinationContainer[destinationIndex + i] = sourceContainer[sourceIndex + i];
            }
        }
        else
        {
            for (int i = 0; i < length; i++)
            {
                destinationContainer[destinationIndex + i] = sourceContainer[sourceIndex + i];
            }
        }
    }

    public void Add(T value)
    {
        if (_size == _container.Length)
        {
            ContainerResize(_size == 0 ? 1 : _size * 2);
        }

        _container[_size] = value;
        ++_size;
    }

    public void AddRange(IEnumerable<T> collection)
    {
        InsertRange(_size, collection);
    }

    public void Clear()
    {
        for (int i = 0; i < _size; ++i)
        {
            _container[i] = default(T);
        }
    }

    public bool Contains(T value)
    {
        return _size != 0 && IndexOf(value) != -1;
    }

    public LinearList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
    {
        LinearList<TOutput> outputList = new LinearList<TOutput>(_size);
        for (int i = 0; i < _size; i++)
        {
            outputList.Add(converter(_container[i]));
        }
        return outputList;
    }

    public T Find(Predicate<T> targetValue)
    {
        for (int i = 0; i < _size; ++i)
        {
            if (targetValue(_container[i]))
            {
                return _container[i];
            }
        }

        return default;
    }

    public LinearList<T> FindAll(Predicate<T> targetValue)
    {
        LinearList<T> list = new LinearList<T>();
        for (int i = 0; i < _size; ++i)
        {
            if (targetValue(_container[i]))
            {
                list.Add(_container[i]);
            }
        }

        return list;
    }

    public void Insert(int index, T value)
    {
        if (index > _size)
        {
            throw new IndexOutOfRangeException();
        }

        if (_size == _container.Length)
        {
            ContainerResize(_size + 1);
        }

        if (index < _size)
        {
            CustomArrayCopy(_container, index, _container, index + 1, _size - index);
        }

        _container[index] = value;
        ++_size;
    }

    private void InsertRange(int index, IEnumerable<T> collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException();
        }
        
        if (index > _size)
        {
            throw new IndexOutOfRangeException();
        }

        if (collection is ICollection<T> c)
        {
            int count = c.Count;
            if (count > 0)
            {
                if (_container.Length - _size < count)
                {
                    ContainerResize(_size + count);
                }

                if (index < _size)
                {
                    CustomArrayCopy(_container, index, _container, index + count, _size - index);
                }
                
                c.CopyTo(_container, index);
                _size += count;
            }
        }
        else
        {
            using (IEnumerator<T> enumerator = collection.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Insert(index++, enumerator.Current);
                }
            }
        }
    }

    public int IndexOf(T value)
    {
        for (int i = 0; i < _size; ++i)
        {
            if (_container[i].Equals(value))
            {
                return i;
            }
        }

        return -1;
    }

    public void RemoveAt(int index)
    {
        if ((uint)index >= (uint)_size)
        {
            throw new IndexOutOfRangeException();
        }

        _size--;
        
        if (index < _size)
        {
            CustomArrayCopy(_container, index + 1, _container, index, _size - index);
        }

        _container[_size] = default;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _size; ++i)
        {
            yield return _container[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Reverse()
    {
        for (int i = 0; i < _size / 2; ++i)
        {
            T temp = _container[i];
            _container[i] = _container[_size - i - 1];
            _container[_size - i - 1] = temp;
        }
    }
}