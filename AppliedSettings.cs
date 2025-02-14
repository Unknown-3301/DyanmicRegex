using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRegex
{
    /// <summary>
    /// A class to manage pattern settings. This is mainly used to solve negation related issues with composite code.
    /// </summary>
    public class AppliedSettings
    {
        /// <summary>
        /// the original settings parsed.
        /// </summary>
        public PatternSettings Original { get; private set; }
        /// <summary>
        /// The settings after applying settings from the higher code (composite code). If the code is not related to a composite code, this is the same as the original settings.
        /// </summary>
        public PatternSettings Applied { get; set; }

        /// <summary>
        /// MinRepeat of <see cref="Original"/>.
        /// </summary>
        public int OriginalMinRepeat { get => Original.MinRepeat; }
        /// <summary>
        /// MaxRepeat of <see cref="Original"/>.
        /// </summary>
        public int OriginalMaxRepeat { get => Original.MaxRepeat; }
        /// <summary>
        /// Negation of <see cref="Original"/>.
        /// </summary>
        public bool OriginalNegation { get => Original.Negation; }

        /// <summary>
        /// MinRepeat of <see cref="Applied"/>.
        /// </summary>
        public int MinRepeat
        { 
            get => Applied.MinRepeat; 
            set 
            {
                PatternSettings s = Applied;
                s.MinRepeat = value;
                Applied = s;
            }
        }
        /// <summary>
        /// MaxRepeat of <see cref="Applied"/>.
        /// </summary>
        public int MaxRepeat
        {
            get => Applied.MaxRepeat;
            set
            {
                PatternSettings s = Applied;
                s.MaxRepeat = value;
                Applied = s;
            }
        }
        /// <summary>
        /// Negation of <see cref="Applied"/>.
        /// </summary>
        public bool Negation
        {
            get => Applied.Negation;
            set
            {
                PatternSettings s = Applied;
                s.Negation = value;
                Applied = s;
            }
        }

        /// <summary>
        /// Creates a new object. It sets Applied = original initially.
        /// </summary>
        /// <param name="original"></param>
        public AppliedSettings(PatternSettings original) {  Original = original; Applied = original; }


    }
}
