using Lakatos.Collections.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lakatos.Collections.Persistent.Tests
{
    public class BloomFilterTests
    {
        [Fact]
        public void AddAndContains_WithDefaultMurmurHash_ShouldReturnTrueForAddedElement()
        {
            // Arrange
            var bloomFilter = new BloomFilter(1_000_000);

            // Act
            bloomFilter.Add("example.com");

            // Assert
            Assert.True(bloomFilter.Contains("example.com"));
            Assert.False(bloomFilter.Contains("notadded.com"));
        }

        [Fact]
        public void AddAndContains_WithMD5Hash_ShouldReturnTrueForAddedElement()
        {
            // Arrange
            var md5HashFunction = new HashAlgorithmHashFunction(MD5.Create());
            var bloomFilter = new BloomFilter(1_000_000, md5HashFunction);

            // Act
            bloomFilter.Add("example.com");

            // Assert
            Assert.True(bloomFilter.Contains("example.com"));
            Assert.False(bloomFilter.Contains("notadded.com"));
        }

        [Fact]
        public void AddAndContains_WithSHA256Hash_ShouldReturnTrueForAddedElement()
        {
            // Arrange
            var sha256HashFunction = new HashAlgorithmHashFunction(SHA256.Create());
            var bloomFilter = new BloomFilter(1_000_000, sha256HashFunction);

            // Act
            bloomFilter.Add("example.com");

            // Assert
            Assert.True(bloomFilter.Contains("example.com"));
            Assert.False(bloomFilter.Contains("notadded.com"));
        }

        [Fact]
        public void BloomFilter_ShouldThrowExceptionForUnsupportedHashAlgorithm()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                var sha512HashFunction = new HashAlgorithmHashFunction(SHA512.Create());
                var bloomFilter = new BloomFilter(1_000_000, sha512HashFunction);
            });

            Assert.Equal("Unsupported hash algorithm. Supported algorithms are: MD5, SHA1, and SHA256.", exception.Message);
        }

        [Fact]
        public void Clear_ShouldRemoveAllElementsFromBloomFilter()
        {
            // Arrange
            var bloomFilter = new BloomFilter(1_000_000);
            bloomFilter.Add("example.com");
            bloomFilter.Add("google.com");

            // Act
            bloomFilter.Clear();

            // Assert
            Assert.False(bloomFilter.Contains("example.com"));
            Assert.False(bloomFilter.Contains("google.com"));
        }
    }
}
