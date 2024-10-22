using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Lakatos.Collections.Persistent;
using Microsoft.FSharp.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Lakatos.Collections.Benchmark
{
    public class PersistentListBenchmark
    {
        private PersistentList<int> persistentList;
        private ImmutableList<int> immutableList;
        private FSharpList<int> fsharpList;

        [GlobalSetup]
        public void Setup()
        {
            persistentList = PersistentList<int>.Empty;
            immutableList = ImmutableList<int>.Empty;
            fsharpList = FSharpList<int>.Empty;

            for (int i = 0; i < 1000; i++)
            {
                persistentList = persistentList.Add(i);
                immutableList = immutableList.Add(i);
                fsharpList = FSharpList<int>.Cons(i, fsharpList);
            }
        }

        [Benchmark]
        public void AddToPersistentList()
        {
            var list = persistentList;
            for (int i = 0; i < 1000; i++)
            {
                list = list.Add(i);
            }
        }

        [Benchmark]
        public void AddToImmutableList()
        {
            var list = immutableList;
            for (int i = 0; i < 1000; i++)
            {
                list = list.Add(i);
            }
        }

        [Benchmark]
        public void AddToFSharpList()
        {
            var list = fsharpList;
            for (int i = 0; i < 1000; i++)
            {
                list = FSharpList<int>.Cons(i, list); 
            }
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
