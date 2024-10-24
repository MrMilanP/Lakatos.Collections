namespace Lakatos.Collections.Filters
{
    /// <summary>
    /// Represents a hash function used in the Bloom Filter.
    /// </summary>
    public interface IHashFunction
    {
        /// <summary>
        /// Computes the hash value for the specified input.
        /// </summary>
        /// <param name="input">The input string to hash.</param>
        /// <param name="seed">An optional seed value for generating different hash values.</param>
        /// <returns>A 32-bit hash value.</returns>
        int ComputeHash(string input, int seed = 0);
    }
}
