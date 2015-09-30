using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure
{
    public static class StringHelper
    {
        public static string ToHtmlList(this List<string> source)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("<ul>");
            foreach(var item in source)
            {
                stringBuilder.Append(string.Format("<li>{0}</li>", item));
                
            }
            stringBuilder.Append("</ul>");
            return stringBuilder.ToString();
        }
    }
}
