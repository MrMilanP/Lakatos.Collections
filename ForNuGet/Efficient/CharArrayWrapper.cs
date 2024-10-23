using System;

namespace Lakatos.Collections.Efficient
{
    /// <summary>
    /// Represents a wrapper for a char array that implements IComparable for use in sorted collections.
    /// </summary>
    public class CharArrayWrapper : IComparable<CharArrayWrapper>
    {
        /// <summary>
        /// The underlying character array.
        /// </summary>
        public char[] Characters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharArrayWrapper"/> class.
        /// </summary>
        /// <param name="characters">The character array to wrap.</param>
        public CharArrayWrapper(char[] characters)
        {
            Characters = characters ?? throw new ArgumentNullException(nameof(characters));
        }

        /// <summary>
        /// Compares the current instance with another CharArrayWrapper and returns an integer indicating their relative order.
        /// </summary>
        /// <param name="other">The other CharArrayWrapper to compare to.</param>
        /// <returns>
        /// A value less than zero if this instance precedes the other, zero if they are equal,
        /// and greater than zero if this instance follows the other.
        /// </returns>
        public int CompareTo(CharArrayWrapper? other)
        {
            if (other == null)
                return 1;

            int lengthComparison = Characters.Length.CompareTo(other.Characters.Length);
            if (lengthComparison != 0)
                return lengthComparison;

            for (int i = 0; i < Characters.Length; i++)
            {
                int charComparison = Characters[i].CompareTo(other.Characters[i]);
                if (charComparison != 0)
                    return charComparison;
            }

            return 0;
        }

        /// <summary>
        /// Overrides the Equals method to check for equality based on the character arrays.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>True if the objects are equal; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not CharArrayWrapper other)
                return false;

            if (Characters.Length != other.Characters.Length)
                return false;

            for (int i = 0; i < Characters.Length; i++)
            {
                if (Characters[i] != other.Characters[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Overrides the GetHashCode method to provide a hash code for the character array.
        /// </summary>
        /// <returns>A hash code based on the characters in the array.</returns>
        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var character in Characters)
            {
                hash = hash * 31 + character.GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Provides a string representation of the character array.
        /// </summary>
        /// <returns>A string representation of the wrapped character array.</returns>
        public override string ToString()
        {
            return new string(Characters);
        }
    }
}
