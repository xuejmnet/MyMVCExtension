using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure
{
    public interface IEmailHTMLContentProvider
    {
         string GetHTML(HTMLContentContext context);
    }

    public class HTMLContentContext
    {
        public string ContentName { get; set; }
        public Dictionary<string, object> HTMLContentContextParams { get; set; }
    }
}
