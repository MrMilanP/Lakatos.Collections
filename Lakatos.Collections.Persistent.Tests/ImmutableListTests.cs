using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace Lakatos.Collections.Immutable.Tests
{
    public class ImmutableListTests
    {
        private readonly ITestOutputHelper _output;

        public ImmutableListTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Should_Insert_10_Million_Elements()
        {
            var random = new Random();
            var knownIpAddress = "255.251.250.158";
            var immutableList = ImmutableList<string>.Empty;

            var creationStopwatch = new Stopwatch();
            creationStopwatch.Start();

            // Dodavanje 10 miliona IP adresa
            for (int i = 0; i < 9999999; i++)
            {
                var randomIp = GenerateRandomIp();
                immutableList = immutableList.Add(randomIp);
            }

            // Ubacivanje poznate IP adrese na nasumičnu poziciju
            int randomPosition = random.Next(0, 9999999);
            immutableList = immutableList.Insert(randomPosition, knownIpAddress);

            creationStopwatch.Stop();
            _output.WriteLine($"Time to create ImmutableList: {creationStopwatch.ElapsedMilliseconds:F3} ms");

            Assert.Equal(10000000, immutableList.Count);
        }

        [Fact]
        public void Should_Sort_ImmutableList()
        {
            var random = new Random();
            var immutableList = ImmutableList<string>.Empty;

            // Kreiranje liste sa 10 miliona IP adresa
            for (int i = 0; i < 10000000; i++)
            {
                var randomIp = GenerateRandomIp();
                immutableList = immutableList.Add(randomIp);
            }

            var sortStopwatch = new Stopwatch();
            sortStopwatch.Start();

            // Sortiranje ImmutableList (novi list se kreira jer je immutable)
            var sortedList = immutableList.Sort();
            sortStopwatch.Stop();
            _output.WriteLine($"Time to sort ImmutableList: {sortStopwatch.ElapsedMilliseconds:F3} ms");

            Assert.Equal(10000000, sortedList.Count);
        }

        [Fact]
        public void Should_Search_Known_IP_Address()
        {
            var random = new Random();
            var knownIpAddress = "255.251.250.158";
            var immutableList = ImmutableList<string>.Empty;

            // Kreiranje liste sa 10 miliona IP adresa
            for (int i = 0; i < 9999999; i++)
            {
                var randomIp = GenerateRandomIp();
                immutableList = immutableList.Add(randomIp);
            }

            int randomPosition = random.Next(0, 9999999);
            immutableList = immutableList.Insert(randomPosition, knownIpAddress);

            // Sortiranje liste pre pretrage
            immutableList = immutableList.Sort();

            var searchStopwatch = new Stopwatch();
            searchStopwatch.Start();

            // Pretraga poznate IP adrese u sortiranom ImmutableList
            int foundIndex = immutableList.BinarySearch(knownIpAddress);
            searchStopwatch.Stop();

            double elapsedMilliseconds = searchStopwatch.ElapsedTicks * (1000.0 / Stopwatch.Frequency);
            _output.WriteLine($"Time to find element: {elapsedMilliseconds:F3} milliseconds at index {foundIndex}");

            Assert.True(foundIndex >= 0, "The known IP address should be found in the list.");
        }


        [Fact]
        public void Should_Find_Multiple_Known_IP_Addresses_Parallel()
        {
            var random = new Random();
            var knownIpAddresses = Enumerable.Range(0, 100)
                                             .Select(i => $"242.168.0.{i}")
                                             .ToList();

            var immutableList = ImmutableList<string>.Empty;

            var creationStopwatch = new Stopwatch();
            creationStopwatch.Start();

            // Kreiranje liste sa 10 miliona IP adresa
            for (int i = 0; i < 9999999; i++)
            {
                var randomIp = GenerateRandomIp();
                immutableList = immutableList.Add(randomIp);
            }

            // Ubacivanje poznatih IP adresa na nasumične pozicije
            foreach (var ip in knownIpAddresses)
            {
                int randomPosition = random.Next(0, 9999999);
                immutableList = immutableList.Insert(randomPosition, ip);
            }

            creationStopwatch.Stop();
            _output.WriteLine($"Time to create ImmutableList: {creationStopwatch.ElapsedMilliseconds:F3} ms");

            // Sortiranje liste pre pretrage
            var sortStopwatch = new Stopwatch();
            sortStopwatch.Start();
            immutableList = immutableList.Sort();
            sortStopwatch.Stop();
            _output.WriteLine($"Time to sort ImmutableList: {sortStopwatch.ElapsedMilliseconds:F3} ms");

            var searchStopwatch = new Stopwatch();
            searchStopwatch.Start();

            // Paralelna pretraga svih poznatih IP adresa
            var foundAll = true;
            Parallel.ForEach(knownIpAddresses, ip =>
            {
                if (immutableList.BinarySearch(ip) < 0)
                {
                    foundAll = false;
                }
            });

            searchStopwatch.Stop();
            double elapsedMilliseconds = searchStopwatch.ElapsedTicks * (1000.0 / Stopwatch.Frequency);
            _output.WriteLine($"Time to find all elements in parallel: {elapsedMilliseconds:F3} milliseconds");

            Assert.True(foundAll, "All known IP addresses should be found in the list.");
        }

        private string GenerateRandomIp()
        {
            var random = new Random();
            return $"{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}";
        }
    }
}
