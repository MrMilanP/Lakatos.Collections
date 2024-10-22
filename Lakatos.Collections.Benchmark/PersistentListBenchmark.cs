using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Lakatos.Collections.Persistent;
using System.Collections.Immutable;
using System.Linq;

namespace Lakatos.Collections.Benchmark
{
    public class PersistentListBenchmark
    {
        private PersistentList<int> persistentList;
        private ImmutableList<int> immutableList;

        [GlobalSetup]
        public void Setup()
        {
            persistentList = PersistentList<int>.Empty;
            immutableList = ImmutableList<int>.Empty;

            for (int i = 0; i < 200; i++)
            {
                persistentList = persistentList.Add(i);
                immutableList = immutableList.Add(i);
            }
        }

        [Benchmark]
        public void AddToPersistentList()
        {
            var list = persistentList;
            for (int i = 0; i < 200; i++)
            {
                list = list.Add(i);
            }
        }

        [Benchmark]
        public void AddToImmutableList()
        {
            var list = immutableList;
            for (int i = 0; i < 200; i++)
            {
                list = list.Add(i);
            }
        }

        [Benchmark]
        public void RemoveFromPersistentList()
        {
            var list = persistentList;
            for (int i = 0; i < 200; i++)
            {
                if (!list.IsEmpty)
                {
                    list = list.Remove();
                }
            }
        }

        [Benchmark]
        public void RemoveFromImmutableList()
        {
            var list = immutableList;
            for (int i = 0; i < 200; i++)
            {
                if (list.Count > 0)
                {
                    list = list.RemoveAt(0);
                }
            }
        }

        [Benchmark]
        public void FindInPersistentList()
        {
            persistentList.Find(x => x == 100);
        }

        [Benchmark]
        public void FindInImmutableList()
        {
            immutableList.FirstOrDefault(x => x == 100);
        }

        [Benchmark]
        public void MapPersistentList()
        {
            var mappedList = persistentList.Map(x => x * 2);
        }

        [Benchmark]
        public void ToListPersistent()
        {
            var list = persistentList.ToList();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<PersistentListBenchmark>();
        }
    }
}
