using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatternMatching;
using DynamicRegex.Operations;

namespace DynamicRegex.Codes
{
    /// <summary>
    /// The pattern code for combining other pattern codes to create more complex codes.
    /// </summary>
    public class CompositionCode : PatternCode
    {
        /// <inheritdoc/>
        public override AppliedSettings Settings
        { 
            get => settings;
            set
            {
                settings = value;

                if (code is PatternOperation)
                    code.Settings = value;
                else
                    code.Settings.Negation = code.Settings.OriginalNegation ^ value.Negation;
            }
        }

        private IPattern code;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="code"></param>
        public CompositionCode(IPattern code)
        {
            this.code = code;
        }
        /// <inheritdoc/>
        public override PatternCode TryParse(string pattern, int startIndex, out int endIndex, Dictionary<string, int> featureNames)
        {
            endIndex = startIndex;
            if (pattern.Length - startIndex < 3) return null;
            if (pattern[startIndex] != '[') return null;

            int i = startIndex + 1;

            IPattern current = PatternUtils.TryParseCode(pattern, i, out int end1, featureNames);
            if (current == null) return null;
            i = end1;

            List<(PatternOperation op, PatternCode pat)> values = new List<(PatternOperation op, PatternCode pat)>();

            while (i < pattern.Length)
            {
                if (pattern[i] == ']') break;

                PatternOperation op = PatternUtils.TryParseOperation(pattern, i);

                if (op == null) return null;
                i += op.Name.Length;

                PatternCode pat = PatternUtils.TryParseCode(pattern, i, out int end2, featureNames);

                if (pat == null) return null;
                i = end2;

                values.Add((op, pat));
            }

            endIndex = i + 1;

            for (int j = 0; j < values.Count; j++)
            {
                (PatternOperation op, PatternCode pat) = values[j];

                op.P1 = current;
                op.P2 = pat;

                current = op;
            }

            return new CompositionCode(current);
        }

        /// <inheritdoc/>
        protected override int GetPatternLength(string text, int startIndex, FeatureData data)
        {
            int result = code.GetLength(text, startIndex, data);

            if (result != -1 && FeatureName != null)
                data.AddFeature(FeatureName, text.Substring(startIndex, result));

            return result;
        }
    }
}
