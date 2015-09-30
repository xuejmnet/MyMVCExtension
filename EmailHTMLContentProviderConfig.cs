using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure
{
    public static class EmailHTMLContentProviderConfig
    {
        private static IEmailHTMLContentProvider _provider = null;

        public static void SetEmailHTMLContentProvider(IEmailHTMLContentProvider provider)
        {
            _provider = provider;
        }

        public static IEmailHTMLContentProvider GetProvider()
        {
            if(_provider==null)
            {
                throw new NullReferenceException("Before getting the Provider, please set the Provider first!");
            }
            return _provider;
        }
    }
}
