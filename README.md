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
- **High Performance (EfficientList)**: Optimized for fast operations with support for parallel execution.
- **Scalability (EfficientList)**: Handles millions of elements efficiently, suitable for large-scale and real-time data processing.


### Specific to EfficientList:
- **Fast Operations**: Sorting and searching optimized for performance with support for parallel execution.
- **Scalability**: Handles millions of elements efficiently, suitable for large-scale and real-time data processing.

### Practical Use-Cases
- **PersistentList**:
  - Undo/Redo Operations: Ideal for applications that need to revert actions.
  - Time-Travel Debugging: Allows inspection of historical states.
  - Multithreaded Applications: Ensures thread-safety without synchronization issues.
- **EfficientList**:
  - DNS Filtering: Efficient for checking and responding to queries in real-time.
  - Data Analysis: Optimized for sorting and searching over large datasets.
  - Real-Time Systems: Suitable for scalable and high-performance environments.


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

| Collection      | Average Insertion Time | Historical Tracking | Complexity                |
|-----------------|------------------------|---------------------|---------------------------|
| PersistentList  | ~7 µs                  | Yes                 | Logarithmic               |
| EfficientList   | ~3 µs (insertion)      | No                  | Linear (for searching)    |
| FSharpList      | ~3 µs                  | No                  | Linear                    |
| ImmutableList   | ~127 µs                | No                  | Linear                    |

## Pros & Cons

### Pros
- **Immutability and Safety**: No accidental modifications of data, making the code easier to debug and maintain.
- **Persistence (PersistentList)**: Track the evolution of data over time, enabling undo/redo functionality and version control-like behavior.
- **High Performance (EfficientList)**: Optimized for fast operations, suitable for real-time and large-scale systems.

### Cons
- **Memory Usage**: Persistent collections can consume more memory as each change creates a new version of the data.
- **Speed**: Persistent collections may be slower than non-persistent collections due to version handling, while `EfficientList` provides better performance for non-persistent operations.

## Advanced Features

### Time Travel Debugging (PersistentList)
Provides the capability to step back in time and see previous states of the collection, useful for debugging complex application states.

### Concurrent Operations
Due to immutability (PersistentList) and efficient parallel operations (EfficientList), these collections are inherently thread-safe, making them useful for concurrent applications without synchronization issues.


## License
This project is licensed under the MIT License. You are free to use, modify, and distribute the code with proper attribution. See the LICENSE file for more details.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change. Any contributions are highly appreciated!

## Contact
Author: Milan Lakatos Petrovic

