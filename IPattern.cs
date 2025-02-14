using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRegex
{
    /// <summary>
    /// The interface for all pattern types
    /// </summary>
    public interface IPattern
    {
        /// <summary>
        /// The settings of the pattern.
        /// </summary>
        AppliedSettings Settings { get; set; }

        /// <summary>
        /// Represents the name of this pattern that will be saved during pattern matching.
        /// </summary>
        string FeatureName { get; set; }

        /// <summary>
        /// Returns the length of the largest valid text that follows the pattern (-1 if the text is invalid to the pattern).
        /// </summary>
        /// <param name="text">The text</param>
        /// <param name="startIndex">The index to start scanning from for the pattern.</param>
        /// <param name="data">Feature data.</param>
        /// <returns></returns>
        int GetLength(string text, int startIndex, FeatureData data);
    }
}
