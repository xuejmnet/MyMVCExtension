using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Dow.SSD.Framework
{
    public static class ControllerExtension
    {
        public static ActionResult KeepInSameView(this Controller controller)
        {
            var sourcURL = controller.Session["CurrentURL"] + string.Empty;
            if (string.IsNullOrEmpty(sourcURL))
            {
                throw new NullReferenceException("Please specify the CurrentURL in Session");
            }
            return new RedirectResult(sourcURL);
        }
    }
}
