using Lakatos.Collections.Efficient;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Lakatos.Collections.Persistent.Tests
{
    public class EfficientListTestsWithStrings
    {
        private readonly ITestOutputHelper _output;

        public EfficientListTestsWithStrings(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Should_Find_Known_IP_Address_Using_QuickSort_WithStrings()
        {
            var efficientList = new EfficientList<string>();
            var random = new Random();
            //'2', '5', '5', '.', '2', '5', '1', '.', '2', '5', '0', '.', '1', '5', '8' 
            var knownIpAddress = "255.251.250.158";

            var creationStopwatch = new Stopwatch();
            creationStopwatch.Start();

            for (int i = 0; i < 9999999; i++)
            {
                var randomIp = GenerateRandomIpString();
                efficientList.Add(randomIp);
            }

            int randomPosition = random.Next(0, 9999999);
            efficientList.InsertAt(randomPosition, knownIpAddress);

            creationStopwatch.Stop();
            _output.WriteLine($"Time to create list: {creationStopwatch.ElapsedMilliseconds:F3} ms");

            var sortStopwatch = new Stopwatch();
            sortStopwatch.Start();
            efficientList.QuickSort();
            sortStopwatch.Stop();
            _output.WriteLine($"Time to sort list (QuickSort): {sortStopwatch.ElapsedMilliseconds:F3} ms");

            var searchStopwatch = new Stopwatch();
            searchStopwatch.Start();
            var foundIndex = efficientList.BinarySearch(knownIpAddress);
            searchStopwatch.Stop();

            double elapsedMilliseconds = searchStopwatch.ElapsedTicks * (1000.0 / Stopwatch.Frequency);
            _output.WriteLine($"Time to find element: {elapsedMilliseconds:F3} ms at index {foundIndex}");

            Assert.True(foundIndex >= 0, "The known IP address should be found in the list.");
        }

        [Fact]
        public void Should_Find_Known_IP_Address_Using_MergeSort_WithStrings()
        {
            var efficientList = new EfficientList<string>();
            var random = new Random();
            //'2', '5', '5', '.', '2', '5', '1', '.', '2', '5', '0', '.', '1', '5', '8' 
            var knownIpAddress = "255.251.250.158";

            var creationStopwatch = new Stopwatch();
            creationStopwatch.Start();

            for (int i = 0; i < 9999999; i++)
            {
                var randomIp = GenerateRandomIpString();
                efficientList.Add(randomIp);
            }

            int randomPosition = random.Next(0, 9999999);
            efficientList.InsertAt(randomPosition, knownIpAddress);

            creationStopwatch.Stop();
            _output.WriteLine($"Time to create list: {creationStopwatch.ElapsedMilliseconds:F3} ms");

            var sortStopwatch = new Stopwatch();
            sortStopwatch.Start();
            efficientList.MergeSort();
            sortStopwatch.Stop();
            _output.WriteLine($"Time to sort list (QuickSort): {sortStopwatch.ElapsedMilliseconds:F3} ms");

            var searchStopwatch = new Stopwatch();
            searchStopwatch.Start();
            var foundIndex = efficientList.BinarySearch(knownIpAddress);
            searchStopwatch.Stop();

            double elapsedMilliseconds = searchStopwatch.ElapsedTicks * (1000.0 / Stopwatch.Frequency);
            _output.WriteLine($"Time to find element: {elapsedMilliseconds:F3} ms at index {foundIndex}");

            Assert.True(foundIndex >= 0, "The known IP address should be found in the list.");
        }


        [Fact]
        public void Should_Find_Known_IP_Address_Using_ParallelSort_WithStrings()
        {
            var efficientList = new EfficientList<string>();
            var random = new Random();
            //'2', '5', '5', '.', '2', '5', '1', '.', '2', '5', '0', '.', '1', '5', '8' 
            var knownIpAddress = "255.251.250.158";

            var creationStopwatch = new Stopwatch();
            creationStopwatch.Start();

            for (int i = 0; i < 9999999; i++)
            {
                var randomIp = GenerateRandomIpString();
                efficientList.Add(randomIp);
            }

            int randomPosition = random.Next(0, 9999999);
            efficientList.InsertAt(randomPosition, knownIpAddress);

            creationStopwatch.Stop();
            _output.WriteLine($"Time to create list: {creationStopwatch.ElapsedMilliseconds:F3} ms");

            var sortStopwatch = new Stopwatch();
            sortStopwatch.Start();
            efficientList.ParallelSort();
            sortStopwatch.Stop();
            _output.WriteLine($"Time to sort list (QuickSort): {sortStopwatch.ElapsedMilliseconds:F3} ms");

            var searchStopwatch = new Stopwatch();
            searchStopwatch.Start();
            var foundIndex = efficientList.BinarySearch(knownIpAddress);
            searchStopwatch.Stop();

            double elapsedMilliseconds = searchStopwatch.ElapsedTicks * (1000.0 / Stopwatch.Frequency);
            _output.WriteLine($"Time to find element: {elapsedMilliseconds:F3} ms at index {foundIndex}");

            Assert.True(foundIndex >= 0, "The known IP address should be found in the list.");
        }

        [Fact]
        public void Should_Find_Multiple_Known_IP_Addresses_Using_ParallelBinarySearch_WithStrings()
        {
            var efficientList = new EfficientList<string>();
            var random = new Random();
            var knownIpAddresses = new List<string>();

            for (int i = 0; i < 100; i++)
            {
                string knownIp = $"242.168.{i / 10}.{i % 10}";
                knownIpAddresses.Add(knownIp);
            }

            // Kreiranje liste
            var creationStopwatch = new Stopwatch();
            creationStopwatch.Start();

            for (int i = 0; i < 9999999; i++)
            {
                var randomIp = GenerateRandomIpString();
                efficientList.Add(randomIp);
            }

            // Ubacivanje poznatih IP adresa na nasumične pozicije
            foreach (var ip in knownIpAddresses)
            {
                int randomPosition = random.Next(0, 9999999);
                efficientList.InsertAt(randomPosition, ip);
            }

            creationStopwatch.Stop();
            _output.WriteLine($"Time to create list: {creationStopwatch.ElapsedMilliseconds:F3} ms");

            // Sortiranje liste
            var sortStopwatch = new Stopwatch();
            sortStopwatch.Start();
            efficientList.ParallelSort();
            sortStopwatch.Stop();
            _output.WriteLine($"Time to sort list (ParallelSort): {sortStopwatch.ElapsedMilliseconds:F3} ms");

            // Pretraga svih poznatih IP adresa
            var searchStopwatch = new Stopwatch();
            searchStopwatch.Start();

            var foundAll = knownIpAddresses.All(ip => efficientList.ParallelBinarySearch(ip) >= 0);

            searchStopwatch.Stop();

            double elapsedMilliseconds = searchStopwatch.ElapsedTicks * (1000.0 / Stopwatch.Frequency);
            _output.WriteLine($"Time to find all elements: {elapsedMilliseconds:F3} milliseconds");

            Assert.True(foundAll, "All known IP addresses should be found in the list.");
        }

        // Helper method to generate random IP addresses as strings
        private string GenerateRandomIpString()
        {
            var random = new Random();
            return $"{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}";
        }

    }
}
