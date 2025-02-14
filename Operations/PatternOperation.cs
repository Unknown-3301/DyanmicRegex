using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRegex.Operations
{
    /// <summary>
    /// The abstract class for any pattern operation used for composite pattern.
    /// </summary>
    public abstract class PatternOperation : IPattern
    {
        /// <summary>
        /// The left hand side pattern.
        /// </summary>
        protected IPattern p1;
        /// <summary>
        /// The right hand side pattern.
        /// </summary>
        protected IPattern p2;
        
        /// <summary>
        /// The left hand side pattern.
        /// </summary>
        public virtual IPattern P1 
        {
            get => p1;
            set
            {
                p1 = value;
                if (Settings == null) return;
                p1.Settings.Negation = p1.Settings.OriginalNegation ^ Settings.Negation;
            }
        }
        /// <summary>
        /// The right hand side pattern.
        /// </summary>
        public virtual IPattern P2
        {
            get => p2;
            set
            {
                p2 = value;
                if (Settings == null) return;
                p2.Settings.Negation = p2.Settings.OriginalNegation ^ Settings.Negation;
            }
        }

        /// <summary>
        /// The settings of the operation.
        /// </summary>
        protected AppliedSettings settings = new AppliedSettings(PatternSettings.Default);
        
        /// <inheritdoc/>
        public virtual AppliedSettings Settings
        { 
            get => settings;
            set
            {
                settings.Negation = value.Negation;
                p1.Settings.Negation = p1.Settings.OriginalNegation ^ Settings.Negation;
                p2.Settings.Negation = p2.Settings.OriginalNegation ^ Settings.Negation;
            }
        }

        /// <summary>
        /// It is redundant here.
        /// </summary>
        public string FeatureName { get; set; }

        /// <summary>
        /// The string that represents the symbol of the operation.
        /// </summary>
        public string Name { get; protected set; }

        /// <inheritdoc/>
        public abstract int GetLength(string text, int startIndex, FeatureData data);
        
        /// <summary>
        /// Create a new instance of the operation.
        /// </summary>
        /// <returns></returns>
        public abstract PatternOperation CreateNew();
    }
}
