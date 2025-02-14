using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DynamicRegex.Codes
{
    /// <summary>
    /// The pattern that references another pattern.
    /// </summary>
    public class ReferenceCode : PatternCode
    {
        /// <summary>
        /// The referenced pattern.
        /// </summary>
        public PatternCode Reference { get; private set; }
        private string referenceName;
        
        /// <summary>
        /// 
        /// </summary>
        public ReferenceCode()
        {
        }

        /// <summary>
        /// Creates a new reference pattern.
        /// </summary>
        /// <param name="referenceName">The referenced pattern name.</param>
        public ReferenceCode(string referenceName)
        {
            this.referenceName = referenceName;
            PatternUtils.GetPatternRequests += () =>
            {
                Reference = PatternUtils.GetPatternCode(this.referenceName);
            };
        }
        
        
        /// <inheritdoc/>
        public override PatternCode TryParse(string pattern, int startIndex, out int endIndex, Dictionary<string, int> featureNames)
        {
            endIndex = startIndex;
            if (pattern.Length - startIndex < 2) return null;
            if (pattern[startIndex] != '#' || !char.IsLetterOrDigit(pattern[startIndex + 1])) return null;

            int start = startIndex + 1;
            int i = start;
            for (; i < pattern.Length; i++)
            {
                if (!char.IsLetterOrDigit(pattern[i])) break;
            }

            endIndex = i;
            string refName = pattern.Substring(start, i - start);

            return new ReferenceCode(refName);
        }
        /// <inheritdoc/>
        public override int GetLength(string text, int startIndex, FeatureData data)
        {
            //overriding settings (Disabled for now!, becuase of problems where it will override all the time even if the reference actually doesn't have any pattern settings (settings == default))
            //overriding can be done by wrapping this pattern with a composite pattern and adding settings to it.

            //AppliedSettings s = new AppliedSettings(Reference.Settings.Original);
            //Reference.Settings = Settings;

            int l = Reference.GetLength(text, startIndex, data);

            //Reference.Settings = s;

            return l;
        }
        protected override int GetPatternLength(string text, int startIndex, FeatureData data)
        {
            throw new NotImplementedException();
        }
    }
}
