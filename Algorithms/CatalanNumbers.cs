/***
 * Computes the Catalan Numbers. A dynamic-programming solution.
 * 
 * Wikipedia: https://en.wikipedia.org/wiki/Catalan_number
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Algorithms.Numeric
{
    public static class CatalanNumbers
    {
        /// <summary>
        /// Internal cache.
        /// By default contains the first two catalan numbers for the ranks: 0, and 1.
        /// </summary>
        private static Dictionary<uint, ulong> _catalanNumbers = new Dictionary<uint, ulong>() { { 0, 1 }, { 1, 1 } };

        /// <summary>
        /// Helper function.
        /// </summary>
        private static ulong _recursiveHelper(uint rank)
        {
            if (_catalanNumbers.ContainsKey(rank))
                return _catalanNumbers[rank];

            ulong number = 0;
            uint lastRank = rank - 1;

            for (uint i = 0; i <= lastRank; ++i)
            {
                ulong firstPart = _recursiveHelper(i);
                ulong secondPart = _recursiveHelper(lastRank - i);

                if (!_catalanNumbers.ContainsKey(i)) _catalanNumbers.Add(i, firstPart);
                if (!_catalanNumbers.ContainsKey(lastRank - i)) _catalanNumbers.Add(lastRank - i, secondPart);

                number = number + (firstPart * secondPart);
            }

            return number;
        }

        /// <summary>
        /// Public API
        /// </summary>
        public static ulong GetNumber(uint rank)
        {
            // Assert the cache is not empty.
            Debug.Assert(_catalanNumbers.Count >= 2);

            return _recursiveHelper(rank);
        }

        /// <summary>
        /// Calculate the number using the Binomial Coefficients algorithm
        /// </summary>
        public static ulong GetNumberByBinomialCoefficients(uint rank)
        {
            // Calculate value of 2nCn
            var catalanNumber = BinomialCoefficients.Calculate(2 * rank, rank);

            // return 2nCn/(n+1)
            return Convert.ToUInt64(catalanNumber / (rank + 1));
        }

        /// <summary>
        /// Return the list of catalan numbers between two ranks, inclusive.
        /// </summary>
        public static List<ulong> GetRange(uint fromRank, uint toRank)
        {
            List<ulong> numbers = new List<ulong>();

            if (fromRank > toRank)
                return null;

            for (uint i = fromRank; i <= toRank; ++i)
                numbers.Add(GetNumber(i));

            return numbers;
        }

    }
    public static class BinomialCoefficients
    {
        /// <summary>
        /// Calculate binomial coefficients, C(n, k).
        /// </summary>
        public static ulong Calculate(uint n, uint k)
        {
            ulong result = 1;

            // Since C(n, k) = C(n, n-k)
            if (k > n - k)
                k = n - k;

            // Calculate value of [n*(n-1)*---*(n-k+1)] / [k*(k-1)*---*1]
            for (int i = 0; i < k; ++i)
            {
                result *= Convert.ToUInt64(n - i);
                result /= Convert.ToUInt64(i + 1);
            }

            return result;
        }

    }
    public static class GreatestCommonDivisor
    {
        /// <summary>
        ///     Finds and returns the greatest common divisor of two numbers
        /// </summary>
        public static uint FindGCD(uint a, uint b)
        {
            if (a == 0)
                return b;
            if (b == 0)
                return a;

            uint _a = a, _b = b;
            var r = _a % _b;

            while (r != 0)
            {
                _a = _b;
                _b = r;
                r = _a % _b;
            }

            return _b;
        }

        /// <summary>
        ///     Determines given two numbers are relatively prime
        /// </summary>
        public static bool IsRelativelyPrime(uint a, uint b)
        {
            return FindGCD(a, b) == 1;
        }
    }
}
