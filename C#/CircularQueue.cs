using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

class CircularQueue<T> : IEnumerable
{
    private T[] _container;
    private int _head;
    private int _tail;
    private int _size;

    public CircularQueue(int initialCapacity = 4)  // Set a default initial capacity
    {
        _container = new T[initialCapacity];
        _head = 0;
        _tail = 0;
        _size = 0;
    }

    public int Count => _size;
    
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

    private void ContainerResize(int capacity)
    {
        const int GrowFactor = 2;

        int newCapacity = GrowFactor * _container.Length;
        
        SetContainer(newCapacity);
    }

    public void Clear()
    {
        Debug.Assert(_size != 0);

        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            if (_head < _tail)
            {
                for (int i = _head; i < _head + _size; i++)
                {
                    _container[i] = default(T);
                }
            }
            else
            {
                for (int i = _head; i < _container.Length; ++i)
                {
                    _container[i] = default(T);
                }
                for (int i = 0; i < _tail; ++i)
                {
                    _container[i] = default(T);
                }
            }
            
            _size = 0;
        }

        _head = 0;
        _tail = 0;
    }

    public bool Contains(T value)
    {
        Debug.Assert(_size != 0);

        if (_head < _tail)
        {
            return Array.IndexOf(_container, value, _head, _size) >= 0;
        }
        
        return Array.IndexOf(_container, value, _head, _container.Length - _head) >= 0 || Array.IndexOf(_container, value, 0, _tail) >= 0;
    }

    public T Dequeue()
    {
        Debug.Assert(_size != 0);
        
        int head = _head;
        T[] tmpContainer = _container;

        T removed = tmpContainer[head];
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            tmpContainer[head] = default;
        }
        
        MoveIndexPosition(ref _head);
        --_size;

        return removed;
    }

    public void Enqueue(T value)
    {
        if (_size == _container.Length)
        {
            ContainerResize(_size + 1);
        }

        _container[_tail] = value;
        MoveIndexPosition(ref _tail);
        ++_size;
    }

    public T Peek()
    {
        Debug.Assert(_size != 0);

        return _container[_head];
    }

    private void SetContainer(int capacity)
    {
        T[] tmpContainer = new T[capacity];
        if (_size > 0)
        {
            CustomArrayCopy(_container, _head, tmpContainer, 0, _size);
        }
        else
        {
            CustomArrayCopy(_container, _head, tmpContainer, 0, _container.Length - _head);
            CustomArrayCopy(_container, 0, tmpContainer, _container.Length - _head, _tail);
        }

        _container = tmpContainer;
        _head = 0;
        _tail = (_size == capacity) ? 0 : _size;
    }
    
    private void MoveIndexPosition(ref int index)
    {
        int tmp = index + 1;
        if (tmp == _container.Length)
        {
            tmp = 0;
        }

        index = tmp;
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        int current = _head;
        for (int i = 0; i < _size; ++i)
        {
            yield return _container[current];
            current = (current + 1) % _container.Length;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}