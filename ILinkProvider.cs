using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure
{
    public interface ILinkProvider
    {
        string GetLink(LinkContext context);
    }

    public class LinkContext
    {
        public string LinkName { get; set; }
        public Dictionary<string, string> LinkParams { get; set; }
    }
}
