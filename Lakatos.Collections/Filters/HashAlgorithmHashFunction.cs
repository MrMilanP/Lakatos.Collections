using System;
using System.Security.Cryptography;
using System.Text;

namespace Lakatos.Collections.Filters
{
    public class HashAlgorithmHashFunction : IHashFunction
    {
        private readonly HashAlgorithm _hashAlgorithm;

        public HashAlgorithmHashFunction(HashAlgorithm hashAlgorithm)
        {
            // Validacija da li je algoritam podržan
            if (!IsSupportedHashAlgorithm(hashAlgorithm))
            {
                throw new ArgumentException("Unsupported hash algorithm. Supported algorithms are: MD5, SHA1, and SHA256.");
            }

            _hashAlgorithm = hashAlgorithm;
        }

        public int ComputeHash(string input, int seed)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            byte[] hash = _hashAlgorithm.ComputeHash(data);

            // Koristimo seed za dodatnu varijaciju u hash vrednosti
            int modifiedHash = BitConverter.ToInt32(hash, 0) ^ seed;
            return modifiedHash;
        }

        private bool IsSupportedHashAlgorithm(HashAlgorithm algorithm)
        {
            return algorithm is MD5 || algorithm is SHA1 || algorithm is SHA256;
        }
    }
}
