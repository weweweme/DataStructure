using System.Collections;

class CustomStack<T> : IEnumerable
{
    private T[] _container;
    private int _size;

    public CustomStack()
    {
        _container = new T[4];
    }

    public int Count => _size;
    public int Capacity => _container.Length;
    
    private void ContainerResize(int newSize)
    {
        T[] newContainer = new T[newSize];
        CustomArrayCopy(_container, 0, newContainer, 0, _size);
        _container = newContainer;
    }

    private void CustomArrayCopy(T[] sourceContainer, int sourceIndex, T[] destinationContainer, int destinationIndex, int length)
    {
        for (int i = 0; i < length; ++i)
        {
            destinationContainer[destinationIndex + i] = sourceContainer[sourceIndex + i];
        }
    }

    public T Peek()
    {
        if (_size == 0)
        {
            throw new InvalidOperationException("스택이 비어 있습니다.");
        }

        return _container[_size - 1];
    }

    public T Pop()
    {
        if (_size == 0)
        {
            throw new InvalidOperationException("스택이 비어 있습니다.");
        }

        int index = _size - 1;
        T value = _container[index];

        _container[index] = default(T);

        --_size;

        if (_size < _container.Length / 4)
        {
            ContainerResize(_container.Length / 2);
        }

        return value;
    }
    
    public void Push(T value)
    {
        if (_size == _container.Length)
        {
            int newSize = (_size == 0) ? 1 : _size * 2;
            ContainerResize(newSize);
        }

        _container[_size] = value;
        ++_size;
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
}