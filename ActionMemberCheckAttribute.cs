using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Dow.SSD.Framework.Infrastructure
{
    public class ActionMemberCheckAttribute : ActionFilterAttribute
    {
        private string[] authorizatedUserRole;

        public ActionMemberCheckAttribute(params string[] AuthorizatedUserRole)
        {
            this.authorizatedUserRole = AuthorizatedUserRole;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            var roleProvider = RoleProviderConfig.GetCurrentRoleProvider();
            if (roleProvider == null)
            {
                throw new NullReferenceException("Please specify the RoleProvider by using RoleProviderConfig.SetCurrentRoleProvider in global.asax");
            }
            var user = filterContext.RequestContext.HttpContext.User.Identity.Name;
            user = user.Substring(user.IndexOf("\\") + 1);
            if (!roleProvider.IsUserInRole(user, authorizatedUserRole))
            {
                if(filterContext.RequestContext.HttpContext.Request.HttpMethod.ToUpper()=="GET")
                {
                    filterContext.Result = new EmptyResult();
                }
                var viewResult = new ViewResult();
                viewResult.ViewBag.ErrorMsg = "You are not authorizated to perform this operation";
                viewResult.ViewName = "Error";
                filterContext.Result = viewResult;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
