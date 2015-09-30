using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Dow.SSD.Framework
{
    public interface IModelCommon
    {
        [Display(AutoGenerateField=false)]
        [ScaffoldColumn(false)]
        int ID { get; set; }

        [Editable(false)]
        [Display(Name = "Created")]
        [HiddenInput(DisplayValue=false)]
        [ScaffoldColumn(false)]
        DateTime? Created { get; set; }

        [Editable(false)]
        [Display(Name = "Updated Date")]
        [HiddenInput(DisplayValue=false)]
        [ScaffoldColumn(false)]
        DateTime? Modified { get; set; }

        [Editable(false)]
        [Display(Name = "Created By")]
        [HiddenInput(DisplayValue = false)]
        [ScaffoldColumn(false)]
        string CreatedBy { get; set; }

        [Editable(false)]
        [Display(Name = "Updated By")]
        [HiddenInput(DisplayValue = false)]
        [ScaffoldColumn(false)]
        string ModifiedBy { get; set; }
    }
}
