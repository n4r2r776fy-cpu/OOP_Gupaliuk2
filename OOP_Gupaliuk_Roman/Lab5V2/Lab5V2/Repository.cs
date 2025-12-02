using System;
using System.Collections.Generic;
using System.Linq;

public class Repository<T>
{

    private List<T> items = new List<T>();

    // Додати елемент
    public void Add(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Елемент не може бути null");
        items.Add(item);
    }

    // Отримати всі елементи
    public List<T> GetAll()
    {
        return items;
    }

    // Знайти елементи за умовою
    public IEnumerable<T> Find(Func<T, bool> predicate)
    {
        return items.Where(predicate);
    }
}
