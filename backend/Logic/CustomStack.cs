namespace DomParserAPI.Logic;

public class CustomStack<T>
{
    private readonly List<T> _elements = new();

    // Yığına yeni eleman ekler
    public void Push(T item)
    {
        _elements.Add(item);
    }

    // Yığının en üstündeki elemanı çıkarır ve döndürür
    public T Pop()
    {
        if (IsEmpty()) 
            throw new InvalidOperationException("Stack boş, Pop işlemi yapılamaz.");
            
        T item = _elements[^1]; // En son elemanı al
        _elements.RemoveAt(_elements.Count - 1); // Listeden çıkar
        return item;
    }

    // Çıkarmadan yığının en üstündeki elemana bakar
    public T Peek()
    {
        if (IsEmpty()) 
            throw new InvalidOperationException("Stack boş.");
            
        return _elements[^1];
    }

    public bool IsEmpty()
    {
        return _elements.Count == 0;
    }
}