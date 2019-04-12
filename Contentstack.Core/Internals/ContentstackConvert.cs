using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Contentstack.Core.Internals
{
    internal class ContentstackConvert
    {
        #region Private Variables
        private static JsonSerializerSettings _JsonSerializerSettings = default(JsonSerializerSettings);
        #endregion

        #region Public Functions
        public static JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                if (ContentstackConvert._JsonSerializerSettings == default(JsonSerializerSettings))
                {
                    ContentstackConvert._JsonSerializerSettings = new JsonSerializerSettings()
                    {
                        DateParseHandling = DateParseHandling.None,
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                        NullValueHandling = NullValueHandling.Ignore
                    };
                }

                return ContentstackConvert._JsonSerializerSettings;
            }
        }
        public static Int32 ToInt32(object input)
        {
            Int32 output = 0;

            try
            {
                output = Convert.ToInt32(input);
            }
            catch { }

            return output;
        }

        public static bool ToBoolean(object input)
        {
            bool output = false;

            try
            {
                output = Convert.ToBoolean(input);
            }
            catch { }

            return output;
        }

        public static string ToString(object input, string defaultValue = "")
        {
            string output = defaultValue;

            try
            {
                output = Convert.ToString(input);
            }
            catch { }

            return output;
        }

        public static double ToDouble(object input)
        {
            double output = 0;

            try
            {
                output = Convert.ToDouble(input);
            }
            catch { }

            return output;
        }

        public static decimal ToDecimal(object input)
        {
            decimal output = 0;

            try
            {
                output = Convert.ToDecimal(input);
            }
            catch { }

            return output;
        }

        public static DateTime ToDateTime(object input)
        {
            DateTime output = new DateTime();

            try
            {
                output = DateTime.Parse(ContentstackConvert.ToString(input));
            }
            catch { }

            return output;
        }

        public static string ToISODate(object input)
        {
            DateTime dt = DateTime.Now;
            string output = string.Empty;

            try
            {
                dt = (DateTime)input;
            }
            catch
            {
            }

            DateTime now = DateTime.Now;
            try
            {
                now = (DateTime)input;
            }
            catch
            {
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
                else if (value.GetType() == typeof(string))
                {
                    obj = value.ToString();
                }
            }
            catch
            { }

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

        //public static String GetMD5FromString(String value)
        //{
        //    String output;
        //    output = value.ToString().Trim();
        //    if (value.Length > 0)
        //    {
        //        try
        //        {
        //            // Create MD5 Hash
        //            MD5 md5 = new MD5CryptoServiceProvider();
        //            byte[] result = md5.ComputeHash(GenerateStreamFromString(output));
        //            StringBuilder sb = new StringBuilder();
        //            for (int i = 0; i < result.Length; i++)
        //            {
        //                sb.Append(result[i].ToString("x2"));
        //            }
        //            return sb.ToString();
        //        }
        //        catch (Exception e)
        //        {
        //            //showLog("appUtils", "------------getMD5FromString catch-|" + e.toString());
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        //public static string GetJsonFromCacheFile(string file)
        //{
        //    try
        //    {
        //    string readContents;
        //    using (StreamReader streamReader = new StreamReader(file, Encoding.UTF8))
        //    {
        //        readContents = streamReader.ReadToEnd();
        //    }
        //    return readContents;
        //    }
        //    catch (Exception e)
        //    {
        //        //showLog("appUtils", "------------getJsonFromFilec catch-|" + e.toString());
        //        return null;
        //    }
        //}
        //public static bool IsNetworkAvailable()
        //{
        //    // only recognizes changes related to Internet adapters
        //    if (NetworkInterface.GetIsNetworkAvailable())
        //    {
        //        // however, this will include all adapters
        //        NetworkInterface[] interfaces =
        //            NetworkInterface.GetAllNetworkInterfaces();
                
        //        foreach (NetworkInterface face in interfaces)
        //        {
        //            // filter so we see only Internet adapters
        //            if (face.OperationalStatus == OperationalStatus.Up)
        //            {
        //                if ((face.NetworkInterfaceType != NetworkInterfaceType.Tunnel) &&
        //                    (face.NetworkInterfaceType != NetworkInterfaceType.Loopback))
        //                {
        //                    IPv4InterfaceStatistics statistics =
        //                        face.GetIPv4Statistics();

        //                    // all testing seems to prove that once an interface
        //                    // comes online it has already accrued statistics for
        //                    // both received and sent...

        //                    if ((statistics.BytesReceived > 0) &&
        //                        (statistics.BytesSent > 0))
        //                    {
        //                        return true;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return false;
        //}
        
        public static bool GetResponseTimeFromCacheFile(string filePath, long responseTime, long DefaultCacheTime)
        {
            try
            {
               long dateDiff = (DateTime.UtcNow.Ticks - responseTime);
                long dateDiffInMin = dateDiff / (60 * 1000);


                if (dateDiffInMin > (DefaultCacheTime / 60000))
                {
                    return true;// need to send call.
                }
                else
                {
                    return false;// no need to send call.
                }

            }
            catch 
            {
                //showLog("appUtils", "------------getJsonFromFilec catch-|" + e.toString());
                return false;
            }
        }
        
        #endregion

    }
}
