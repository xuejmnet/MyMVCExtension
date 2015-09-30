using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure
{
    public static class LinkProviderConfig
    {
        private static ILinkProvider _currentLinkProvider = null;

        static LinkProviderConfig()
        {

        }

        public static void SetCurrentLinkProvider(ILinkProvider linkProvider)
        {
            _currentLinkProvider = linkProvider;
        }

        public static ILinkProvider GetLinkProvider()
        {
            return _currentLinkProvider;
        }
    }
}
