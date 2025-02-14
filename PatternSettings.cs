using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRegex
{
    /// <summary>
    /// A pattern struct for storing modifiers.
    /// </summary>
    public struct PatternSettings
    {
        /// <summary>
        /// The minimum number of repetitions of the pattern code for the pattern to be valid.
        /// </summary>
        public int MinRepeat;
        /// <summary>
        /// The maximum number of repetitions of the pattern code for the pattern to be valid.
        /// </summary>
        public int MaxRepeat;
        /// <summary>
        /// Whether logical negation is applied to the pattern code (where it works like an inverse).
        /// </summary>
        public bool Negation;

        /// <summary>
        /// Creates a new pattern settings.
        /// </summary>
        /// <param name="minRepeat">The minimum number of repetitions of the pattern code for the pattern to be valid.</param>
        /// <param name="maxRepeat">The maximum number of repetitions of the pattern code for the pattern to be valid.</param>
        /// <param name="negation">Whether logical negation is applied to the pattern code (where it works like an inverse).</param>
        public PatternSettings(int minRepeat, int maxRepeat, bool negation)
        {
            MinRepeat = minRepeat;
            MaxRepeat = maxRepeat;
            Negation = negation;
        }

        /// <summary>
        /// The default settings for pattern codes with no explicit settings.
        /// <br>MinRepeat = 1</br>
        /// <br>MaxRepeat = 1</br>
        /// <br>Negation = false</br>
        /// </summary>
        public static readonly PatternSettings Default = new PatternSettings(1, 1, false);
        /// <summary>
        /// The settings for pattern codes where it allowes the pattern to exist or not.
        /// <br>MinRepeat = 0</br>
        /// <br>MaxRepeat = 1</br>
        /// <br>Negation = false</br>
        /// </summary>
        public static readonly PatternSettings ZeroOrOne = new PatternSettings(0, 1, false);
        /// <summary>
        /// The settings for pattern codes where it allowes the pattern to repeat multiple times, or not exist.
        /// <br>MinRepeat = 0</br>
        /// <br>MaxRepeat = int.MaxValue</br>
        /// <br>Negation = false</br>
        /// </summary>
        public static readonly PatternSettings ZeroOrMore = new PatternSettings(0, int.MaxValue, false);
        /// <summary>
        /// The settings for pattern codes where it allowes the pattern to repeat multiple times, but the pattern must appear at least once.
        /// <br>MinRepeat = 1</br>
        /// <br>MaxRepeat = int.MaxValue</br>
        /// <br>Negation = false</br>
        /// </summary>
        public static readonly PatternSettings OnceOrMore = new PatternSettings(1, int.MaxValue, false);

        /// <summary>
        /// Compares all fields in the object.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(PatternSettings x, PatternSettings y) => x.MinRepeat == y.MinRepeat && x.MaxRepeat == y.MaxRepeat && x.Negation == y.Negation;
        /// <summary>
        /// Compares all fields in the object.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(PatternSettings x, PatternSettings y) => x.MinRepeat != y.MinRepeat || x.MaxRepeat != y.MaxRepeat || x.Negation != y.Negation;
    }
}
