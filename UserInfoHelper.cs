using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Dow.SSD.Framework
{
    public static class UserInfoHelper
    {
        public static string GetCurrentLoginUser()
        {
            var userFullID = HttpContext.Current.Request.LogonUserIdentity.Name;
            var userID = userFullID.Substring(userFullID.IndexOf("\\") + 1);
            return userID;
        }

        public static IEnumerable<object> GetUsersBySearchKey(string key)
        {
            var returnValue = new List<object>();
            if(string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "SearchKey can not be null or empty!");
            }
            var searchResult = UserInfoHelper.DomainSearch(key);
            foreach(SearchResult item in searchResult)
            {
                dynamic obj = new ExpandoObject();
                IDictionary<string, object> dic = obj;
                foreach(var propertyName in item.Properties.PropertyNames)
                {

                    dic.Add(propertyName.ToString(), GetProperty(item, propertyName.ToString()));
                }
                returnValue.Add(obj);
            }
            return returnValue;
        }

        private static SearchResultCollection DomainSearch(string keyWord)
        {
            String DomainName = string.Empty;

            //Get the Domain Name
            try
            {
                DomainName = ConfigurationManager.AppSettings["DomainAlias"].Trim().ToString();
            }
            catch
            {
                DomainName = "DOW";
            }

            //Build the Domain Search
            DirectoryEntry DomainEntry = new DirectoryEntry("LDAP://" + DomainName);
            DirectorySearcher DomainSearch = new DirectorySearcher(DomainEntry);
            if (ConfigurationManager.AppSettings["LDAPUser"]==null)
            {
                throw new ArgumentNullException("LDAPUser", "Please specify LDAPUser in web.config");
            }
            if (ConfigurationManager.AppSettings["LDAPUserPwd"]==null)
            {
                throw new ArgumentNullException("LDAPUserPwd", "Please specify LDAPUserPwd in web.config");
            }
            DomainEntry.Username = ConfigurationManager.AppSettings["LDAPUser"].Trim().ToString();
            DomainEntry.Password = ConfigurationManager.AppSettings["LDAPUserPwd"].Trim().ToString();
            DomainSearch.PageSize = 2;

            DomainSearch.Filter = String.Format("(&(|(&(objectCategory=person)(objectClass=user))(objectCategory=group))(|(sAMAccountName={0}*)(displayName={0}*)(Givenname={0}*)(Sn={0}*)(cn={0}*)))", keyWord.Trim());

            SearchResultCollection SResultCollection = DomainSearch.FindAll();
            return SResultCollection;
        }

        private static string GetProperty(SearchResult searchResult, string PropertyName)
        {
            if (searchResult.Properties.Contains(PropertyName))
            {
                return searchResult.Properties[PropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public class WindowsBasedUserIDProvider : Dow.SSD.Framework.Infrastructure.IUserIDProvider
    {
        //Not sure if there will be any context, the param is just for the future use
        public string GetCurrentUserID(object context)
        {
            return UserInfoHelper.GetCurrentLoginUser();
        }
    }
}
