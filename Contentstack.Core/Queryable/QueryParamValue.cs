using System.Collections.Generic;

namespace Contentstack.Core.Queryable
{
    public class QueryParamValue { }

    /// <summary>
    /// Double parameter value.
    /// </summary>
    public class DoubleParameterValue : QueryParamValue
    {
        /// <summary>
        /// String value of the parameter.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Constructs ParameterValue for a single string.
        /// </summary>
        /// <param name="value"></param>
        public DoubleParameterValue(double value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Bool parameter value.
    /// </summary>
    public class BoolParameterValue : QueryParamValue
    {
        /// <summary>
        /// String value of the parameter.
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// Constructs ParameterValue for a single string.
        /// </summary>
        /// <param name="value"></param>
        public BoolParameterValue(bool value)
        {
            Value = value;
        }
    }


    /// <summary>
    /// String parameter value.
    /// </summary>
    public class StringParameterValue : QueryParamValue
    {
        /// <summary>
        /// String value of the parameter.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Constructs ParameterValue for a single string.
        /// </summary>
        /// <param name="value"></param>
        public StringParameterValue(string value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// String list parameter value.
    /// </summary>
    public class StringListParameterValue : QueryParamValue
    {
        /// <summary>
        /// List of strings value of the parameter.
        /// </summary>
        public List<string> Value { get; set; }

        /// <summary>
        /// Constructs ParameterValue for a list of strings.
        /// </summary>
        /// <param name="values"></param>
        public StringListParameterValue(List<string> values)
        {
            Value = values;
        }
    }

    /// <summary>
    /// Double list parameter value.
    /// </summary>
    public class DoubleListParameterValue : QueryParamValue
    {
        /// <summary>
        /// List of doubles value of the parameter.
        /// </summary>
        public List<double> Value
        {
            get; set;
        }

        /// <summary>
        /// Constructs ParameterValue for a list of doubles.
        /// </summary>
        /// <param name="values"></param>
        public DoubleListParameterValue(List<double> values)
        {
            Value = values;
        }
    }
}


