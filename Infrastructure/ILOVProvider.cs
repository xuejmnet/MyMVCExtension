using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure.Infrastructure
{
    public interface ILOVProvider
    {
        List<ILOV> GetLOVByType(string type);
    }

}
