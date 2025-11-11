using System;
using System.Collections.Generic;
using Contentstack.Core.Models;

namespace Contentstack.Core.Tests.Models
{
    /// <summary>
    /// Generic model for testing simple content type operations
    /// </summary>
    public class SimpleContentTypeModel
    {
        public string Uid { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public object[] Tags { get; set; }
        public List<object> Reference { get; set; }
        public DateTime Created_at { get; set; }
        public string Created_by { get; set; }
        public DateTime Updated_at { get; set; }
        public string Updated_by { get; set; }
    }
}

