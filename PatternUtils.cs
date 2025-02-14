using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicRegex.Codes;
using DynamicRegex.Operations;

namespace DynamicRegex
{
    /// <summary>
    /// A class used in library that contains useful functions
    /// </summary>
    public static class PatternUtils
    {
        #region Codes Record
        private static readonly Dictionary<string, PatternCode> codesRecord = new Dictionary<string, PatternCode>();

        /// <summary>
        /// An event that stores all pattern reference access requests.
        /// </summary>
        public static event Action GetPatternRequests = delegate { };

        /// <summary>
        /// Flush all pattern reference access requests.
        /// </summary>
        public static void FlushRequests()
        {
            GetPatternRequests();
            GetPatternRequests = delegate { }; //Pretty sure not a good way
        }
        
        /// <summary>
        /// Adds <paramref name="code"/> to a global record.
        /// </summary>
        /// <param name="codeName">A unique name to identify the code.</param>
        /// <param name="code">The code to add.</param>
        /// <exception cref="Exception"></exception>
        public static void AddCodeName(string codeName, PatternCode code)
        {
            if (codeName == null) throw new ArgumentNullException(nameof(codeName));
            if (code == null) throw new ArgumentNullException(nameof(code));

            if (codesRecord.ContainsKey(codeName)) throw new Exception($"PATTERN ERROR: Code name {codeName} already exist!");

            codesRecord.Add(codeName, code);
        }
        /// <summary>
        /// Returns a certain code from the global record using <paramref name="codeName"/>.
        /// </summary>
        /// <param name="codeName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static PatternCode GetPatternCode(string codeName)
        {
            if (codeName == null) throw new ArgumentNullException(nameof(codeName));

            if (codesRecord.TryGetValue(codeName, out PatternCode code)) return code;
            throw new Exception($"PATTERN ERROR: Code name {codeName} does not exist!");
        }
        #endregion

        #region Pattern Code stuff
        private static readonly PatternCode[] codes = new PatternCode[]
        {
            new ReferenceCode(),
            new FreeCode(),
            new RangeCode('a', 'a'),
            new CompositionCode(null),
            new ChoiceCode(new string[]{"a"}),
        };
        private static string ReadName(string code, int startIndex, out int end1)
        {
            end1 = startIndex;
            if (code[startIndex] != '$') return null;

            int i = startIndex + 1;
            for (; i < code.Length; i++)
            {
                if (!char.IsLetterOrDigit(code[i])) break;
            }

            int l = i - startIndex - 1;
            if (l <= 0) return null;

            end1 = i;

            return code.Substring(startIndex + 1, l);
        }
        private static string ReadFeatureName(string code, int startIndex, out int end1)
        {
            end1 = startIndex;
            if (code[startIndex] != '%') return null;

            int i = startIndex + 1;
            for (; i < code.Length; i++)
            {
                if (!char.IsLetterOrDigit(code[i])) break;
            }

            int l = i - startIndex - 1;
            if (l <= 0) return null;

            end1 = i;

            return code.Substring(startIndex + 1, l);
        }
        /// <summary>
        /// Tries to parse a substring of <paramref name="code"/> starting from <paramref name="startIndex"/> into a pattern code object.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex">The index where the parser finished parsing (this index is not included in the pattern object string so no need to +1)</param>
        /// <param name="features"></param>
        /// <returns></returns>
        public static PatternCode TryParseCode(string code, int startIndex, out int endIndex, Dictionary<string, int> features = null)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));

            endIndex = startIndex;
            if (code.Length - startIndex < 1) return null;

            string name = ReadName(code, startIndex, out int end1);
            string featureName = ReadFeatureName(code, end1, out int end2);

            for (int i = 0; i < codes.Length; i++)
            {
                PatternCode c = codes[i].TryParse(code, end2, out int end3, features);

                if (c != null)
                {
                    if (name != null) AddCodeName(name, c);

                    if (featureName != null && features != null)
                    {
                        if (features.TryGetValue(featureName, out int value))
                            throw new Exception($"The feature name \"{featureName}\" already exists!");

                        features.Add(featureName, 1);
                        c.FeatureName = featureName;
                    }

                    AppliedSettings s = ParseSettings(code, end3, out int end4);
                    c.Settings = s;
                    endIndex = end4;

                    return c;
                }
            }

            return null;
        }
        /// <summary>
        /// Parses a substring of <paramref name="s"/> to a code settings object
        /// <br></br>
        /// <br>Some possible state for the settings:</br>
        /// <br>"(~)" : negation only (negation, if mentioned, must be in the beginning)</br>
        /// <br>"(?)" : zero or one time </br>
        /// <br>"(*)" : zero or more times</br>
        /// <br>"(+)" : one or more times (all of these must be before stating the range explicitly)</br>
        /// <br>"(n)" : where n is an integer. This spacifies the min and max length n.</br>
        /// <br>"(n,)" : where n is an integer. This spacifies the min length n with no upper limit.</br>
        /// <br>"(n,m)" : where n is an integer. This spacifies the min length n and max length m.</br>
        /// Examples: (~,*), (~,2), (19,)
        /// </summary>
        /// <param name="s">The string containing code setting</param>
        /// <param name="startIndex">The index to start parsing from.</param>
        /// <param name="endIndex">The index after parsing</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="Exception"></exception>
        public static AppliedSettings ParseSettings(string s, int startIndex, out int endIndex)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));

            endIndex = startIndex;

            if (startIndex >= s.Length) return new AppliedSettings(PatternSettings.Default);

            if (s[startIndex] != '(') return new AppliedSettings(PatternSettings.Default);

            int i = startIndex + 1;
            for (; i < s.Length; i++)
            {
                char c = s[i];
                if (c == ')') break;
            }

            endIndex = i + 1;

            if (s.Length - i <= 0) throw new Exception("Invalid Code Settings!");

            string[] data = s.Substring(startIndex + 1, i - startIndex - 1).Split(',');

            //represents the case where a min value was found, and now a max value is expected (if it appears)
            bool max = false;

            int bits = 0;
            int pre = -1;

            PatternSettings settings = PatternSettings.Default;

            for (int j = 0; j < data.Length; j++)
            {
                SettingsTypes st = SettingsTypes.Negation; //anything

                if (data[j] == "")
                {
                    if (!max) throw new Exception("Invalid Code Settings!");

                    //this is the case like '(1,)' where the is no max value. In that case, int.MaxValue is set as max
                    st = SettingsTypes.MaxRange;
                    settings.MaxRepeat = int.MaxValue;
                }
                else
                {
                    char c = data[j][0];

                    switch (c)
                    {
                        case '~': st = SettingsTypes.Negation; settings.Negation = true; break;
                        case '?': st = SettingsTypes.Range; settings.MinRepeat = 0; settings.MaxRepeat = 1; break;
                        case '*': st = SettingsTypes.Range; settings.MinRepeat = 0; settings.MaxRepeat = int.MaxValue; break;
                        case '+': st = SettingsTypes.Range; settings.MinRepeat = 1; settings.MaxRepeat = int.MaxValue; break;
                        default:

                            if (char.IsDigit(c)) st = max ? SettingsTypes.MaxRange : SettingsTypes.MinRange;
                            else throw new Exception("Invalid Code Settings!");
                            string str = data[j];
                            int sum = 0;
                            for (int k = 0; k < str.Length; k++)
                            {
                                char c2 = str[k];
                                if (!char.IsDigit(c2)) throw new Exception("Invalid Code Settings!");

                                sum *= 10;
                                sum += c2 - '0';
                            }

                            if (!max) settings.MinRepeat = sum;
                            settings.MaxRepeat = sum;

                            max = true;

                            break;
                    }
                }

                int ist = (int)st;

                //checks if the type st was already added
                if ((bits & ist) != 0) throw new Exception("Invalid Code Settings!");

                bits |= ist;

                //to insure that the arguments come in order (the order is from the value of the SettingTypes)
                if (ist < pre) throw new Exception("Invalid Code Settings!");

                pre = ist;
            }

            return new AppliedSettings(settings);
        }

        [Flags]
        enum SettingsTypes
        {
            Negation = 0,
            MinRange = 1,
            MaxRange = 2,

            Range = MinRange | MaxRange,
        }
        #endregion

        #region Pattern Operations stuff
        private static PatternOperation[] operations =
        {
            new OpAND(),
            new OpOR(),
            new OpORDER(),
        };
        private static bool CheckName(string pattern, int startIndex, string name)
        {
            if (pattern.Length - startIndex < name.Length) return false;

            for (int i = 0; i < name.Length; i++)
            {
                if (pattern[i + startIndex] != name[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Attempts to parse a PatternOperation object from a substring string.
        /// </summary>
        /// <param name="pattern">The text containing the operation.</param>
        /// <param name="startIndex">The index to start parsing from</param>
        /// <returns></returns>
        public static PatternOperation TryParseOperation(string pattern, int startIndex)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex >= pattern.Length) return null;

            for (int i = 0; i < operations.Length; i++)
            {
                if (CheckName(pattern, startIndex, operations[i].Name))
                {
                    return operations[i].CreateNew();
                }
            }

            return null;
        }


        #endregion

        #region Pattern stuff

        /// <summary>
        /// Attempts to parse an Exact pattern object from a substring.
        /// </summary>
        /// <param name="pattern">The text containing the pattern.</param>
        /// <param name="startIndex">The index to start parsing from.</param>
        /// <param name="endIndex">The index after parsing.</param>
        /// <returns></returns>
        public static Exact TryParseExact(string pattern, int startIndex, out int endIndex)
        {
            endIndex = startIndex;

            int i = startIndex;
            bool finished = false;
            for (; i < pattern.Length; i++)
            {
                //The start of a code block is "\<" except if it is "\\<" then it would be considered "\<" as it is (not a code block)
                if (pattern[i] == '<' && i != startIndex)
                {
                    if (pattern[i - 1] == '\\')
                    {
                        if (i - 1 == startIndex) 
                        {
                            finished = true;
                        }
                        else
                        {
                            if (pattern[i - 2] != '\\') 
                            { 
                                finished = true;
                            }
                        }
                    }
                }

                if (finished)
                {
                    endIndex = i + 1;
                    i--;
                    break;
                }
            }


            int l = i - startIndex;
            if (l <= 0) return null;

            if (!finished) endIndex = i;

            return new Exact(pattern.Substring(startIndex, l).Replace("\\\\<", "\\<"));
        }
        #endregion
    }
}
