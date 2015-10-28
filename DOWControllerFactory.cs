using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Dow.SSD.Framework.Infrastructure
{
    public class DOWControllerFactory:DefaultControllerFactory
    {
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (requestContext.RouteData.Values["Lang"] != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(requestContext.RouteData.Values["Lang"] as string);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(requestContext.RouteData.Values["Lang"] as string);
            }
            return base.GetControllerInstance(requestContext, controllerType);
        }
    }
}
