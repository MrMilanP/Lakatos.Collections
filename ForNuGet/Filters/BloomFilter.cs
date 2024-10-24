using System;
using System.Collections;
using System.Collections.Generic;

namespace Lakatos.Collections.Filters
{
    public class BloomFilter
    {
        private readonly BitArray _bitArray;
        private readonly int _size;
        private readonly IHashFunction _hashFunction;
        private readonly int[] _seeds;

        public BloomFilter(int size, IHashFunction hashFunction = null)
        {
            _size = size;
            _bitArray = new BitArray(size);
            _hashFunction = hashFunction ?? new MurmurHash3();

            // Definišemo 7 različitih seed-ova
            _seeds = new int[] { 17, 31, 61, 89, 101, 151, 197 };
        }

        public void Add(string item)
        {
            foreach (var seed in _seeds)
            {
                int hashValue = _hashFunction.ComputeHash(item, seed);
                int index = Math.Abs(hashValue % _size);
                _bitArray[index] = true;
            }
        }

        public bool Contains(string item)
        {
            foreach (var seed in _seeds)
            {
                int hashValue = _hashFunction.ComputeHash(item, seed);
                int index = Math.Abs(hashValue % _size);
                if (!_bitArray[index])
                {
                    return false;
                }
            }
            return true;
        }

        public void Clear()
        {
            _bitArray.SetAll(false);
        }

    }
}
