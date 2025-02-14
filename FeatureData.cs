using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternMatching
{
    /// <summary>
    /// The class to store and manage feature maps.
    /// </summary>
    public class FeatureData
    {
        /// <summary>
        /// A mapping of each feature name to the feature text.
        /// </summary>
        public Dictionary<string, List<string>> FeatureMap { get; private set; } = new Dictionary<string, List<string>>();

        /// <summary>
        /// Adds a feature text.
        /// </summary>
        /// <param name="name">The name of the feature.</param>
        /// <param name="text">The feature.</param>
        public void AddFeature(string name, string text)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (text == null) throw new ArgumentNullException(nameof(text));

            if (!FeatureMap.TryGetValue(name, out List<string> features))
            {
                features = new List<string>();
                FeatureMap[name] = features;
            }

            features.Add(text);
        }
    }
}
