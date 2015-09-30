using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure
{
    public class RoleProviderConfig
    {
        private static IRoleProvider _roleProvider;
        private static IUserIDProvider _userIDProvider;
         static RoleProviderConfig()
        {
            
        }

        public static void SetCurrentRoleProvider(IRoleProvider provider)
        {
            _roleProvider = provider;
        }

        public static IRoleProvider GetCurrentRoleProvider()
        {
            return _roleProvider;
        }

        public static void SetCurrentUserIDProvider(IUserIDProvider provider)
        {
            _userIDProvider = provider;
        }

        public static IUserIDProvider GetUserIDProvider()
        {
            return _userIDProvider;
        }
    }
}
