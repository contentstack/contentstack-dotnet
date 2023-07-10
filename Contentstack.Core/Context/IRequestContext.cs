using System;
using Contentstack.Core.Configuration;
using Contentstack.Core.Interfaces;

namespace Contentstack.Core.Context
{
    public interface IRequestContext
    {
        IContentstackService service { get; set; }
        ContentstackOptions config { get; set; }
        int Retries { get; set; }
    }
}

