using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRegex.Codes
{
    /// <summary>
    /// The pattern code that checks if the substring is an element in a set of choices.
    /// </summary>
    public class ChoiceCode : PatternCode
    {
        private string[] choices;
        private int minChoice, maxChoice;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="choices">The pattern choices.</param>
        public ChoiceCode(string[] choices)
        {
            this.choices = new string[choices.Length];
            minChoice = int.MaxValue;
            maxChoice = int.MinValue;
            for (int i = 0; i < choices.Length; i++)
            {
                this.choices[i] = choices[i];
                minChoice = Math.Min(minChoice, choices[i].Length);
                maxChoice = Math.Max(maxChoice, choices[i].Length);
            }
        }

        /// <inheritdoc/>
        public override PatternCode TryParse(string pattern, int startIndex, out int endIndex, Dictionary<string, int> featureNames)
        {
            endIndex = startIndex;
            if (pattern.Length - startIndex < 3) return null;
            if (pattern[startIndex] != '[') return null;

            int i = startIndex + 1;
            for (; i < pattern.Length; i++)
            {
                if (pattern[i] == ']') break;
            }

            endIndex = i + 1;
            string[] data = pattern.Substring(startIndex + 1, i - 1 - startIndex).Split(',');

            //No choices can be empty (so "[13,,12]" is not valid)
            if (data.Any(x => x == "")) return null;

            return new ChoiceCode(data);
        }


        private static bool CheckChoice(string pattern, int startIndex, string choice)
        {
            if (pattern.Length - startIndex < choice.Length) return false;

            for (int i = 0; i < choice.Length; i++)
            {
                if (pattern[i + startIndex] != choice[i]) return false;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override int GetPatternLength(string text, int startIndex, FeatureData data)
        {
            //This is because if we allowed such case, it would cause confusion to what length to return if the pattern matches (meaning that the string did not match any of the choices).
            if (minChoice != maxChoice && Settings.Negation) throw new Exception($"PATTERN ERROR: Negation is not allowed for Choice code with different choices' lengths! (minLength = {minChoice} maxLength = {maxChoice})");

            int maxLength = -1;

            for (int i = 0; i < choices.Length; i++)
            {
                bool check = CheckChoice(text, startIndex, choices[i]);
                bool match = check ^ Settings.Negation;

                if (check && Settings.Negation) return -1;
                if (match) maxLength = Math.Max(maxLength, choices[i].Length);
            }

            if (maxLength != -1 && FeatureName != null)
                data.AddFeature(FeatureName, text.Substring(startIndex, maxLength));

            return maxLength;
        }
    }
}
