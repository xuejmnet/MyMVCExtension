using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure
{
    public interface IRoleProvider
    {
        bool IsUserInRole(string userID,params string[] roles);

    }
}
