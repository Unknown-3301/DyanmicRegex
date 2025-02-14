using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRegex.Codes
{
    /// <summary>
    /// The abstract class for pattern codes.
    /// </summary>
    public abstract class PatternCode : IPattern
    {
        /// <inheritdoc/>
        protected AppliedSettings settings;
        /// <inheritdoc/>
        public virtual AppliedSettings Settings
        { 
            get => settings;
            set
            {
                if (settings == null) //init
                {
                    settings = value;
                    return;
                }

                settings.Applied = value.Applied;
            }
        }

        /// <inheritdoc/>
        public string FeatureName { get; set; }

        /// <inheritdoc/>
        public virtual int GetLength(string text, int startIndex, FeatureData data)
        {
            int count = 0;
            int i = startIndex;
            for (;count < Settings.MaxRepeat; count++)
            {
                if (i >= text.Length) break;
                int l = GetPatternLength(text, i, data);
                if (l == -1) break;
                i += l;
            }

            return count < Settings.MinRepeat ? -1 : i - startIndex;
        }

        //No need to check here if the index is out of range
        /// <summary>
        /// Returns the maximum length of the substring in <paramref name="text"/> starting from <paramref name="startIndex"/> that follows the pattern, or -1 if no substring was found to follow the pattern.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <param name="startIndex">The index to start checking for pattern matching.</param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract int GetPatternLength(string text, int startIndex, FeatureData data);

        /// <summary>
        /// Attempts to parse a pattern code from the pattern text.
        /// </summary>
        /// <param name="pattern">The pattern to parse from.</param>
        /// <param name="startIndex">The index to start parsing.</param>
        /// <param name="endIndex">The index after parsing.</param>
        /// <param name="featureNames"></param>
        /// <returns></returns>
        public abstract PatternCode TryParse(string pattern, int startIndex, out int endIndex, Dictionary<string, int> featureNames);
    }
}
