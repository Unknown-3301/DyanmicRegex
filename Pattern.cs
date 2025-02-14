using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PatternMatching;
using DynamicRegex.Codes;

namespace DynamicRegex
{
    /// <summary>
    /// The main class for pattern matching.
    /// </summary>
    public class Pattern
    {
        private List<IPattern> patterns;

        /// <summary>
        /// Creates a new pattern object.
        /// </summary>
        /// <param name="pattern"></param>
        /// <exception cref="Exception"></exception>
        public Pattern(string pattern)
        {
            patterns = new List<IPattern>();
            Dictionary<string, int> featureNames = new Dictionary<string, int>();

            for (int i = 0; i < pattern.Length; i++)
            {
                Exact e = PatternUtils.TryParseExact(pattern, i, out int end1);
                
                if (e != null) patterns.Add(e);

                i = end1;
                if (i >= pattern.Length) break;

                PatternCode code = PatternUtils.TryParseCode(pattern, i, out int end2, featureNames) ?? throw new Exception($"PATTERN ERROR: Invalid Code string! at {i}");

                patterns.Add(code);
                i = end2;

                if (i >= pattern.Length) throw new Exception("PATTERN ERROR: Code blocks must be inclosed inside \"\\<...>\", the char \'>\' is missing!");
                if (pattern[i] != '>') throw new Exception("PATTERN ERROR: Code blocks must be inclosed inside \"\\<...>\", the char \'>\' is missing!");
            }

            PatternUtils.FlushRequests();
        }
        
        /// <summary>
        /// Returns the maximum length of the substring in <paramref name="text"/> starting from <paramref name="startIndex"/> that follows the pattern, or -1 if no substring was found to follow the pattern.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <param name="startIndex">The index to start checking for pattern matching.</param>
        /// <returns></returns>
        public int GetPatternLength(string text, int startIndex)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));

            //note that we dont check whether startIndex >= text.length incase of EOF pattern code [](~)

            int i = startIndex;
            int g = 0;
            FeatureData data = new FeatureData();

            //EDIT: "&& i < s.Length" was removed for the case the remaining groups are optional (minlength = 0)
            //This means that all groups must check whether the index is out of range themselves!
            while (g < patterns.Count)
            {
                int l = patterns[g].GetLength(text, i, data);

                if (l == -1) return -1;

                i += l;
                g++;
            }

            return g != patterns.Count ? -1 : (i - startIndex);
        }

        /// <summary>
        /// Returns the maximum length of the substring in <paramref name="text"/> starting from <paramref name="startIndex"/> that follows the pattern, or -1 if no substring was found to follow the pattern.
        /// <br>Additionally, it extract pattern features to <paramref name="featureMap"/>.</br>
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <param name="startIndex">The index to start checking for pattern matching.</param>
        /// <param name="featureMap"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int GetPatternLength(string text, int startIndex, out Dictionary<string, List<string>> featureMap)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));

            //note that we dont check whether startIndex >= text.length incase of EOF pattern code [](~)    

            int i = startIndex;
            int g = 0;

            FeatureData data = new FeatureData();
            featureMap = new Dictionary<string, List<string>>();

            //EDIT: "&& i < s.Length" was removed for the case the remaining groups are optional (minlength = 0)
            //This means that all groups must check whether the index is out of range themselves!
            while (g < patterns.Count)
            {
                int l = patterns[g].GetLength(text, i, data);

                if (l == -1) return -1;

                i += l;
                g++;
            }

            int result = g != patterns.Count ? -1 : (i - startIndex);

            if (result != -1)
                featureMap = data.FeatureMap;

            return result;
        }
    }
}
