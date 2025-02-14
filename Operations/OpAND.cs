using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRegex.Operations
{
    /// <summary>
    /// The operation for choosing either two patterns (the shortest)
    /// </summary>
    public class OpAND : PatternOperation
    {
        /// <summary>
        /// Creates a new operation.
        /// </summary>
        public OpAND() { Name = "&&"; }

        /// <inheritdoc/>
        public override PatternOperation CreateNew() => new OpAND();

        /// <inheritdoc/>
        public override int GetLength(string text, int startIndex, FeatureData data)
        {
            int l1 = P1.GetLength(text, startIndex, data);

            if (l1 == -1 && !settings.Negation) return -1;

            int l2 = P2.GetLength(text, startIndex, data);

            return settings.Negation ? Math.Max(l1, l2) : Math.Min(l1, l2);
        }
    }
}
