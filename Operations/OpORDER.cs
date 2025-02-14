using PatternMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRegex.Operations
{
    /// <summary>
    /// The operation for two sequential patterns.
    /// </summary>
    public class OpORDER : PatternOperation
    {
        /// <summary>
        /// Create a new operation.
        /// </summary>
        public OpORDER() { Name = "->"; }

        /// <inheritdoc/>
        public override PatternOperation CreateNew() => new OpORDER();

        /// <inheritdoc/>
        public override int GetLength(string text, int startIndex, FeatureData data)
        {
            int l1 = P1.GetLength(text, startIndex, data);

            if (l1 == -1) return -1;

            int l2 = P2.GetLength(text, startIndex + l1, data);

            if (l2 == -1) return -1;

            return l1 + l2;
        }
    }
}
