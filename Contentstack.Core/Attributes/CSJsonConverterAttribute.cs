using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Contentstack.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CSJsonConverterAttribute : Attribute
    {
        private readonly string name;
        private readonly bool isAutoloadEnable;
        private static ConcurrentDictionary<Type, List<Type>> _types = new ConcurrentDictionary<Type, List<Type>>();

        /// <summary>
        /// Name for the JsonConverter
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// To enable autoload in ContentstackClient. Default is Enable.
        /// </summary>
        public bool IsAutoloadEnable
        {
            get
            {
                return this.isAutoloadEnable;
            }
        }

        /// <summary>
        /// CSJsonConverterAttribute constructor
        /// </summary>
        /// <param name="name">Name for the JsonConverter</param>
        /// <param name="isAutoloadEnable"> To enable autoload in ContentstackClient. Default is Enable.</param>
        public CSJsonConverterAttribute(string name, bool isAutoloadEnable = true)
        {
            this.name = name;
            this.isAutoloadEnable = isAutoloadEnable;
        }

        internal static IEnumerable<Type> GetCustomAttribute(Type attribute)
        {
            if (!_types.ContainsKey(attribute))
            {
               List<Type> result = new List<Type>();
               foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            var objectType = type.GetCustomAttributes(attribute, true);
                            foreach (var attr in type.GetCustomAttributes(typeof(CSJsonConverterAttribute)))
                            {
                                CSJsonConverterAttribute ctdAttr = attr as CSJsonConverterAttribute;
                                Trace.Assert(ctdAttr != null, "cast is null");
                                if (ctdAttr.isAutoloadEnable)
                                {
                                    result.Add(type);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
                _types[attribute] = result;
            }
            return _types[attribute].ToArray();
        }
    }
}
