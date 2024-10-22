using System;
using System.Collections.Generic;

namespace Lakatos.Collections.Persistent
{
    /// <summary>
    /// Represents a persistent (immutable) list implementation.
    /// Each modification creates a new instance, preserving the original.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class PersistentList<T>
    {
        private readonly Node? head;

        /// <summary>
        /// Internal class representing a node in the list.
        /// </summary>
        private class Node
        {
            public T Value { get; }
            public Node? Next { get; }

            public Node(T value, Node? next)
            {
                Value = value;
                Next = next;
            }
        }

        private PersistentList(Node? head)
        {
            this.head = head;
        }



        /// <summary>
        /// Static instance representing an empty list.
        /// </summary>
        public static readonly PersistentList<T> Empty = new PersistentList<T>(null);

        /// <summary>
        /// Adds an element to the beginning of the list and returns a new instance.
        /// </summary>
        /// <param name="item">The element to add to the list.</param>
        /// <returns>A new list instance with the element added at the beginning.</returns>
        public PersistentList<T> Add(T item)
        {
            return new PersistentList<T>(new Node(item, head));
        }

        /// <summary>
        /// Removes the head element of the list and returns the remaining elements (tail).
        /// </summary>
        /// <returns>The tail of the list, excluding the head.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to remove an element from an empty list.</exception>
        public PersistentList<T> Remove()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Cannot remove an element from an empty list.");
            }
            return new PersistentList<T>(head!.Next);
        }

        /// <summary>
        /// Checks if the list is empty.
        /// </summary>
        public bool IsEmpty => head == null;

        /// <summary>
        /// Gets the head of the list (the first element). Throws an exception if the list is empty.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the list is empty.</exception>
        public T Head
        {
            get
            {
                if (IsEmpty) throw new InvalidOperationException("The list is empty.");
                return head!.Value;
            }
        }

        /// <summary>
        /// Gets the tail of the list (the remaining elements). Throws an exception if the list is empty.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the list is empty.</exception>
        public PersistentList<T> Tail
        {
            get
            {
                if (IsEmpty) throw new InvalidOperationException("The list is empty.");
                return new PersistentList<T>(head!.Next);
            }
        }

        /// <summary>
        /// Creates a new persistent list from an existing collection of items.
        /// </summary>
        /// <param name="items">The collection of items to create the persistent list from.</param>
        /// <returns>A new instance of <see cref="PersistentList{T}"/> containing all the elements from the provided collection, in the original order.</returns>
        public static PersistentList<T> From(IEnumerable<T> items)
        {
            var result = Empty;

            // Iteriramo kroz originalnu kolekciju, dodajući elemente od pozadi na pravi način
            foreach (var item in items)
            {
                result = result.Add(item);
            }

            return result;
        }


        /// <summary>
        /// Retrieves the element at the specified index in the list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the specified index is out of the bounds of the list.
        /// </exception>
        public T Get(int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            var current = head;
            int count = 0;

            while (current != null)
            {
                if (count == index)
                {
                    return current.Value;
                }
                current = current.Next;
                count++;
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }


        /// <summary>
        /// Concatenates the current list with another persistent list, maintaining the order of both lists.
        /// </summary>
        /// <param name="other">The other list to concatenate with the current list.</param>
        /// <returns>A new list containing elements from both lists in their original order.</returns>
        public PersistentList<T> Concat(PersistentList<T> other)
        {
            var current = this;
            var result = other;

            Stack<T> tempStack = new Stack<T>();

            while (!current.IsEmpty)
            {
                tempStack.Push(current.Head!);
                current = current.Tail!;
            }

            while (tempStack.Count > 0)
            {
                result = result.Add(tempStack.Pop());
            }

            return result;
        }


        /// <summary>
        /// Finds and returns the first element that matches the given predicate.
        /// </summary>
        /// <param name="predicate">The function to test each element for a condition.</param>
        /// <returns>The first element that matches the predicate, or default if none is found.</returns>
        public T? Find(Func<T, bool> predicate)
        {
            var current = head;

            while (current != null)
            {
                if (predicate(current.Value))
                {
                    return current.Value;
                }
                current = current.Next;
            }

            return default;
        }

        /// <summary>
        /// Attempts to find the first element that matches the given predicate.
        /// </summary>
        /// <param name="predicate">The function to test each element for a condition.</param>
        /// <param name="value">The first element that matches the predicate if found; otherwise, the default value of type T.</param>
        /// <returns>
        /// Returns <c>true</c> if an element that matches the predicate is found; otherwise, <c>false</c>.
        /// </returns>
        public bool TryFind(Func<T, bool> predicate, out T? value)
        {
            var current = head;

            while (current != null)
            {
                if (predicate(current.Value))
                {
                    value = current.Value;
                    return true;
                }
                current = current.Next;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Filters the list based on the provided predicate function and returns a new list containing only the elements that satisfy the condition.
        /// </summary>
        /// <param name="predicate">A function that determines whether an element should be included in the new list.</param>
        /// <returns>A new list containing elements that match the predicate, maintaining the order of the original list.</returns>
        public PersistentList<T> Filter(Func<T, bool> predicate)
        {
            var current = this;
            PersistentList<T> result = Empty;

            // Prvo prolazimo kroz listu i dodajemo na početak, da bi sačuvali redosled.
            Stack<T> tempStack = new Stack<T>();

            while (!current.IsEmpty)
            {
                if (predicate(current.Head!))
                {
                    tempStack.Push(current.Head!);
                }
                current = current.Tail!;
            }

            while (tempStack.Count > 0)
            {
                result = result.Add(tempStack.Pop());
            }

            return result;
        }


        /// <summary>
        /// Applies the provided transformation function to each element in the list and returns a new list with the transformed elements in the original order.
        /// </summary>
        /// <typeparam name="TOut">The type of the elements in the transformed list.</typeparam>
        /// <param name="transform">A function that transforms each element.</param>
        /// <returns>A new list containing the transformed elements, in the same order as the original list.</returns>
        public PersistentList<TOut> Map<TOut>(Func<T, TOut> transform)
        {
            var current = this;
            var result = PersistentList<TOut>.Empty;

            Stack<TOut> tempStack = new Stack<TOut>();

            while (!current.IsEmpty)
            {
                tempStack.Push(transform(current.Head!));
                current = current.Tail!;
            }

            while (tempStack.Count > 0)
            {
                result = result.Add(tempStack.Pop());
            }

            return result;
        }


        /// <summary>
        /// Trims the list to the specified maximum number of elements.
        /// </summary>
        /// <param name="maxLength">The maximum number of elements to retain in the list.</param>
        /// <returns>A new list containing up to <paramref name="maxLength"/> elements from the start of the original list.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxLength"/> is negative.</exception>
        public PersistentList<T> Trim(int maxLength)
        {
            if (maxLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength), "Length cannot be negative.");
            }

            var current = this;
            var result = Empty;
            int count = 0;

            Stack<T> tempStack = new Stack<T>();

            while (!current.IsEmpty && count < maxLength)
            {
                tempStack.Push(current.Head!);
                current = current.Tail!;
                count++;
            }

            while (tempStack.Count > 0)
            {
                result = result.Add(tempStack.Pop());
            }

            return result;
        }


        /// <summary>
        /// Converts the persistent list to a standard <see cref="List{T}"/>.
        /// </summary>
        /// <param name="preserveOriginalOrder">If true, maintains the original order of items. 
        /// Otherwise, returns the list as stored internally.</param>
        /// <returns>A new instance of <see cref="List{T}"/> containing all elements from the persistent list.</returns>
        public List<T> ToList(bool preserveOriginalOrder = true)
        {
            var result = new List<T>();
            var current = head;

            while (current != null)
            {
                result.Add(current.Value);
                current = current.Next;
            }

            if (preserveOriginalOrder)
            {
                result.Reverse(); // Maintain original order if requested
            }

            return result;
        }


        /// <summary>
        /// Sorts the list based on the provided comparison function.
        /// </summary>
        /// <param name="comparer">The comparison function to determine the order of elements.</param>
        /// <returns>A new sorted list.</returns>
        public PersistentList<T> Sort(Func<T, T, int> comparer)
        {
            var current = this;
            var sortedList = Empty;

            while (!current.IsEmpty)
            {
                sortedList = InsertSorted(sortedList, current.Head!, comparer);
                current = current.Tail!;
            }

            // Log za praćenje sortirane liste
            Console.WriteLine("Sorted List After Sort Call: " + string.Join(" -> ", sortedList.ToList()));

            return sortedList;
        }

        /// <summary>
        /// Inserts an element into the list at the correct position according to the comparison function.
        /// </summary>
        /// <param name="list">The list to insert the item into.</param>
        /// <param name="item">The item to insert.</param>
        /// <param name="comparer">The comparison function for ordering.</param>
        /// <returns>A new list with the item inserted in the correct position.</returns>
        private PersistentList<T> InsertSorted(PersistentList<T> list, T item, Func<T, T, int> comparer)
        {
            if (list.IsEmpty || comparer(item, list.Head) <= 0)
            {
                // Insert the item at the beginning if the list is empty or if the item is smaller or equal to the head
                return list.Add(item);
            }

            // Recursively insert into the tail and create a new node with the same head but the sorted tail
            var newNode = new Node(list.Head, InsertSorted(list.Tail, item, comparer).head);
            return new PersistentList<T>(newNode);
        }

    }
}
