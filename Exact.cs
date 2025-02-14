using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRegex
{
    /// <summary>
    /// Represent a pattern that compares the text letter-by-letter with the pattern.
    /// </summary>
    public class Exact : IPattern
    {
        /// <inheritdoc/>
        public AppliedSettings Settings { get; set; }

        /// <summary>
        /// It is redundant here. as the feature is the text itself (<see cref="Text"/>).
        /// </summary>
        public string FeatureName {  get; set; }

        /// <summary>
        /// The pattern text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Creates a new Exact.
        /// </summary>
        /// <param name="t"></param>
        public Exact(string t) { Text = t; }

        /// <inheritdoc/>
        public int GetLength(string text, int startIndex, FeatureData data)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (text.Length - startIndex < Text.Length) return -1;

            for (int i = 0; i < Text.Length; i++)
            {
                if (Text[i] != text[i + startIndex]) return -1;
            }

            return Text.Length;
        }
    }
}
