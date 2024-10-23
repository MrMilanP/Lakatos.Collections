# Lakatos.Collections

### Overview
Lakatos.Collections is a C# library designed for developers who need advanced data structures focusing on immutability, persistence, and efficiency. It offers collections like PersistentList and EfficientList, providing historical data consistency and high-performance operations for scenarios where data integrity and performance are crucial.


## Collections Available
### 1. PersistentList
A linked list where each modification (like adding a new element) results in a new version of the list. All previous versions remain accessible, ensuring historical consistency without data mutation.

### 2. EfficientList
An optimized list designed for high-performance operations, such as fast sorting and searching. Suitable for scenarios where efficiency and speed are critical, even with large datasets. 

### Features
- **Immutability**: Ensures no in-place modifications, avoiding side effects.
- **Version History (PersistentList)**: Track changes and maintain historical versions, useful for undo features and debugging.
- **Efficient Snapshots (PersistentList)**: Stores only differences, optimizing memory usage.
- **High Performance (EfficientList)**: Provides fast sorting algorithms like `QuickSort`, `MergeSort`, and `ParallelSort` with support for parallel execution.
- **Scalability (EfficientList)**: Efficiently handles millions of elements, suitable for large-scale and real-time data processing.


### Practical Use-Cases
- **PersistentList**:
  - Undo/Redo Operations, time-travel debugging, and multithreaded applications.
- **EfficientList**:
  - DNS filtering, data analysis, and real-time systems.



## Getting Started
### Installation
Install via NuGet Package Manager:

```bash
Install-Package Lakatos.Collections
```
### Example Usage

### PersistentList
```csharp
using Lakatos.Collections.Persistent;

var persistentList = PersistentList<int>.Empty;
persistentList = persistentList.Add(1);
persistentList = persistentList.Add(2);

// Accessing previous versions
var previousList = persistentList.Previous; // Will be [1]
```

### EfficientList

#### Using `string`

```csharp
using Lakatos.Collections.Efficient;

var efficientList = new EfficientList<string>();
efficientList.Add("192.168.0.1");
efficientList.Add("10.0.0.1");
efficientList.Add("172.16.0.1");

// Sort the list using QuickSort
efficientList.QuickSort();

// Search for an element
int index = efficientList.BinarySearch("10.0.0.1");
Console.WriteLine($"Element found at index: {index}");
```

#### Using `CharArrayWrapper`

```csharp
using Lakatos.Collections.Efficient;

var efficientList = new EfficientList<CharArrayWrapper>();

efficientList.Add(new CharArrayWrapper(new char[] { '1', '9', '2', '.', '1', '6', '8', '.', '0', '.', '1' }));
efficientList.Add(new CharArrayWrapper(new char[] { '1', '0', '.', '0', '.', '0', '.', '1' }));
efficientList.Add(new CharArrayWrapper(new char[] { '1', '7', '2', '.', '1', '6', '.', '0', '.', '1' }));

// Sort the list using QuickSort
efficientList.QuickSort();

// Search for an element
var searchElement = new CharArrayWrapper(new char[] { '1', '0', '.', '0', '.', '0', '.', '1' });
int index = efficientList.BinarySearch(searchElement);
Console.WriteLine($"Element found at index: {index}");
```

## Comparison to Other Collections

| Collection     | Average Insertion Time | Historical Tracking | Complexity for Search        | Immutable | Avg Search Time for 100 Parallel Searches |
|----------------|------------------------|---------------------|------------------------------|-----------|------------------------------------------|
| PersistentList | ~7 µs                  | Yes                 | Logarithmic                  | Yes       | N/A                                      |
| EfficientList  | ~0.78 µs (insertion)   | No                  | Logarithmic (sorted),        | No        | 5.267 ms                                 |
|                |                        |                     | Linear (unsorted)            |           |                                          |
| FSharpList     | ~3 µs                  | No                  | Linear                       | Yes       | N/A                                      |
| ImmutableList  | ~0.9 µs                | No                  | Logarithmic                  | Yes       | 18.379 ms                                |


**Note**: `EfficientList` and `ImmutableList` were tested with **10 million insertions**, sorting operations, and **100 parallel searches**, confirming their efficiency and speed when handling large datasets.


## Pros & Cons

### Pros
- **Immutability and Safety**: Prevents accidental modifications, simplifying debugging and maintenance.
- **Persistence (PersistentList)**: Enables undo/redo functionality and version control-like behavior.
- **High Performance (EfficientList)**: Optimized for fast operations, suitable for real-time and large-scale systems.
- **Concurrent Operations**: Thread-safe design due to immutability and efficient parallel processing.

### Cons
- **Memory Usage**: Persistent collections consume more memory as each change creates a new version.
- **Speed**: May be slower than non-persistent collections due to version handling, but `EfficientList` improves performance for non-persistent operations.



## License
This project is licensed under the MIT License. You are free to use, modify, and distribute the code with proper attribution. See the LICENSE file for more details.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change. Any contributions are highly appreciated!

## Contact
Author: Milan Lakatos Petrovic

