using System;
using Contentstack.Core.Configuration;
using Contentstack.Core.Interfaces;

namespace Contentstack.Core.Context
{
    internal class RequestContext : IRequestContext
    {
        public IContentstackService service { get; set; }

        public ContentstackOptions config { get; set; }
        public int Retries { get; set; }
    }
}

