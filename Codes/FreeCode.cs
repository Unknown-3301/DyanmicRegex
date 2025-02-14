using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRegex.Codes
{

    /// <summary>
    /// A pattern code type for giving no constraints for any character.
    /// Note that if Negation is applied to this code, it becomes an EOF code that checks if the end of the string has been reached (since it is the opposite of allowing any char)
    /// </summary>
    public class FreeCode : PatternCode
    {
        /// <inheritdoc/>
        public override PatternCode TryParse(string pattern, int startIndex, out int endIndex, Dictionary<string, int> featureNames)
        {
            endIndex = startIndex;
            if (pattern.Length - startIndex < 2) return null;
            if (pattern[startIndex] != '[' || pattern[startIndex + 1] != ']') return null;

            endIndex = startIndex + 2;
            return new FreeCode();
        }

        /// <inheritdoc/>
        public override int GetLength(string text, int startIndex, FeatureData data)
        {
            if (Settings.Negation) //EOF Code
            {
                return startIndex >= text.Length ? 0 : (Settings.MinRepeat == 0 ? 0 : -1);
            }

            int l = text.Length - startIndex;
            int result = l < Settings.MinRepeat ? -1 : Math.Min(l, Settings.MaxRepeat);

            if (result != -1 && FeatureName != null)
                data.AddFeature(FeatureName, text.Substring(startIndex, result));

            return result;
        }

        /// <inheritdoc/>
        protected override int GetPatternLength(string text, int startIndex, FeatureData data)
        {
            throw new NotImplementedException();
        }
    }
}
