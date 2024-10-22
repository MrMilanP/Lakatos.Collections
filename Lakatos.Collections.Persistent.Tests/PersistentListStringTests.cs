using Xunit.Abstractions;

namespace Lakatos.Collections.Persistent.Tests
{
    public class PersistentListStringTests
    {
        private readonly ITestOutputHelper _output;

        public PersistentListStringTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Add_ShouldAddStringElementToList()
        {
            // Arrange
            var list = PersistentList<string>.Empty;

            // Act
            var newList = list.Add("Hello");

            // Print
            _output.WriteLine("Original list: " + PrintList(list));
            _output.WriteLine("New list after adding 'Hello': " + PrintList(newList));

            // Assert
            Assert.True(list.IsEmpty); // Original list should remain unchanged
            Assert.False(newList.IsEmpty); // New list should not be empty
            Assert.Equal("Hello", newList.Head); // New element should be added correctly
            Assert.True(newList.Tail!.IsEmpty); // Tail should be empty
        }

        [Fact]
        public void Remove_ShouldRemoveStringElementFromList()
        {
            // Arrange
            var list = PersistentList<string>.Empty.Add("World").Add("Hello");

            // Act
            var newList = list.Remove();

            // Print
            _output.WriteLine("Original list: " + PrintList(list));
            _output.WriteLine("New list after removing head: " + PrintList(newList));

            // Assert
            Assert.Equal("World", newList.Head); // Head should be "World" after removing "Hello"
            Assert.False(newList.IsEmpty);
            Assert.True(newList.Tail!.IsEmpty); // Tail should now be empty
        }

        [Fact]
        public void GetHead_ShouldReturnCorrectStringValue()
        {
            // Arrange
            var list = PersistentList<string>.Empty.Add("Hello").Add("World");

            // Act
            var head = list.Head;

            // Print
            _output.WriteLine("List: " + PrintList(list));
            _output.WriteLine("Head: " + head);

            // Assert
            Assert.Equal("World", head); // Head should be the last added element ("World")
        }

        [Fact]
        public void Find_ShouldReturnCorrectStringElement()
        {
            // Arrange
            var list = PersistentList<string>.Empty.Add("One").Add("Two").Add("Three");

            // Act
            var found = list.Find(x => x == "Two");

            // Print
            _output.WriteLine("List: " + PrintList(list));
            _output.WriteLine("Found element: " + found);

            // Assert
            Assert.Equal("Two", found); // Should find element "Two"
        }

        [Fact]
        public void TryFind_ShouldReturnTrueAndStringValueIfElementIsFound()
        {
            // Arrange
            var list = PersistentList<string>.Empty.Add("Alpha").Add("Beta").Add("Gamma");

            // Act
            var result = list.TryFind(x => x == "Beta", out var foundValue);

            // Print
            _output.WriteLine("List: " + PrintList(list));
            _output.WriteLine("Found element (if any): " + foundValue);

            // Assert
            Assert.True(result); // Should return true as element is found
            Assert.Equal("Beta", foundValue); // Value should be "Beta"
        }

        [Fact]
        public void Map_ShouldTransformStringElements()
        {
            // Arrange
            var list = PersistentList<string>.Empty.Add("one").Add("two").Add("three");

            // Act
            var mapped = list.Map(x => x.ToUpper());

            // Print
            _output.WriteLine("Original list: " + PrintList(list));
            _output.WriteLine("Mapped list (elements in uppercase): " + PrintList(mapped));

            // Assert
            Assert.Equal("THREE", mapped.Head); // Element "three" becomes "THREE"
            Assert.Equal("TWO", mapped.Tail!.Head); // Element "two" becomes "TWO"
            Assert.Equal("ONE", mapped.Tail!.Tail!.Head); // Element "one" becomes "ONE"
            Assert.True(mapped.Tail!.Tail!.Tail!.IsEmpty); // No more elements
        }

        [Fact]
        public void Filter_ShouldReturnMatchingStringElements()
        {
            // Arrange
            var list = PersistentList<string>.Empty.Add("apple").Add("banana").Add("cherry").Add("date");

            // Act
            var filtered = list.Filter(x => x.StartsWith("b"));

            // Print
            _output.WriteLine("Original list: " + PrintList(list));
            _output.WriteLine("Filtered list (elements starting with 'b'): " + PrintList(filtered));

            // Assert
            Assert.Equal("banana", filtered.Head); // First matching element should be "banana"
            Assert.True(filtered.Tail!.IsEmpty); // There should be no more elements
        }

        [Fact]
        public void Concat_ShouldReturnConcatenatedStringList()
        {
            // Arrange
            var list1 = PersistentList<string>.Empty.Add("Hello").Add("World");
            var list2 = PersistentList<string>.Empty.Add("Foo").Add("Bar");

            // Act
            var concatenated = list1.Concat(list2);

            // Print
            _output.WriteLine("List 1: " + PrintList(list1));
            _output.WriteLine("List 2: " + PrintList(list2));
            _output.WriteLine("Concatenated list: " + PrintList(concatenated));

            // Assert
            Assert.Equal("World", concatenated.Head); // First element should be "World" from list1
            Assert.Equal("Hello", concatenated.Tail!.Head); // Next element should be "Hello" from list1
            Assert.Equal("Bar", concatenated.Tail!.Tail!.Head); // Followed by "Bar" from list2
            Assert.Equal("Foo", concatenated.Tail!.Tail!.Tail!.Head); // Then "Foo" from list2
            Assert.True(concatenated.Tail!.Tail!.Tail!.Tail!.IsEmpty); // No more elements
        }

        [Fact]
        public void Sort_ShouldReturnSortedList()
        {
            // Arrange
            var list = PersistentList<string>.Empty.Add("Banana").Add("Apple").Add("Cherry").Add("Date");

            // Log the original list
            _output.WriteLine("Original list: " + PrintList(list));

            // Act
            var sorted = list.Sort((x, y) => x.CompareTo(y));

            // Log the sorted list
            _output.WriteLine("Sorted list: " + PrintList(sorted));

            // Assert
            Assert.Equal("Apple", sorted.Head);        // The first element alphabetically should be "Apple"
            Assert.Equal("Banana", sorted.Tail!.Head);  // The next element should be "Banana"
            Assert.Equal("Cherry", sorted.Tail!.Tail!.Head); // Then "Cherry"
            Assert.Equal("Date", sorted.Tail!.Tail!.Tail!.Head); // The last element should be "Date"
            Assert.True(sorted.Tail!.Tail!.Tail!.Tail!.IsEmpty, "The list should contain only four elements, sorted in alphabetical order");
        }

        [Fact]
        public void Get_ShouldReturnElementAtSpecifiedIndex()
        {
            // Arrange
            var list = PersistentList<string>.Empty.Add("One").Add("Two").Add("Three");

            // Log the original list
            _output.WriteLine("Original list: " + PrintList(list));

            // Act
            var element = list.Get(1);

            // Log the retrieved element
            _output.WriteLine("Element at index 1: " + element);

            // Assert
            Assert.Equal("Two", element); // Element at index 1 should be "Two"
        }

        [Fact]
        public void Get_ShouldThrowExceptionForOutOfRangeIndex()
        {
            // Arrange
            var list = PersistentList<string>.Empty.Add("One").Add("Two").Add("Three");

            // Log the original list
            _output.WriteLine("Original list: " + PrintList(list));

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Get(5)); // Should throw for out-of-bounds index
        }


        [Fact]
        public void Trim_ShouldReturnListWithSpecifiedLength()
        {
            // Arrange
            var list = PersistentList<string>.Empty.Add("One").Add("Two").Add("Three").Add("Four");

            // Log the original list
            _output.WriteLine("Original list: " + PrintList(list));

            // Act
            var trimmedList = list.Trim(2);

            // Log the trimmed list
            _output.WriteLine("Trimmed list (max length 2): " + PrintList(trimmedList));

            // Assert
            Assert.Equal("Four", trimmedList.Head); // The first element should be "Four"
            Assert.Equal("Three", trimmedList.Tail!.Head); // The second element should be "Three"
            Assert.True(trimmedList.Tail!.Tail!.IsEmpty, "The list should contain only two elements");
        }


        [Fact]
        public void ToList_ShouldConvertPersistentListToStandardList_PreservingOriginalOrder()
        {
            // Arrange
            var list = PersistentList<string>.Empty.Add("A").Add("B").Add("C");

            // Log the original list
            _output.WriteLine("Original PersistentList: " + PrintList(list));

            // Act
            var standardList = list.ToList(preserveOriginalOrder: true);

            // Log the converted list
            _output.WriteLine("Converted standard list (original order): " + string.Join(", ", standardList));

            // Assert
            Assert.Equal(3, standardList.Count);
            Assert.Equal("A", standardList[0]); // Očekujemo da je prvi element "A"
            Assert.Equal("B", standardList[1]); // Drugi element treba da bude "B"
            Assert.Equal("C", standardList[2]); // Treći element treba da bude "C"
        }

        [Fact]
        public void ToList_ShouldConvertPersistentListToStandardList_InternalOrder()
        {
            // Arrange
            var list = PersistentList<string>.Empty.Add("A").Add("B").Add("C");

            // Log the original list
            _output.WriteLine("Original PersistentList: " + PrintList(list));

            // Act
            var standardList = list.ToList(preserveOriginalOrder: false);

            // Log the converted list
            _output.WriteLine("Converted standard list (internal order): " + string.Join(", ", standardList));

            // Assert
            Assert.Equal(3, standardList.Count);
            Assert.Equal("C", standardList[0]); // Prvi element treba da bude "C"
            Assert.Equal("B", standardList[1]); // Drugi element treba da bude "B"
            Assert.Equal("A", standardList[2]); // Treći element treba da bude "A"
        }

        [Fact]
        public void Previous_ShouldReturnCorrectPreviousVersion()
        {
            // Arrange
            var initialList = PersistentList<int>.Empty.Add(1); // List: [1]
            var secondList = initialList.Add(2); // List: [2, 1]
            var thirdList = secondList.Add(3); // List: [3, 2, 1]

            // Act
            var previousOfThird = thirdList.Previous; // Should be [2, 1]
            var previousOfSecond = previousOfThird?.Previous; // Should be [1], safe access with ?.

            // Print
            _output.WriteLine("Third List: " + PrintList(thirdList));
            _output.WriteLine("Previous of Third: " + (previousOfThird != null ? PrintList(previousOfThird) : "null"));
            _output.WriteLine("Previous of Second: " + (previousOfSecond != null ? PrintList(previousOfSecond) : "null"));

            // Assert
            Assert.NotNull(previousOfThird); // Ensure it's not null
            Assert.NotNull(previousOfSecond); // Ensure it's not null
            Assert.Equal(2, previousOfThird!.Head); // The head of the previous list should be 2
            Assert.Equal(1, previousOfSecond!.Head); // The head of the previous list should be 1

            // Assert the remaining elements in each list
            Assert.Equal(1, previousOfThird.Tail.Head);
            Assert.True(previousOfSecond.Tail.IsEmpty); // The tail of the initial list should be empty
        }



        // Helper method to print the list
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