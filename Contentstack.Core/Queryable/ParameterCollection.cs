using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Contentstack.Core.Queryable
{
    public class ParameterCollection : SortedDictionary<string, QueryParamValue>
    {
        public ParameterCollection() : base(comparer: StringComparer.Ordinal) { }

        /// <summary>
        /// Adds a parameter with a string value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, double value)
        {
            this.Add(key, new DoubleParameterValue(value));
        }

        /// <summary>
        /// Adds a parameter with a string value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, bool value)
        {
            this.Add(key, new BoolParameterValue(value));
        }

        /// <summary>
        /// Adds a parameter with a string value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string value)
        {
            this.Add(key, new StringParameterValue(value));
        }

        /// <summary>
        /// Adds a parameter with a list-of-strings value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        public void Add(string key, List<string> values)
        {
            Add(key, new StringListParameterValue(values));
        }

        /// <summary>
        /// Adds a parameter with a list-of-doubles value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        public void Add(string key, List<double> values)
        {
            Add(key, new DoubleListParameterValue(values));
        }

        /// <summary>
        /// Converts the current parameters into a list of key-value pairs.
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetSortedParametersList()
        {
            return GetParametersEnumerable().ToList();
        }

        private IEnumerable<KeyValuePair<string, string>> GetParametersEnumerable()
        {
            foreach (var kvp in this)
            {
                var name = kvp.Key;
                var value = kvp.Value;

                switch (value)
                {
                    case BoolParameterValue boolParameterValue:
                        yield return new KeyValuePair<string, string>(name, boolParameterValue.Value.ToString().ToLower());
                        break;
                    case DoubleParameterValue doubleParameterValue:
                        yield return new KeyValuePair<string, string>(name, doubleParameterValue.Value.ToString());
                        break;
                    case StringParameterValue stringParameterValue:
                        yield return new KeyValuePair<string, string>(name, stringParameterValue.Value);
                        break;
                    case StringListParameterValue stringListParameterValue:
                        var sortedStringListParameterValue = stringListParameterValue.Value;
                        sortedStringListParameterValue.Sort(StringComparer.Ordinal);
                        foreach (var listValue in sortedStringListParameterValue)
                            yield return new KeyValuePair<string, string>($"{name}[]", listValue);
                        break;
                    case DoubleListParameterValue doubleListParameterValue:
                        var sortedDoubleListParameterValue = doubleListParameterValue.Value;
                        sortedDoubleListParameterValue.Sort();
                        foreach (var listValue in sortedDoubleListParameterValue)
                            yield return new KeyValuePair<string, string>($"{name}[]", listValue.ToString(CultureInfo.InvariantCulture));
                        break;
                    default:
                        throw new Exception("Unsupported parameter value type '" + value.GetType().FullName + "'");
                }
            }
        }
    }
}

