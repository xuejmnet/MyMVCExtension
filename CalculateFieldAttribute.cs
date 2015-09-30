using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Dow.SSD.Framework.Infrastructure
{
    public class CalculateFieldAttribute:Attribute,IMetadataAware
    {
        public string Expression { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression">The expression should be able like below:[Field] operator(+,-,*,/) [Field], constant does not needed to be surranded by []</param>
        public CalculateFieldAttribute(string expression)
        {
            this.Expression = expression;
        }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add("CalculateExpression", Expression);
        }
    }
}
