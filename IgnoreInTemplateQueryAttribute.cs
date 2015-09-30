using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreInTemplateQueryAttribute:Attribute
    {
        [DefaultValue(false)]
        public bool IngoreInTemplateQuery
        {
            get;
            private set;
        }

        public IgnoreInTemplateQueryAttribute(bool ignore)
        {
            IngoreInTemplateQuery = ignore;
        }
    }
}
