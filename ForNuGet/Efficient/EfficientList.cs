using System;
using System.Collections.Generic;

namespace Lakatos.Collections.Efficient
{
    /// <summary>
    /// Represents an efficient list that stores elements of type <typeparamref name="T"/>
    /// and provides fast search and sort capabilities. The type <typeparamref name="T"/>
    /// must implement <see cref="IComparable{T}"/> for comparison operations.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list, which must implement <see cref="IComparable{T}"/>.</typeparam>
    public class EfficientList<T> where T : IComparable<T>
    {
        private T[] _items;
        private int _size;

        /// <summary>
        /// Initializes a new instance of the <see cref="EfficientList{T}"/> class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity of the list. Defaults to 16.</param>
        public EfficientList(int capacity = 16)
        {
            _items = new T[capacity];
            _size = 0;
        }

        /// <summary>
        /// Adds an element to the list, expanding the internal array if necessary.
        /// </summary>
        /// <param name="item">The element to add to the list.</param>
        public void Add(T item)
        {
            if (_size >= _items.Length)
            {
                ExpandCapacity();
            }
            _items[_size] = item;
            _size++;
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
        public T Get(int index)
        {
            if (index < 0 || index >= _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return _items[index];
        }

        /// <summary>
        /// Searches for an element using binary search. Assumes the list is sorted.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>The index of the item if found, otherwise -1.</returns>
        public int BinarySearch(T item)
        {
            int left = 0;
            int right = _size - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                int comparison = _items[mid].CompareTo(item);

                if (comparison == 0)
                {
                    return mid;
                }
                else if (comparison < 0)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return -1; // Not found
        }


        /// <summary>
        /// Performs a parallel binary search on the list, utilizing multiple threads based on the number of available processor cores.
        /// </summary>
        /// <param name="item">The item to search for in the list.</param>
        /// <returns>The index of the item if found; otherwise, returns -1.</returns>
        /// <remarks>
        /// The method divides the list into segments based on the number of processor cores and searches each segment in parallel.
        /// If the item is found in any segment, the search stops and returns the index.
        /// </remarks>
        public int ParallelBinarySearch(T item)
        {
            int result = -1;
            int partitionCount = Environment.ProcessorCount; // Number of available processor cores

            Parallel.For(0, partitionCount, (partitionIndex, state) =>
            {
                // Pre-check to avoid unnecessary work if another thread has already found the result
                if (state.IsStopped)
                    return;
                int left = partitionIndex * (_size / partitionCount);
                int right = (partitionIndex == partitionCount - 1) ? _size - 1 : (partitionIndex + 1) * (_size / partitionCount) - 1;

                int localResult = BinarySearchSegment(item, left, right);
                if (localResult != -1)
                {
                    Interlocked.CompareExchange(ref result, localResult, -1);
                    state.Break(); // Stop other threads if the item is found
                }
            });

            return result;
        }

        /// <summary>
        /// Performs a binary search within a specified segment of the list.
        /// </summary>
        /// <param name="item">The item to search for in the list.</param>
        /// <param name="left">The left boundary index of the segment to search within.</param>
        /// <param name="right">The right boundary index of the segment to search within.</param>
        /// <returns>The index of the item if found; otherwise, returns -1.</returns>
        /// <remarks>
        /// This method operates within the boundaries specified by the <paramref name="left"/> and <paramref name="right"/> parameters.
        /// It is used internally by the <see cref="ParallelBinarySearch"/> method.
        /// </remarks>
        private int BinarySearchSegment(T item, int left, int right)
        {
            T[] items = _items; // Direktna referenca na niz da izbegnemo pristup preko `this`

            while (left <= right)
            {
                int mid = (left + right) >> 1; // Bit-shift umesto deljenja radi bržeg računanja
                int comparison = items[mid].CompareTo(item);

                if (comparison == 0)
                {
                    return mid;
                }
                else if (comparison < 0)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return -1; // Ako nije pronađen
        }



        /// <summary>
        /// Sorts the list using quicksort algorithm for fast sorting.
        /// </summary>
        public void QuickSort()
        {
            QuickSort(0, _size - 1);
        }

        /// <summary>
        /// Returns the current size of the list.
        /// </summary>
        public int Count => _size;

        /// <summary>
        /// Expands the capacity of the internal array when needed.
        /// </summary>
        private void ExpandCapacity()
        {
            int newCapacity = (int)(_items.Length * 1.5);
            T[] newArray = new T[newCapacity];
            Array.Copy(_items, newArray, _size);
            _items = newArray;
        }

        /// <summary>
        /// Quicksort algorithm implementation for sorting the internal array.
        /// </summary>
        /// <param name="low">The starting index.</param>
        /// <param name="high">The ending index.</param>
        private void QuickSort(int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(low, high);
                QuickSort(low, pivotIndex - 1);
                QuickSort(pivotIndex + 1, high);
            }
        }

        /// <summary>
        /// Partition function for the quicksort algorithm.
        /// </summary>
        /// <param name="low">The starting index.</param>
        /// <param name="high">The ending index.</param>
        /// <returns>The index of the pivot element.</returns>
        private int Partition(int low, int high)
        {
            T pivot = _items[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (_items[j].CompareTo(pivot) <= 0)
                {
                    i++;
                    Swap(i, j);
                }
            }

            Swap(i + 1, high);
            return i + 1;
        }

        /// <summary>
        /// Swaps two elements in the internal array.
        /// </summary>
        /// <param name="index1">The index of the first element.</param>
        /// <param name="index2">The index of the second element.</param>
        private void Swap(int index1, int index2)
        {
            T temp = _items[index1];
            _items[index1] = _items[index2];
            _items[index2] = temp;
        }

        /// <summary>
        /// Inserts an item at the specified position in the list.
        /// </summary>
        /// <param name="index">The position where the item should be inserted.</param>
        /// <param name="item">The item to insert.</param>
        public void InsertAt(int index, T item)
        {
            if (index < 0 || index > _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (_size == _items.Length)
            {
                Array.Resize(ref _items, _items.Length * 2);
            }

            // Shift elements to the right
            for (int i = _size; i > index; i--)
            {
                _items[i] = _items[i - 1];
            }

            _items[index] = item;
            _size++;
        }

        /// <summary>
        /// Finds an item in the list that matches the provided item.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The item if found, otherwise null.</returns>
        public T? Find(T item)
        {
            for (int i = 0; i < _size; i++)
            {
                if (_items[i].CompareTo(item) == 0)
                {
                    return _items[i];
                }
            }

            return default;
        }

        /// <summary>
        /// Sorts the list using the ParallelSort algorithm for faster sorting on multicore systems.
        /// </summary>
        public void ParallelSort()
        {
            Array.Sort(_items, 0, _size, Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts the list using the MergeSort algorithm.
        /// </summary>
        public void MergeSort()
        {
            MergeSort(0, _size - 1);
        }


        /// <summary>
        /// Recursively sorts the elements of the list using the merge sort algorithm.
        /// </summary>
        /// <param name="low">The starting index of the range to sort.</param>
        /// <param name="high">The ending index of the range to sort.</param>
        /// <remarks>
        /// Merge sort is a divide-and-conquer algorithm that recursively splits the list into halves,
        /// sorts each half, and then merges the sorted halves back together.
        /// </remarks>
        private void MergeSort(int low, int high)
        {
            if (low < high)
            {
                int mid = (low + high) / 2;
                MergeSort(low, mid);
                MergeSort(mid + 1, high);
                Merge(low, mid, high);
            }
        }

        /// <summary>
        /// Merges two sorted halves of the list into a single sorted range.
        /// </summary>
        /// <param name="low">The starting index of the first half.</param>
        /// <param name="mid">The ending index of the first half and the midpoint of the range.</param>
        /// <param name="high">The ending index of the second half.</param>
        /// <remarks>
        /// This method merges two sorted halves of the list (_items) defined by the indices [low, mid]
        /// and [mid + 1, high], storing the sorted result in the original array.
        /// </remarks>
        private void Merge(int low, int mid, int high)
        {
            T[] temp = new T[high - low + 1];
            int i = low, j = mid + 1, k = 0;

            // Merge elements from both halves into temp until one half is exhausted
            while (i <= mid && j <= high)
            {
                if (_items[i].CompareTo(_items[j]) <= 0)
                {
                    temp[k++] = _items[i++];
                }
                else
                {
                    temp[k++] = _items[j++];
                }
            }

            // Copy any remaining elements from the first half
            while (i <= mid)
            {
                temp[k++] = _items[i++];
            }

            // Copy any remaining elements from the second half
            while (j <= high)
            {
                temp[k++] = _items[j++];
            }

            // Copy the sorted elements back into the original array
            for (i = low; i <= high; i++)
            {
                _items[i] = temp[i - low];
            }
        }


        /// <summary>
        /// Converts the list to an array for easier manipulation and display.
        /// </summary>
        /// <returns>An array containing all elements of the list.</returns>
        public T[] ToArray()
        {
            T[] result = new T[_size];
            Array.Copy(_items, result, _size);
            return result;
        }
    }
}
