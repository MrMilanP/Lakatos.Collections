# Lakatos.Collections

## Overview
**Lakatos.Collections** is a C# library designed for developers who need advanced data structures focusing on immutability, persistence, and efficiency. This library simplifies working with immutable collections while offering version history, safe, consistent operations, and high-performance data structures.

## Why Lakatos.Collections?
Unlike the standard collections in .NET like `ImmutableList`, **Lakatos.Collections** provides advanced persistence through collections like `PersistentList`, and high-efficiency data management through `EfficientList`. These collections are highly beneficial for scenarios where data integrity, consistency, performance, and history tracking are crucial.

## Collections Available
### 1. PersistentList
A linked list where each modification (like adding a new element) results in a new version of the list. All previous versions remain accessible, ensuring historical consistency without data mutation.

### 2. EfficientList
An optimized list designed for high-performance operations, such as fast sorting and searching. Suitable for scenarios where efficiency and speed are critical, even with large datasets. 

#### Features:
- **High Performance**: Provides fast sorting algorithms like `QuickSort`, `MergeSort`, and `ParallelSort`.
- **Parallel Operations**: Supports parallel search (`ParallelBinarySearch`) for improved speed on multi-core systems.
- **Dynamic Growth**: Efficiently manages memory and grows dynamically without significant overhead.

## Features
### Common Features:
- **Immutability**: Collections from Lakatos.Collections do not allow in-place modifications, helping avoid side effects in your applications.
- **Version History** (for `PersistentList`): Track changes and maintain historical versions of your data. Useful for undo features, historical data viewing, and concurrent system safety.
- **Efficient Snapshots** (for `PersistentList`): Only the difference is stored, allowing multiple versions without memory blow-up.

### Specific to EfficientList:
- **Fast Operations**: Sorting and searching optimized for performance with support for parallel execution.
- **Scalability**: Handles millions of elements efficiently, suitable for large-scale and real-time data processing.

## Practical Use-Cases
### PersistentList
- **Undo/Redo Operations**: Implement undo/redo functionality in applications effortlessly.
- **Time-Travel Debugging**: Enable the inspection of how data structures change over time.
- **State Management**: Particularly useful in multi-threaded or distributed systems where you want a consistent snapshot at any point in time.

### EfficientList
- **Real-Time Systems**: Efficient for large-scale data processing where performance is critical.
- **DNS Filtering**: Can be used to implement high-speed DNS filtering and lookups.
- **Data Analysis**: Suitable for scenarios requiring fast sorting and searching over large datasets.

## Getting Started
### Installation
Install via NuGet Package Manager:

```bash
Install-Package Lakatos.Collections
```


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

## Practical Scenarios for Use

### PersistentList
- **Undo/Redo Functionalities**: Useful for text editors, drawing apps, and anywhere where reverting operations are needed.
- **Data Flow Systems**: In systems where data passes through many transformations, immutable collections help track and validate every stage.
- **Multithreaded Applications**: When multiple threads need to access a collection without risk of corruption or race conditions.

### EfficientList
- **DNS Filtering**: Ideal for implementing DNS filters that check and respond to millions of queries efficiently.
- **Data Analysis**: Perfect for applications where large datasets need to be sorted and searched in real-time.
- **Scalable Real-Time Systems**: Handles large-scale data processing with high performance, suitable for use in services like log processing and monitoring systems.

## License
This project is licensed under the MIT License. You are free to use, modify, and distribute the code with proper attribution. See the LICENSE file for more details.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change. Any contributions are highly appreciated!

## Contact
Author: Milan Lakatos Petrovic

