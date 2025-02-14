using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRegex.Codes
{
    /// <summary>
    /// The pattern code for UTF16 range check.
    /// </summary>
    public class RangeCode : PatternCode
    {

        /// <summary>
        /// The start of the UTF16 range (inclusive).
        /// </summary>
        public char RangeStart { get; }

        /// <summary>
        /// The end of the UTF16 range (inclusive).
        /// </summary>
        public char RangeEnd { get; }

        /// <summary>
        /// Creates a new range pattern.
        /// </summary>
        /// <param name="rangeStart">The start range.</param>
        /// <param name="rangeEnd">The end range.</param>
        public RangeCode(char rangeStart, char rangeEnd)
        {
            RangeStart = rangeStart;
            RangeEnd = rangeEnd;
        }
        
        /// <inheritdoc/>
        public override PatternCode TryParse(string pattern, int startIndex, out int endIndex, Dictionary<string, int> featureNames)
        {
            endIndex = startIndex;
            if (pattern.Length - startIndex < 5) return null;
            if (pattern[startIndex] != '[' || pattern[startIndex + 2] != '-' || pattern[startIndex + 4] != ']') return null;
            char s = pattern[startIndex + 1];
            char e = pattern[startIndex + 3];

            endIndex = startIndex + 5;
            return new RangeCode(s, e);
        }

        /// <inheritdoc/>
        protected override int GetPatternLength(string text, int startIndex, FeatureData data)
        {
            char c = text[startIndex];

            int result = c >= RangeStart && c <= RangeEnd ? 1 : -1;

            if (result != -1 && FeatureName != null)
                data.AddFeature(FeatureName, text.Substring(startIndex, result));

            return result;
        }
    }
}
