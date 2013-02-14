using System.Diagnostics;
using System.Linq;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Performs the specified action on each element of the IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source list of items.</param>
        /// <param name="action">The System.Action&lt;T&gt; delegate to perform on each element of the IEnumerable&lt;T&gt;.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (action == null)
                throw new ArgumentNullException("action");

            using (IEnumerator<T> data = source.GetEnumerator())
                while (data.MoveNext())
                {
                    action(data.Current);
                }
        }

        /// <summary>
        /// Performs the specified action on each element of the IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source list of items.</param>
        /// <param name="action">The System.Action&lt;T&gt; delegate to perform on each element of the IEnumerable&lt;T&gt;.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (action == null)
                throw new ArgumentNullException("action");

            int index = 0;
            using (IEnumerator<T> data = source.GetEnumerator())
                while (data.MoveNext())
                {
                    action(data.Current, index);
                    index++;
                }
        }

        /// <summary>
        /// Clumps items into same size lots.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source list of items.</param>
        /// <param name="size">The maximum size of the clumps to make.</param>
        /// <returns>A list of list of items, where each list of items is no bigger than the size given.</returns>
        public static IEnumerable<IEnumerable<T>> Clump<T>(this IEnumerable<T> source, int size)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (size < 1)
                throw new ArgumentOutOfRangeException("size", "size must be greater than 0");

            return ClumpIterator<T>(source, size);
        }

        private static IEnumerable<IEnumerable<T>> ClumpIterator<T>(IEnumerable<T> source, int size)
        {
            Debug.Assert(source != null, "source is null.");

            T[] items = new T[size];
            int count = 0;
            foreach (var item in source)
            {
                items[count] = item;
                count++;

                if (count == size)
                {
                    yield return items;
                    items = new T[size];
                    count = 0;
                }
            }
            if (count > 0)
            {
                if (count == size)
                    yield return items;
                else
                {
                    T[] tempItems = new T[count];
                    Array.Copy(items, tempItems, count);
                    yield return tempItems;
                }
            }
        }

        /// <summary>
        /// Creates a list by applying a delegate to pairs of items in the IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source">The source list of items.</param>
        /// <param name="zipFunction">The delegate to use to combine items.</param>
        /// <returns></returns>
        public static IEnumerable<TResult> Scan<T, TResult>(this IEnumerable<T> source, Func<T, T, TResult> combine)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (combine == null)
                throw new ArgumentNullException("combine");

            return ScanIterator<T, TResult>(source, combine);
        }

        private static IEnumerable<TResult> ScanIterator<T, TResult>(IEnumerable<T> source, Func<T, T, TResult> combine)
        {
            Debug.Assert(source != null, "source is null.");
            Debug.Assert(combine != null, "combine is null.");

            using (IEnumerator<T> data = source.GetEnumerator())
                if (data.MoveNext())
                {
                    T first = data.Current;

                    while (data.MoveNext())
                    {
                        yield return combine(first, data.Current);
                        first = data.Current;
                    }
                }
        }

        public static IEnumerable<T> Scanl<T>(this IEnumerable<T> source, Func<T, T, T> combine)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (combine == null)
                throw new ArgumentNullException("combine");

            return ScanlIterator<T>(source, combine);
        }

        private static IEnumerable<T> ScanlIterator<T>(IEnumerable<T> source, Func<T, T, T> combine)
        {
            Debug.Assert(source != null, "source is null.");
            Debug.Assert(combine != null, "combine is null.");

            using (IEnumerator<T> data = source.GetEnumerator())
                if (data.MoveNext())
                {
                    T first = data.Current;
                    yield return first;

                    while (data.MoveNext())
                    {
                        first = combine(first, data.Current);
                        yield return first;
                    }
                }
        }

        /// <summary>
        /// Returns true if there are enough items in the IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source list of items.</param>
        /// <param name="number">The number of items to check for.</param>
        /// <returns>True if there are enough items, false otherwise.</returns>
        public static bool AtLeast<T>(this IEnumerable<T> source, int number)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            int count = 0;
            using (IEnumerator<T> data = source.GetEnumerator())
                while (count < number && data.MoveNext())
                {
                    count++;
                }
            return count == number;
        }

        /// <summary>
        /// Returns true if there are enough items in the IEnumerable&lt;T&gt; to satisfy the condition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source list of items.</param>
        /// <param name="number">The number of items to check for.</param>
        /// <param name="predicate">The condition to apply to the items.</param>
        /// <returns>True if there are enough items, false otherwise.</returns>
        public static bool AtLeast<T>(this IEnumerable<T> source, int number, Func<T, bool> condition)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (condition == null)
                throw new ArgumentNullException("condition");

            int count = 0;
            using (IEnumerator<T> data = source.GetEnumerator())
                while (count < number && data.MoveNext())
                {
                    if (condition(data.Current))
                        count++;
                }
            return count == number;
        }

        /// <summary>
        /// Returns true if there are no more than a set number of items in the IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source list of items.</param>
        /// <param name="number">The number of items that must exist.</param>
        /// <returns>True if there are no more than the number of items, false otherwise.</returns>
        public static bool AtMost<T>(this IEnumerable<T> source, int number)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            bool result;
            int count = 0;
            using (IEnumerator<T> data = source.GetEnumerator())
            {
                while (count < number && data.MoveNext())
                {
                    count++;
                }
                result = !data.MoveNext();
            }
            return result;
        }

        /// <summary>
        /// Returns true if there are no more than a set number of items in the IEnumerable&lt;T&gt; that satisfy a given condition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source list of items.</param>
        /// <param name="number">The number of items that must exist.</param>
        /// <param name="predicate">The condition to apply to the items.</param>
        /// <returns>True if there are no more than the number of items, false otherwise.</returns>
        public static bool AtMost<T>(this IEnumerable<T> source, int number, Func<T, bool> condition)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (condition == null)
                throw new ArgumentNullException("condition");

            bool result;
            int count = 0;
            using (IEnumerator<T> data = source.GetEnumerator())
            {
                while (count < number && data.MoveNext())
                {
                    if (condition(data.Current))
                        count++;
                }
                result = !data.MoveNext();
            }
            return result;
        }

        /// <summary>
        /// Creates a list by combining three other lists into one.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source1">One of the lists to zip.</param>
        /// <param name="source2">One of the lists to zip.</param>
        /// <param name="source3">One of the lists to zip.</param>
        /// <param name="combine">The delegate used to combine the items.</param>
        /// <returns>A new list with the combined items.</returns>
        public static IEnumerable<TResult> Zip<T1, T2, T3, TResult>(this IEnumerable<T1> source1, IEnumerable<T2> source2, IEnumerable<T3> source3, Func<T1, T2, T3, TResult> combine)
        {
            if (source1 == null)
                throw new ArgumentNullException("source1");
            if (source2 == null)
                throw new ArgumentNullException("source2");
            if (source3 == null)
                throw new ArgumentNullException("source3");
            if (combine == null)
                throw new ArgumentNullException("combine");

            return ZipIterator<T1, T2, T3, TResult>(source1, source2, source3, combine);
        }

        private static IEnumerable<TResult> ZipIterator<T1, T2, T3, TResult>(IEnumerable<T1> source1, IEnumerable<T2> source2, IEnumerable<T3> source3, Func<T1, T2, T3, TResult> combine)
        {
            Debug.Assert(source1 != null, "source1 is null.");
            Debug.Assert(source2 != null, "source2 is null.");
            Debug.Assert(source3 != null, "source3 is null.");
            Debug.Assert(combine != null, "combine is null.");

            using (IEnumerator<T1> data1 = source1.GetEnumerator())
            using (IEnumerator<T2> data2 = source2.GetEnumerator())
            using (IEnumerator<T3> data3 = source3.GetEnumerator())
                while (data1.MoveNext() && data2.MoveNext() && data3.MoveNext())
                {
                    yield return combine(data1.Current, data2.Current, data3.Current);
                }
        }

        /// <summary>
        /// Creates a list by combining four other lists into one
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source1">One of the lists to zip.</param>
        /// <param name="source2">One of the lists to zip.</param>
        /// <param name="source3">One of the lists to zip.</param>
        /// <param name="source4">One of the lists to zip.</param>
        /// <param name="combine">The delegate used to combine the items.</param>
        /// <returns>A new list with the combined items.</returns>
        public static IEnumerable<TResult> Zip<T1, T2, T3, T4, TResult>(this IEnumerable<T1> source1, IEnumerable<T2> source2, IEnumerable<T3> source3, IEnumerable<T4> source4, Func<T1, T2, T3, T4, TResult> combine)
        {
            if (source1 == null)
                throw new ArgumentNullException("source1");
            if (source2 == null)
                throw new ArgumentNullException("source2");
            if (source3 == null)
                throw new ArgumentNullException("source3");
            if (source4 == null)
                throw new ArgumentNullException("source4");
            if (combine == null)
                throw new ArgumentNullException("combine");

            return ZipIterator<T1, T2, T3, T4, TResult>(source1, source2, source3, source4, combine);
        }

        private static IEnumerable<TResult> ZipIterator<T1, T2, T3, T4, TResult>(this IEnumerable<T1> source1, IEnumerable<T2> source2, IEnumerable<T3> source3, IEnumerable<T4> source4, Func<T1, T2, T3, T4, TResult> combine)
        {
            Debug.Assert(source1 != null, "source1 is null.");
            Debug.Assert(source2 != null, "source2 is null.");
            Debug.Assert(source3 != null, "source3 is null.");
            Debug.Assert(source4 != null, "source4 is null.");
            Debug.Assert(combine != null, "combine is null.");

            using (IEnumerator<T1> data1 = source1.GetEnumerator())
            using (IEnumerator<T2> data2 = source2.GetEnumerator())
            using (IEnumerator<T3> data3 = source3.GetEnumerator())
            using (IEnumerator<T4> data4 = source4.GetEnumerator())
                while (data1.MoveNext() && data2.MoveNext() && data3.MoveNext() && data4.MoveNext())
                {
                    yield return combine(data1.Current, data2.Current, data3.Current, data4.Current);
                }
        }

        /// <summary>
        /// Creates a list by applying a function to the last item in the list to generate the next item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The first item in the list.</param>
        /// <param name="count">The number of items to return.</param>
        /// <param name="iterator">The delegate to generate the next item from.</param>
        /// <returns>A list of generated items.</returns>
        public static IEnumerable<T> Iterate<T>(T start, int count, Func<T, T> iterator)
        {
            if (start == null)
                throw new ArgumentNullException("source");
            if (iterator == null)
                throw new ArgumentNullException("iterator");

            return IterateIterator<T>(start, count, iterator);
        }

        private static IEnumerable<T> IterateIterator<T>(T start, int count, Func<T, T> iterator)
        {
            Debug.Assert(start != null, "start is null.");
            Debug.Assert(iterator != null, "iterator is null.");

            int i = 0;
            T result = start;
            while (i < count)
            {
                yield return result;
                result = iterator(result);
                i++;
            }
        }

        /// <summary>
        /// Creates a list by repeating another list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The list to repeat.</param>
        /// <param name="count">The number of items to return.</param>
        /// <returns>A circular list of items.</returns>
        public static IEnumerable<T> Cycle<T>(this IEnumerable<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return CycleIterator<T>(source, count);
        }

        private static IEnumerable<T> CycleIterator<T>(IEnumerable<T> source, int count)
        {
            Debug.Assert(source != null, "source is null.");

            int i = 0;
            while (i < count)
                using (IEnumerator<T> data = source.GetEnumerator())
                {
                    while (i < count && data.MoveNext())
                    {
                        yield return data.Current;
                        i++;
                    }
                }
        }

        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return RepeatIterator<T>(source, count);
        }

        private static IEnumerable<T> RepeatIterator<T>(IEnumerable<T> source, int count)
        {
            Debug.Assert(source != null, "source is null.");

            using (IEnumerator<T> data = source.GetEnumerator())
                while (data.MoveNext())
                {
                    for (int i = 0; i < count; i++)
                    {
                        yield return data.Current;
                    }
                }
        }

        /// <summary>
        /// Determine statistical mode for a sequence of type T.
        /// </summary>
        /// <typeparam name="T">Generic type T.</typeparam>
        /// <param name="source">Sequence of data.</param>
        /// <returns>Returns the mode of incoming sequence.</returns>
        public static T Mode<T>(this IEnumerable<T> source)
        {
            return Mode(source, null);
        }

        /// <summary>
        /// Determine statistical mode for a sequence of type T using supplied comparer.
        /// </summary>
        /// <typeparam name="T">Generic type T.</typeparam>
        /// <param name="source">Sequence of data.</param>
        /// <param name="comparer">Custom comparer.</param>
        /// <returns>Return statistical mode of sequence.</returns>
        public static T Mode<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            return source.ToLookup(t => t, comparer).OrderByDescending(l => l.Count()).First().Key;
        }

        /// <summary>
        /// Does a weighted average over the values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector">The item to average.</param>
        /// <param name="totalSelector">The item to weigh the average over.</param>
        /// <returns></returns>
        public static double WeightedAverage<T>(this IEnumerable<T> source, Func<T, double> selector, Func<T, double> totalSelector)
        {
            var total = source.Sum(totalSelector);
            return source.Sum(t => selector(t) * (totalSelector(t) / total));
        }

        public static TSource OnlyOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            ICollection<TSource> list = source as ICollection<TSource>;
            TSource[] array = source as TSource[];

            if (list != null)
            {
                switch (list.Count)
                {
                    case 0:
                        return default(TSource);

                    case 1:
                        return list.First();
                }
            }
            else if (array != null)
            {
                switch (array.Length)
                {
                    case 0:
                        return default(TSource);

                    case 1:
                        return array[0];
                }
            }
            else
            {
                using (IEnumerator<TSource> enumerator = source.GetEnumerator())
                {
                    if (!enumerator.MoveNext())
                    {
                        return default(TSource);
                    }
                    TSource current = enumerator.Current;
                    if (!enumerator.MoveNext())
                    {
                        return current;
                    }
                }
            }
            return default(TSource);
        }

        public static TSource OnlyOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            TSource local = default(TSource);
            long num = 0L;
            foreach (TSource local2 in source)
            {
                if (predicate(local2))
                {
                    local = local2;
                    num += 1L;
                }
            }
            long num2 = num;
            if ((num2 <= 1L) && (num2 >= 0L))
            {
                switch (((int)num2))
                {
                    case 0:
                        return default(TSource);

                    case 1:
                        return local;
                }
            }
            return default(TSource);
        }

        public static IEnumerable<TSource> SkipLast<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException("source", "source is null.");

            ICollection<TSource> list = source as ICollection<TSource>;
            TSource[] array = source as TSource[];

            if (list != null)
                return source.Take(list.Count - 1);
            else if (array != null)
                return source.Take(array.Length - 1);

            return SkipLastIterator<TSource>(source);
        }

        private static IEnumerable<TSource> SkipLastIterator<TSource>(IEnumerable<TSource> source)
        {
            Debug.Assert(source != null, "source is null.");

            using (var data = source.GetEnumerator())
            {
                bool hasItems = false;
                TSource last = default(TSource);
                while (data.MoveNext())
                {
                    if (hasItems)
                        yield return last;

                    hasItems = true;
                    last = data.Current;
                }
            }
        }

        public static bool EnumerableEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null)
                throw new ArgumentNullException("first", "first is null.");
            if (second == null)
                throw new ArgumentNullException("second", "second is null.");

            var firstStorage = new List<TSource>();
            var secondStorage = new List<TSource>();

            bool firstRunning = true;
            bool secondRunning = true;

            using (var firstEnum = first.GetEnumerator())
            using (var secondEnum = second.GetEnumerator())
            {
                while (firstRunning && secondRunning)
                {
                    firstRunning = firstRunning && firstEnum.MoveNext();

                    if (firstRunning && !secondStorage.Remove(firstEnum.Current))
                        firstStorage.Add(firstEnum.Current);

                    secondRunning = secondRunning && secondEnum.MoveNext();

                    if (secondRunning && !firstStorage.Remove(secondEnum.Current))
                        secondStorage.Add(secondEnum.Current);
                }
            }

            return !firstRunning && !secondRunning && firstStorage.Count == 0 && secondStorage.Count == 0;
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }

        public static TSource MaxValue<TSource, TComparer>(this IEnumerable<TSource> source, Func<TSource, TComparer> compareItem)
        {
            if (source == null)
                throw new ArgumentNullException("source", "source is null.");
            if (compareItem == null)
                throw new ArgumentNullException("compareItem", "compareItem is null.");

            Comparer<TComparer> comparer = Comparer<TComparer>.Default;

            return source.Aggregate((l, r) => comparer.Compare(compareItem(l), compareItem(r)) > 0 ? l : r);
        }

        public static TSource MinValue<TSource, TComparer>(this IEnumerable<TSource> source, Func<TSource, TComparer> compareItem)
        {
            if (source == null)
                throw new ArgumentNullException("source", "source is null.");
            if (compareItem == null)
                throw new ArgumentNullException("compareItem", "compareItem is null.");

            Comparer<TComparer> comparer = Comparer<TComparer>.Default;

            return source.Aggregate((l, r) => comparer.Compare(compareItem(l), compareItem(r)) < 0 ? l : r);
        }
    }
}
