using System.Collections;
using System.Collections.Generic;
namespace task03
{
    public class CustomCollection<T> : IEnumerable<T>
    {
        private readonly List<T> _items = new();

        public void Add(T item) => _items.Add(item);
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<T> GetReverseEnumerator()
        {
            List<T> element = new List<T>();
            foreach (T el in this)
            {
                element.Add(el);
            }
            for (int i = 0; i < element.Count; i++)
            {
                yield return element[element.Count - 1 - i];
            }
        }
        public static IEnumerable<int> GenerateSequence(int start, int count)
        {
            for (int i = start; i < start + count; i++)
            {
                yield return i;
            }
        }
        public IEnumerable<T> FilterAndSort(Func<T, bool> predicate, Func<T, IComparable> keySelector)
        {
            var result = this.Where(predicate).OrderBy(keySelector);
            return result;
        }
    }
}
