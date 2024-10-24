using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Lakatos.Collections.Persistent.Tests
{
    public class ListTests
    {
        private readonly ITestOutputHelper _output;

        public ListTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Should_Insert_Sort_And_Search_List()
        {
            var list = new List<string>();
            var random = new Random();
            var knownIpAddress = "255.251.250.158";

            // Insert 10 miliona elemenata
            var creationStopwatch = new Stopwatch();
            creationStopwatch.Start();

            for (int i = 0; i < 9999999; i++)
            {
                list.Add(GenerateRandomIpString(random));
            }

            int randomPosition = random.Next(0, 9999999);
            list.Insert(randomPosition, knownIpAddress);

            creationStopwatch.Stop();
            _output.WriteLine($"Vreme za kreiranje liste: {creationStopwatch.ElapsedMilliseconds:F3} ms");

            // Sortiranje liste
            var sortStopwatch = new Stopwatch();
            sortStopwatch.Start();
            list.Sort();
            sortStopwatch.Stop();
            _output.WriteLine($"Vreme za sortiranje liste: {sortStopwatch.ElapsedMilliseconds:F3} ms");

            // Proverite da li je lista zaista sortirana
            for (int i = 1; i < list.Count; i++)
            {
                if (string.Compare(list[i - 1], list[i]) > 0)
                {
                    throw new InvalidOperationException("Lista nije pravilno sortirana!");
                }
            }

            // Paralelna pretraga za 100 elemenata
            var knownIpAddresses = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                knownIpAddresses.Add($"192.168.{i / 10}.{i % 10}");
            }

            var searchStopwatch = new Stopwatch();
            searchStopwatch.Start();

            var notFoundCount = 0; // Brojač neuspešnih pretraga

            // Koristimo lock da osiguramo sigurno menjanje zajedničkog stanja
            object lockObj = new object();

            Parallel.ForEach(knownIpAddresses, ip =>
            {
                int result = list.BinarySearch(ip);

                if (result < 0)
                {
                    lock (lockObj)
                    {
                        notFoundCount++; // Bezbedno inkrementiranje brojača
                    }
                }
            });

            searchStopwatch.Stop();
            double elapsedMilliseconds = searchStopwatch.ElapsedTicks * (1000.0 / Stopwatch.Frequency);
            _output.WriteLine($"Vreme za pronalaženje svih elemenata paralelno: {elapsedMilliseconds:F3} ms");

            // Ispisivanje broja neuspešnih pretraga
            _output.WriteLine($"Broj elemenata koji nisu pronađeni: {notFoundCount}");

            // Assert se sada zasniva na notFoundCount, nema potrebe za foundAll
            Assert.True(notFoundCount == 0, $"{notFoundCount} poznatih IP adresa nisu pronađene u listi.");
        }



        private string GenerateRandomIpString(Random random)
        {
            return $"{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}";
        }
    }
}
