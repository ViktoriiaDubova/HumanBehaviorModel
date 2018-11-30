using System;
using System.Linq;
using System.Collections.Generic;

namespace HBM.Web.Extensions
{
    public static class CollectionHelpers
    {
        public static void ForAll<T>(this T[] array, Func<T, T> transform)
        {
            for (int i = 0; i < array.Length; i++)
            {
                var item = array[i];
                array[i] = transform(item);
            }
        }
        public static void RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                T element = collection.ElementAt(i);
                if (predicate(element))
                {
                    collection.Remove(element);
                    i--;
                }
            }
        }
        public static void RemoveAll<T>(this ICollection<T> collection, IEnumerable<T> items, Func<T,T,bool> comparer)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                T element = collection.ElementAt(i);
                if (items.FirstOrDefault(item => comparer(item, element)) != null)
                {
                    collection.Remove(element);
                    i--;
                }
            }
        }
        public static void Intersection<T>(this ICollection<T> collection, IEnumerable<T> items, Func<T,T,bool> comparer)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                T element = collection.ElementAt(i);
                if (items.FirstOrDefault(item => comparer(item, element)) == null)
                {
                    collection.Remove(element);
                    i--;
                }
            }
            foreach (var item in items)
            {
                if (collection.FirstOrDefault(element => comparer(element, item)) == null)
                    collection.Add(item);
            }
        }
    }
}