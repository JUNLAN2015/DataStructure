using System;
using System.Collections.Generic;
using Algorithms.Common;
using Algorithms.Sorting;

namespace Algorithms.Strings
{
   
    public static class EditDistance
    {
        /// <summary>
        ///     Computes the Minimum Edit Distance between two strings.
        /// </summary>
        public static long GetMinDistance(string source, string destination, EditDistanceCostsMap<long> distances)
        {
            // Validate parameters and TCost.
            if (source == null || destination == null || distances == null)
                throw new ArgumentNullException("Some of the parameters are null.");
            if (source == destination)
                return 0;

            // Dynamic Programming 3D Table
            var dynamicTable = new long[source.Length + 1, destination.Length + 1];

            // Initialize table
            for (var i = 0; i <= source.Length; ++i)
                dynamicTable[i, 0] = i;

            for (var i = 0; i <= destination.Length; ++i)
                dynamicTable[0, i] = i;

            // Compute min edit distance cost
            for (var i = 1; i <= source.Length; ++i)
            {
                for (var j = 1; j <= destination.Length; ++j)
                {
                    if (source[i - 1] == destination[j - 1])
                    {
                        dynamicTable[i, j] = dynamicTable[i - 1, j - 1];
                    }
                    else
                    {
                        var insert = dynamicTable[i, j - 1] + distances.InsertionCost;
                        var delete = dynamicTable[i - 1, j] + distances.DeletionCost;
                        var substitute = dynamicTable[i - 1, j - 1] + distances.SubstitutionCost;

                        dynamicTable[i, j] = Math.Min(insert, Math.Min(delete, substitute));
                    }
                }
            }

            // Get min edit distance cost
            return dynamicTable[source.Length, destination.Length];
        }

        /// <summary>
        ///     Overloaded method for 32-bits Integer Distances
        /// </summary>
        public static int GetMinDistance(string source, string destination, EditDistanceCostsMap<int> distances)
        {
            // Validate parameters and TCost.
            if (source == null || destination == null || distances == null)
                throw new ArgumentNullException("Some of the parameters are null.");
            var longDistance = new EditDistanceCostsMap<long>(Convert.ToInt64(distances.InsertionCost),
                Convert.ToInt64(distances.DeletionCost), Convert.ToInt64(distances.InsertionCost));

            return Convert.ToInt32(GetMinDistance(source, destination, longDistance));
        }

        /// <summary>
        ///     Overloaded method for 16-bits Integer Distances
        /// </summary>
        public static short GetMinDistance(string source, string destination, EditDistanceCostsMap<short> distances)
        {
            // Validate parameters and TCost.
            if (source == null || destination == null || distances == null)
                throw new ArgumentNullException("Some of the parameters are null.");
            var longDistance = new EditDistanceCostsMap<long>(Convert.ToInt64(distances.InsertionCost),
                Convert.ToInt64(distances.DeletionCost), Convert.ToInt64(distances.InsertionCost));

            return Convert.ToInt16(GetMinDistance(source, destination, longDistance));
        }
    }

    public class EditDistanceCostsMap<TCost> where TCost : IComparable<TCost>, IEquatable<TCost>
    {
        public TCost DeletionCost { get; set; }
        public TCost InsertionCost { get; set; }
        public TCost SubstitutionCost { get; set; }

        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        public EditDistanceCostsMap(TCost insertionCost, TCost deletionCost, TCost substitutionCost)
        {
            if (false == default(TCost).IsNumber())
                throw new ApplicationException("Invalid cost type TCost. Please choose TCost to be a number.");

            DeletionCost = deletionCost;
            InsertionCost = insertionCost;
            SubstitutionCost = substitutionCost;
        }
    }

    public static class Permutations
    {
        /// <summary>
        /// Private Helper. Computes permutations recursively for string source.
        /// </summary>
        /// <param name="s">S.</param>
        private static HashSet<string> _permutations(string source)
        {
            var perms = new HashSet<string>();

            if (source.Length == 1)
            {
                perms.Add(source);
            }
            else if (source.Length > 1)
            {
                int lastIndex = source.Length - 1;
                string lastChar = Convert.ToString(source[lastIndex]);
                string substring = source.Substring(0, lastIndex);
                perms = _mergePermutations(_permutations(substring), lastChar);
            }

            return perms;
        }

        /// <summary>
        /// Private helper. Merges a set of permutations with a character.
        /// </summary>
        private static HashSet<string> _mergePermutations(HashSet<string> permutations, string character)
        {
            var merges = new HashSet<string>();

            foreach (var perm in permutations)
            {
                for (int i = 0; i < perm.Length; ++i)
                {
                    var newMerge = perm.Insert(i, character);

                    if (!merges.Contains(newMerge))
                        merges.Add(newMerge);
                }
            }

            return merges;
        }

        /// <summary>
        /// Computes the permutations of a string.
        /// </summary>
        public static HashSet<string> ComputeDistinct(string Source)
        {
            return _permutations(Source);
        }

        /// <summary>
        /// Determines if the Other string is an anargram of the Source string.
        /// </summary>
        public static bool IsAnargram(string Source, string Other)
        {
            if (string.IsNullOrEmpty(Source) || string.IsNullOrEmpty(Other))
                return false;
            else if (Source.Length != Other.Length)
                return false;
            else if (Source == Other)
                return true;

            // Begin the Anagram check
            // Covnert strings to character arrays
            // O(N) space complexity
            var sourceCharArray = Source.ToCharArray();
            var otherCharArray = Other.ToCharArray();

            // Sort both character arrays in place using heapsort
            // O(N log N) operation
            sourceCharArray.HeapSort<char>(Comparer<char>.Default);
            otherCharArray.HeapSort<char>(Comparer<char>.Default);

            // One pass scan
            // O(N) operation
            for (int i = 0; i < sourceCharArray.Length; ++i)
                if (sourceCharArray[i] != otherCharArray[i])
                    return false;

            return true;
        }
    }
}