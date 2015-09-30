using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Dow.SSD.Framework
{
    public interface ILOV : IModelCommon
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Type is Required")]
        string Type { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Display Value is Required")]
        string DisplayValue { get; set; }
        int? Order { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Value is Required")]
        string Value { get; set; }
        string ParentType { get; set; }
        Nullable<int> ParentID { get; set; }
    }
}
