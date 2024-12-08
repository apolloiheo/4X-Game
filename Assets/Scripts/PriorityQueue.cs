using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<(T Item, int Priority)> _elements = new List<(T, int)>();

    public int Count => _elements.Count;

    public void Enqueue(T item, int priority)
    {
        _elements.Add((item, priority));
        _elements.Sort((x, y) => x.Priority.CompareTo(y.Priority)); // Sort by priority
    }

    public T Dequeue()
    {
        if (Count == 0)
            throw new InvalidOperationException("The queue is empty.");

        var element = _elements[0];
        _elements.RemoveAt(0);
        return element.Item;
    }

    public bool Contains(T item)
    {
        return _elements.Exists(e => EqualityComparer<T>.Default.Equals(e.Item, item));
    }
}
