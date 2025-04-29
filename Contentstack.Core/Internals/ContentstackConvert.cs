using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Contentstack.Core.Internals
{
    internal static class ContentstackConvert
    {
        #region Private Variables
        public static Int32 ToInt32(object input)
        {
            Int32 output = 0;

            try
            {
                output = Convert.ToInt32(input);
            }
            catch (Exception e)
            {
                if (e.Source != null)
                {
                    Console.WriteLine("IOException source: {0}", e.Source);
                }
            }

            return output;
        }

        public static bool ToBoolean(object input)
        {
            bool output = false;

            try
            {
                output = Convert.ToBoolean(input);
            }
            catch (Exception e)
            {
                if (e.Source != null)
                {
                    Console.WriteLine("IOException source: {0}", e.Source);
                }
            }

            return output;
        }

        public static string ToString(object input, string defaultValue = "")
        {
            string output = defaultValue;

            try
            {
                output = Convert.ToString(input);
            }
            catch (Exception e)
            {
                if (e.Source != null)
                {
                    Console.WriteLine("IOException source: {0}", e.Source);
                }
            }

            return output;
        }

        public static double ToDouble(object input)
        {
            double output = 0;

            try
            {
                output = Convert.ToDouble(input);
            }
            catch (Exception e)
            {
                if (e.Source != null) 
                { 
                    Console.WriteLine("IOException source: {0}", e.Source);
                }
            }

            return output;
        }

        public static decimal ToDecimal(object input)
        {
            decimal output = 0;

            try
            {
                output = Convert.ToDecimal(input);
            }
            catch (Exception e)
            {
                if (e.Source != null)
                {
                    Console.WriteLine("IOException source: {0}", e.Source);
                }
            }

            return output;
        }

        public static DateTime ToDateTime(object input)
        {
            DateTime output = new DateTime();

            try
            {
                output = DateTime.Parse(ContentstackConvert.ToString(input));
            }
            catch (Exception e)
            {
                if (e.Source != null)
                {
                    Console.WriteLine("IOException source: {0}", e.Source);
                }
            }

            return output;
        }

        public static string ToISODate(object input)
        {
            DateTime now = DateTime.Now;
            try
            {
                now = (DateTime)input;
            }
            catch (Exception e)
            {
                if (e.Source != null)
                {
                    Console.WriteLine("IOException source: {0}", e.Source);
                }
            }
            return now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'sszzz");
        }

        public static object GetValue(string value)
        {
            object obj = value;

            try
            {
                float tempFloat = 0;
                double tempDouble = 0;
                Int32 tempInt32 = 0;
                Int64 tempInt64 = 0;

                if (value.ToLower() == "true" || value.ToLower() == "false")
                {
                    obj = Convert.ToBoolean(value);
                }
                else if (value == "[]")
                {
                    obj = new object[0];
                }
                else if (value == "{}")
                {
                    obj = null;
                }
                else if (Int32.TryParse(value, out tempInt32))
                {
                    obj = value.ToString();
                }
                else if (Int64.TryParse(value, out tempInt64))
                {
                    obj = value.ToString();
                }
                else if (float.TryParse(value, out tempFloat))
                {
                    obj = value.ToString();
                }
                else if (Double.TryParse(value, out tempDouble))
                {
                    obj = value.ToString();
                }
                else if (value is string)
                {
                    obj = value.ToString();
                }
            }
            catch (Exception e)
            {
                if (e.Source != null)
                {
                    Console.WriteLine("IOException source: {0}", e.Source);
                }
            }

            return obj;
        }

        public static RegexOptions GetRegexOptions(string option)
        {
            switch (option)
            {
                case "i":
                    {
                        return RegexOptions.IgnoreCase;
                    }
                case "m":
                    {
                        return RegexOptions.Multiline;
                    }
                case "s":
                    {
                        return RegexOptions.Singleline;
                    }
                case "n":
                    {
                        return RegexOptions.ExplicitCapture;
                    }
                case "x":
                    {
                        return RegexOptions.IgnorePatternWhitespace;
                    }
                default:
                    {
                        return RegexOptions.None;
                    }
            }
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        #endregion

    }
}
