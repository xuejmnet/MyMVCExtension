using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Dow.SSD.Framework.Infrastructure
{
    public class PickListAttribute:Attribute,IMetadataAware
    {
        public string LOVType { get; private set; }
        public string ParentLOV { get; private set; }
        public PickListAttribute(string LOVType)
        {
            this.LOVType = LOVType;
        }

        public PickListAttribute(string LOVType,string parentLOV)
        {
            this.LOVType = LOVType;
            this.ParentLOV = parentLOV;
        }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add("LOVType", LOVType);
            metadata.AdditionalValues.Add("ParentLOV", ParentLOV);
        }
    }
}
