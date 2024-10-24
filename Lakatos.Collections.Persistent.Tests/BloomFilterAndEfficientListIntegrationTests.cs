using Lakatos.Collections.Efficient;
using Lakatos.Collections.Filters;
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
    public class BloomFilterAndEfficientListIntegrationTests
    {
        private readonly ITestOutputHelper _output;

        public BloomFilterAndEfficientListIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Should_Use_BloomFilter_And_ParallelBinarySearch_With_Correct_Setup()
        {
            // 1. Calculate the optimal size for the Bloom Filter for 10 million elements
            int expectedElements = 10_000_005; // Expected number of elements including special addresses
            double falsePositiveRate = 0.01; // Desired false positive rate (1%)
            int bloomFilterSize = CalculateOptimalBloomFilterSize(expectedElements, falsePositiveRate);
            var specialIpAddresses = new List<CharArrayWrapper>();

            // 2. Initialize the Bloom Filter with the calculated size
            var bloomFilter = new BloomFilter(bloomFilterSize);
            var efficientList = new EfficientList<CharArrayWrapper>();
            var temporaryList = new EfficientList<CharArrayWrapper>();
            var random = new Random();

            // Measure the time for generating and adding elements to the temporary list
            var generationStopwatch = new Stopwatch();
            generationStopwatch.Start();

            // Generate 10 million random IP addresses and add them to the temporary list
            for (int i = 0; i < 10_000_000; i++)
            {
                var randomIp = GenerateRandomIp(); // Generate the IP address
                var wrapper = new CharArrayWrapper(randomIp);
                temporaryList.Add(wrapper);
            }

            generationStopwatch.Stop();
            _output.WriteLine($"Time for generating and adding to the temporary list: {generationStopwatch.ElapsedMilliseconds:F3} milliseconds");

            // 4. Measure the time for adding elements to the EfficientList
            var addEfficientListStopwatch = new Stopwatch();
            addEfficientListStopwatch.Start();
            for (int i = 0; i < temporaryList.Count; i++)
            {
                var wrapper = temporaryList.Get(i);
                efficientList.Add(wrapper);
            }
            addEfficientListStopwatch.Stop();
            _output.WriteLine($"Time for adding to EfficientList: {addEfficientListStopwatch.ElapsedMilliseconds:F3} milliseconds");

            // 5. Measure the time for adding elements to the BloomFilter
            var addBloomFilterStopwatch = new Stopwatch();
            addBloomFilterStopwatch.Start();
            for (int i = 0; i < temporaryList.Count; i++)
            {
                var wrapper = temporaryList.Get(i);
                bloomFilter.Add(wrapper.ToString());
            }
            addBloomFilterStopwatch.Stop();
            _output.WriteLine($"Time for adding to BloomFilter: {addBloomFilterStopwatch.ElapsedMilliseconds:F3} milliseconds");

            // 6. Generate and add 5 special IP addresses to both the list and the Bloom Filter
            for (int i = 0; i < 5; i++)
            {
                var specialIp = new CharArrayWrapper(new char[] { '2', '5', '5', '.', '2', '5', '5', '.', '2', '5', (char)('0' + i), (char)('0' + i) });
                specialIpAddresses.Add(specialIp);

                // Add special IP addresses to EfficientList and Bloom Filter
                efficientList.Add(specialIp);
                bloomFilter.Add(specialIp.ToString());
            }

            // 7. Generate 95 random "IP addresses" that do not exist (outside valid range)
            var nonExistentIpAddresses = new List<CharArrayWrapper>();
            for (int i = 0; i < 95; i++)
            {
                var invalidIp = new CharArrayWrapper(new char[] { '2', '5', '7', '.', (char)random.Next(0, 256).ToString()[0], '.', (char)random.Next(0, 256).ToString()[0], '.', (char)random.Next(0, 256).ToString()[0] });
                nonExistentIpAddresses.Add(invalidIp);
            }

            // 8. Combine special and non-existent IP addresses for searching
            var allTestIpAddresses = new List<CharArrayWrapper>();
            allTestIpAddresses.AddRange(specialIpAddresses);
            allTestIpAddresses.AddRange(nonExistentIpAddresses);

            // Measure the time for sorting the EfficientList
            var sortStopwatch = new Stopwatch();
            sortStopwatch.Start();

            // Sort the EfficientList after adding all elements
            efficientList.ParallelSort();

            sortStopwatch.Stop();
            _output.WriteLine($"Time for sorting EfficientList: {sortStopwatch.ElapsedMilliseconds:F3} milliseconds");

            // 9. Search through the combined list using Bloom Filter and binary search
            var searchStopwatch = new Stopwatch();
            searchStopwatch.Start();

            int bloomPositiveCount = 0;
            int binarySearchCount = 0;

            foreach (var ip in allTestIpAddresses)
            {
                // Step 1: Check through the Bloom Filter
                if (bloomFilter.Contains(ip.ToString()))
                {
                    bloomPositiveCount++;

                    // Perform binary search if Bloom Filter returns a positive result
                    var foundIndex = efficientList.ParallelBinarySearch(ip);
                    if (foundIndex >= 0)
                    {
                        binarySearchCount++;
                        Assert.True(foundIndex >= 0, $"IP address {ip} should be found.");
                    }
                }
            }

            searchStopwatch.Stop();
            double elapsedMilliseconds = searchStopwatch.ElapsedTicks * (1000.0 / Stopwatch.Frequency);
            _output.WriteLine($"Time for combined search: {elapsedMilliseconds:F3} milliseconds");
            _output.WriteLine($"Bloom Filter positive count: {bloomPositiveCount}");
            _output.WriteLine($"Binary search count: {binarySearchCount}");

            // Verify that we have exactly 5 correct binary searches
            Assert.Equal(5, binarySearchCount);
        }

        // Helper method to calculate the optimal size for the Bloom Filter
        private int CalculateOptimalBloomFilterSize(int n, double p)
        {
            return (int)(-(n * Math.Log(p)) / (Math.Pow(Math.Log(2), 2)));
        }

        // Generate a random IP address
        private char[] GenerateRandomIp()
        {
            var random = new Random();
            return $"{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}".ToCharArray();
        }


    }
}
