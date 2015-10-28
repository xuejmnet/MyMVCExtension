using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Dow.SSD.Framework.Infrastructure
{
    public class PickMapAttrribute:Attribute,IMetadataAware
    {
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            throw new NotImplementedException();
        }
    }
}
