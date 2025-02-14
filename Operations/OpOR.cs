using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRegex.Operations
{
    /// <summary>
    /// The operation for choosing either of two patterns (the longest).
    /// </summary>
    public class OpOR : PatternOperation
    {
        /// <summary>
        /// Creates a new operation.
        /// </summary>
        public OpOR() { Name = "||"; }

        /// <inheritdoc/>
        public override PatternOperation CreateNew() => new OpOR();

        /// <inheritdoc/>
        public override int GetLength(string text, int startIndex, FeatureData data)
        {
            int l1 = P1.GetLength(text, startIndex, data);

            if (l1 == -1 && settings.Negation) return -1;

            int l2 = P2.GetLength(text, startIndex, data);

            int l = settings.Negation ? Math.Min(l1, l2) : Math.Max(l1, l2);

            return l;
        }
    }
}
