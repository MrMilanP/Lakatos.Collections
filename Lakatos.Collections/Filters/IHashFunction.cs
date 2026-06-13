namespace Lakatos.Collections.Filters
{
    /// <summary>
    /// Computes deterministic 32-bit hash values for Bloom filter indexing.
    /// </summary>
    public interface IHashFunction
    {
        /// <summary>
        /// Computes the hash value for the specified input and seed.
        /// </summary>
        int ComputeHash(string input, int seed = 0);
    }
}
