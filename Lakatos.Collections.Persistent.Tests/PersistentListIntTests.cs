using Xunit;
using Lakatos.Collections.Persistent;
using System;
using Xunit.Abstractions;

namespace Lakatos.Collections.Persistent.Tests
{
    public class PersistentListIntTests
    {
        private readonly ITestOutputHelper _output;

        public PersistentListIntTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Add_ShouldAddElementToList()
        {
            // Arrange
            var list = PersistentList<int>.Empty;

            // Log initial state
            _output.WriteLine("Original list: " + PrintList(list));

            // Act
            var newList = list.Add(5);

            

            // Log state after adding
            _output.WriteLine("List after adding element 5: " + PrintList(newList));

            // Assert
            Assert.True(list.IsEmpty, "Original list should remain unchanged");
            Assert.False(newList.IsEmpty, "The new list should not be empty");
            Assert.Equal(5, newList.Head);
            Assert.True(newList.Tail!.IsEmpty, "Tail of new list should be empty");
        }

        [Fact]
        public void Remove_ShouldRemoveElementFromList()
        {
            // Arrange
            var list = PersistentList<int>.Empty.Add(5).Add(10);

            // Log initial state
            _output.WriteLine("Original list: " + PrintList(list));

            // Act
            var newList = list.Remove();

            // Log state after removing
            _output.WriteLine("List after removing head: " + PrintList(newList));

            // Assert
            Assert.Equal(5, newList.Head);
            Assert.False(newList.IsEmpty);
            Assert.True(newList.Tail!.IsEmpty, "Tail should now be empty");
        }

        [Fact]
        public void GetHead_ShouldReturnCorrectValue()
        {
            // Arrange
            var list = PersistentList<string>.Empty.Add("Hello").Add("World");

            // Log initial state
            _output.WriteLine("Original list: " + PrintList(list));

            // Act
            var head = list.Head;

            // Log retrieved head
            _output.WriteLine("Head of the list: " + head);

            // Assert
            Assert.Equal("World", head);
        }

        [Fact]
        public void Tail_ShouldReturnRemainingElements()
        {
            // Arrange
            var list = PersistentList<int>.Empty.Add(1).Add(2).Add(3);

            // Log initial state
            _output.WriteLine("Original list: " + PrintList(list));

            // Act
            var tail = list.Tail;

            // Log the tail of the list
            _output.WriteLine("Tail of the list: " + PrintList(tail));

            // Assert
            Assert.Equal(2, tail.Head);
            Assert.Equal(1, tail.Tail!.Head);
        }

        [Fact]
        public void IsEmpty_ShouldReturnTrueForEmptyList()
        {
            // Arrange
            var list = PersistentList<int>.Empty;

            // Log the state of the list
            _output.WriteLine("List state: " + PrintList(list));

            // Assert
            Assert.True(list.IsEmpty, "Empty list should return true for IsEmpty");
        }

        [Fact]
        public void Find_ShouldReturnCorrectElement()
        {
            // Arrange
            var list = PersistentList<int>.Empty.Add(1).Add(2).Add(3);

            // Log initial state
            _output.WriteLine("Original list: " + PrintList(list));

            // Act
            var found = list.Find(x => x == 2);

            // Log the found element
            _output.WriteLine("Found element: " + (found != null ? found.ToString() : "None"));

            // Assert
            Assert.Equal(2, found);
        }

        [Fact]
        public void Find_ShouldReturnDefaultIfElementNotFound()
        {
            // Arrange
            var list = PersistentList<int>.Empty.Add(1).Add(2).Add(3);

            // Log initial state
            _output.WriteLine("Original list: " + PrintList(list));

            // Act
            var found = list.Find(x => x == 4);

            // Log result of the find operation
            _output.WriteLine("Element 4 found: " + (found != null ? found.ToString() : "None"));

            // Assert
            Assert.Equal(default(int), found);
        }

        [Fact]
        public void TryFind_ShouldReturnTrueAndValueIfElementIsFound()
        {
            // Arrange
            var list = PersistentList<int>.Empty.Add(1).Add(2).Add(3);

            // Log initial state
            _output.WriteLine("Original list: " + PrintList(list));

            // Act
            var result = list.TryFind(x => x == 2, out var foundValue);

            // Log the result of the TryFind
            _output.WriteLine("TryFind for 2: Result=" + result + ", Found Value=" + foundValue);

            // Assert
            Assert.True(result);
            Assert.Equal(2, foundValue);
        }

        [Fact]
        public void TryFind_ShouldReturnFalseIfElementNotFound()
        {
            // Arrange
            var list = PersistentList<int>.Empty.Add(1).Add(2).Add(3);

            // Log initial state
            _output.WriteLine("Original list: " + PrintList(list));

            // Act
            var result = list.TryFind(x => x == 4, out var foundValue);

            // Log the result of the TryFind
            _output.WriteLine("TryFind for 4: Result=" + result + ", Found Value=" + foundValue);

            // Assert
            Assert.False(result);
            Assert.Equal(default(int), foundValue);
        }

        [Fact]
        public void Filter_ShouldReturnElementsMatchingPredicate()
        {
            // Arrange
            var list = PersistentList<int>.Empty.Add(1).Add(2).Add(3).Add(4);

            // Log the original list
            _output.WriteLine("Original list: " + PrintList(list));

            // Act
            var filtered = list.Filter(x => x % 2 == 0);

            // Log the filtered list
            _output.WriteLine("Filtered list (even numbers): " + PrintList(filtered));

            // Assert
            Assert.Equal(4, filtered.Head);
            Assert.Equal(2, filtered.Tail!.Head);
            Assert.True(filtered.Tail!.Tail!.IsEmpty);
        }

        [Fact]
        public void Map_ShouldTransformElements()
        {
            // Arrange
            var list = PersistentList<int>.Empty.Add(1).Add(2).Add(3);

            // Log the original list
            _output.WriteLine("Original list: " + PrintList(list));

            // Act
            var mapped = list.Map(x => x * 2);

            // Log the mapped list
            _output.WriteLine("Mapped list (elements * 2): " + PrintList(mapped));

            // Assert
            Assert.Equal(6, mapped.Head);
            Assert.Equal(4, mapped.Tail!.Head);
            Assert.Equal(2, mapped.Tail!.Tail!.Head);
            Assert.True(mapped.Tail!.Tail!.Tail!.IsEmpty);
        }

        [Fact]
        public void Concat_ShouldReturnConcatenatedList()
        {
            // Arrange
            var list1 = PersistentList<int>.Empty.Add(1).Add(2);
            var list2 = PersistentList<int>.Empty.Add(3).Add(4);

            // Print the original lists
            _output.WriteLine("List 1: " + PrintList(list1));
            _output.WriteLine("List 2: " + PrintList(list2));

            // Act
            var concatenated = list1.Concat(list2);

            // Print the concatenated list
            _output.WriteLine("Concatenated list: " + PrintList(concatenated));

            // Assert
            Assert.Equal(2, concatenated.Head);
            Assert.Equal(1, concatenated.Tail!.Head);
            Assert.Equal(4, concatenated.Tail!.Tail!.Head);
            Assert.Equal(3, concatenated.Tail!.Tail!.Tail!.Head);
            Assert.True(concatenated.Tail!.Tail!.Tail!.Tail!.IsEmpty);
        }

        private string PrintList<T>(PersistentList<T> list)
        {
            var current = list;
            var elements = new List<T>();

            while (!current.IsEmpty)
            {
                elements.Add(current.Head!);
                current = current.Tail!;
            }

            return string.Join(" -> ", elements);
        }
    }
}
