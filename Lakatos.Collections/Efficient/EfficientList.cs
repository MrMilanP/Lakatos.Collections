using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lakatos.Collections.Efficient
{
    /// <summary>
    /// Array-backed list with explicit sorting and binary-search support.
    /// </summary>
    /// <typeparam name="T">Element type. It must be comparable for sorting and binary search.</typeparam>
    public class EfficientList<T> where T : IComparable<T>
    {
        private const int DefaultCapacity = 16;

        private T[] _items;
        private int _size;
        private bool _isSorted;

        /// <summary>
        /// Creates a new list with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">Initial internal capacity. Must be greater than zero.</param>
        public EfficientList(int capacity = DefaultCapacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");
            }

            _items = new T[capacity];
            _size = 0;
            _isSorted = true;
        }

        /// <summary>
        /// Number of elements currently stored in the list.
        /// </summary>
        public int Count => _size;

        /// <summary>
        /// Current internal array capacity.
        /// </summary>
        public int Capacity => _items.Length;

        /// <summary>
        /// Indicates whether the current content is known to be sorted.
        /// </summary>
        public bool IsSorted => _isSorted;

        /// <summary>
        /// Gets or replaces an element at the specified index.
        /// Replacing an element marks the list as unsorted.
        /// </summary>
        public T this[int index]
        {
            get => Get(index);
            set
            {
                ValidateIndex(index);
                _items[index] = value;
                _isSorted = false;
            }
        }

        /// <summary>
        /// Adds an element to the end of the list.
        /// </summary>
        public void Add(T item)
        {
            EnsureCapacity(_size + 1);
            _items[_size] = item;
            _size++;

            if (_size > 1 && _items[_size - 2].CompareTo(item) > 0)
            {
                _isSorted = false;
            }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        public T Get(int index)
        {
            ValidateIndex(index);
            return _items[index];
        }

        /// <summary>
        /// Removes all elements while keeping the allocated internal capacity.
        /// </summary>
        public void Clear()
        {
            Array.Clear(_items, 0, _size);
            _size = 0;
            _isSorted = true;
        }

        /// <summary>
        /// Inserts an element at the specified position.
        /// </summary>
        public void InsertAt(int index, T item)
        {
            if (index < 0 || index > _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            EnsureCapacity(_size + 1);

            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }

            _items[index] = item;
            _size++;
            _isSorted = false;
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        public void RemoveAt(int index)
        {
            ValidateIndex(index);

            int moveCount = _size - index - 1;
            if (moveCount > 0)
            {
                Array.Copy(_items, index + 1, _items, index, moveCount);
            }

            _size--;
            _items[_size] = default!;
        }

        /// <summary>
        /// Returns true if the item exists in the list. Uses binary search when the list is sorted.
        /// </summary>
        public bool Contains(T item)
        {
            return _isSorted ? BinarySearch(item) >= 0 : Find(item) is not null;
        }

        /// <summary>
        /// Finds an item using linear search.
        /// </summary>
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
        /// Sorts the list in-place using the default comparer.
        /// </summary>
        public void Sort()
        {
            Array.Sort(_items, 0, _size, Comparer<T>.Default);
            _isSorted = true;
        }

        /// <summary>
        /// Sorts the list in-place. Kept for API compatibility with earlier versions.
        /// </summary>
        public void ParallelSort()
        {
            Sort();
        }

        /// <summary>
        /// Sorts the list in-place using quicksort.
        /// </summary>
        public void QuickSort()
        {
            if (_size <= 1)
            {
                _isSorted = true;
                return;
            }

            QuickSort(0, _size - 1);
            _isSorted = true;
        }

        /// <summary>
        /// Sorts the list in-place using merge sort.
        /// </summary>
        public void MergeSort()
        {
            if (_size <= 1)
            {
                _isSorted = true;
                return;
            }

            T[] buffer = new T[_size];
            MergeSort(0, _size - 1, buffer);
            _isSorted = true;
        }

        /// <summary>
        /// Searches for an element using binary search. The list must be sorted first.
        /// </summary>
        public int BinarySearch(T item)
        {
            EnsureSortedForBinarySearch();

            int left = 0;
            int right = _size - 1;

            while (left <= right)
            {
                int mid = left + ((right - left) >> 1);
                int comparison = _items[mid].CompareTo(item);

                if (comparison == 0)
                {
                    return mid;
                }

                if (comparison < 0)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return -1;
        }

        /// <summary>
        /// Searches for an element by splitting a sorted list into partitions.
        /// Useful only for large sorted lists.
        /// </summary>
        public int ParallelBinarySearch(T item)
        {
            EnsureSortedForBinarySearch();

            if (_size == 0)
            {
                return -1;
            }

            int result = -1;
            int partitionCount = Math.Min(Environment.ProcessorCount, _size);

            Parallel.For(0, partitionCount, (partitionIndex, state) =>
            {
                if (Volatile.Read(ref result) != -1)
                {
                    state.Stop();
                    return;
                }

                int left = partitionIndex * _size / partitionCount;
                int right = ((partitionIndex + 1) * _size / partitionCount) - 1;

                int localResult = BinarySearchSegment(item, left, right);
                if (localResult != -1)
                {
                    Interlocked.CompareExchange(ref result, localResult, -1);
                    state.Stop();
                }
            });

            return result;
        }

        /// <summary>
        /// Returns a compact array containing only the used elements.
        /// </summary>
        public T[] ToArray()
        {
            T[] result = new T[_size];
            Array.Copy(_items, result, _size);
            return result;
        }

        /// <summary>
        /// Ensures that the list can hold at least the specified number of elements.
        /// </summary>
        public void EnsureCapacity(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            if (capacity <= _items.Length)
            {
                return;
            }

            int newCapacity = _items.Length;
            while (newCapacity < capacity)
            {
                newCapacity *= 2;
            }

            Array.Resize(ref _items, newCapacity);
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        private void EnsureSortedForBinarySearch()
        {
            if (!_isSorted)
            {
                throw new InvalidOperationException("Binary search requires the list to be sorted first.");
            }
        }

        private int BinarySearchSegment(T item, int left, int right)
        {
            while (left <= right)
            {
                int mid = left + ((right - left) >> 1);
                int comparison = _items[mid].CompareTo(item);

                if (comparison == 0)
                {
                    return mid;
                }

                if (comparison < 0)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return -1;
        }

        private void QuickSort(int low, int high)
        {
            while (low < high)
            {
                int pivotIndex = Partition(low, high);

                if (pivotIndex - low < high - pivotIndex)
                {
                    QuickSort(low, pivotIndex - 1);
                    low = pivotIndex + 1;
                }
                else
                {
                    QuickSort(pivotIndex + 1, high);
                    high = pivotIndex - 1;
                }
            }
        }

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

        private void Swap(int index1, int index2)
        {
            if (index1 == index2)
            {
                return;
            }

            T temp = _items[index1];
            _items[index1] = _items[index2];
            _items[index2] = temp;
        }

        private void MergeSort(int low, int high, T[] buffer)
        {
            if (low >= high)
            {
                return;
            }

            int mid = low + ((high - low) >> 1);
            MergeSort(low, mid, buffer);
            MergeSort(mid + 1, high, buffer);
            Merge(low, mid, high, buffer);
        }

        private void Merge(int low, int mid, int high, T[] buffer)
        {
            int left = low;
            int right = mid + 1;
            int bufferIndex = low;

            while (left <= mid && right <= high)
            {
                if (_items[left].CompareTo(_items[right]) <= 0)
                {
                    buffer[bufferIndex++] = _items[left++];
                }
                else
                {
                    buffer[bufferIndex++] = _items[right++];
                }
            }

            while (left <= mid)
            {
                buffer[bufferIndex++] = _items[left++];
            }

            while (right <= high)
            {
                buffer[bufferIndex++] = _items[right++];
            }

            for (int i = low; i <= high; i++)
            {
                _items[i] = buffer[i];
            }
        }
    }
}
