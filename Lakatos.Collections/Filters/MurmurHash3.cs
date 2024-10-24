using System;
using System.Text;

namespace Lakatos.Collections.Filters
{
    /// <summary>
    /// Implementation of MurmurHash3 hash function.
    /// </summary>
    public class MurmurHash3 : IHashFunction
    {
        private const uint Seed = 0x9747b28c; // Default seed value for MurmurHash3

        public int ComputeHash(string input, int seed = 0)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            uint hash = seed == 0 ? Seed : (uint)seed;
            int length = data.Length;
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            // Procesiranje bloka od 4 bajta
            int i;
            for (i = 0; i <= length - 4; i += 4)
            {
                uint k = BitConverter.ToUInt32(data, i);
                k *= c1;
                k = RotateLeft(k, 15);
                k *= c2;

                hash ^= k;
                hash = RotateLeft(hash, 13);
                hash = hash * 5 + 0xe6546b64;
            }

            // Obrada preostalih bajtova (manje od 4)
            uint tail = 0;
            int remainingBytes = length % 4;
            if (remainingBytes > 0)
            {
                // Shiftujemo i spajamo preostale bajtove
                for (int j = remainingBytes - 1; j >= 0; j--)
                {
                    tail |= (uint)data[i + j] << (j * 8);
                }

                tail *= c1;
                tail = RotateLeft(tail, 15);
                tail *= c2;
                hash ^= tail;
            }

            hash ^= (uint)length;
            hash = FMix(hash);

            return (int)hash;
        }


        private static uint RotateLeft(uint x, int r)
        {
            return (x << r) | (x >> (32 - r));
        }

        private static uint FMix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }
    }
}
