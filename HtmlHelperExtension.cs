using Dow.SSD.Framework.Infrastructure.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Configuration;
using Dow.SSD.Framework.Infrastructure;
using System.Text.RegularExpressions;

namespace Dow.SSD.Framework
{
    public static class HtmlHelperExtension
    {
        static ILOVProvider _lovProvider;
        static CachedDataAnnotationsModelMetadataProvider metaDataProvider = new CachedDataAnnotationsModelMetadataProvider();
        static HtmlHelperExtension()
        {
            var lovProviderConfig = ConfigurationManager.AppSettings["LOVProvider"];
            var temp = lovProviderConfig.Split(',');
            var lovProviderTypeName = temp[0];
            var lovAssembly = Assembly.GetEntryAssembly();
            if (temp.Count() > 1)
            {
                lovAssembly = Assembly.Load(temp[1]);
            }
            var lovProviderType = lovAssembly.GetType(lovProviderTypeName);
            _lovProvider = Activator.CreateInstance(lovProviderType) as ILOVProvider;
        }

        /// <summary>
        /// For fields who have [PickList] attribute binded, bind dropdownlist to that field
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <param name="propertyExpression">field expression(only support string type field)</param>
        /// <returns></returns>
        public static MvcHtmlString PickListFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, string>> propertyExpression)
        {
            return PickListFor<TModel>(helper, propertyExpression, null, string.Empty);
        }
        /// <summary>
        /// For fields who have [PickList] attribute binded, bind dropdownlist to that field
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <param name="propertyExpression">field expression(only support string type field)</param>
        /// <param name="additionalAttr"></param>
        /// <param name="blankItem"></param>
        /// <param name="additionalSelection"></param>
        /// <returns></returns>
        public static MvcHtmlString PickListFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, string>> propertyExpression, object additionalAttr, string blankItem, params string[] additionalSelection)
        {
            if(_lovProvider==null)
            {
                throw new NullReferenceException("To use the PickListFor HtmlHelperMethod, please specify the LOVProvider in web.config, the LOVProvider must implement the ILOVProvider interface");
            }
            var memberAccessExpression = propertyExpression.Body as MemberExpression;
            if (memberAccessExpression == null)
            {
                throw new ArgumentException("propertyExpression must be property access expression", "propertyExpression");
            }
            if (memberAccessExpression.Member.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException("propertyExpression must be property access expression", "propertyExpression");
            }
            var propertyExpressionString = ExpressionHelper.GetExpressionText(propertyExpression);
            
            var propertyMetaData = ModelMetadata.FromLambdaExpression(propertyExpression, helper.ViewData);
            var lovType = propertyMetaData.AdditionalValues["LOVType"] + "";
            if(lovType==string.Empty)
            {
                throw new ArgumentOutOfRangeException("propertyExpression", "the field using Picklist must have Picklist attribute");
            }
            var lovs = _lovProvider.GetLOVByType(lovType);
            var valueCalculateFunc = propertyExpression.Compile();
            object value = null;

            //In case the picklist usage as below
            //PickListFor(model=>model.Address.Zip), we need to use the actual model type(typeof(Address) here) instead of the param type (typeof(model) in this case)
            try
            {
                value = helper.ViewData.Model == null ? null : valueCalculateFunc(helper.ViewData.Model);
            }
            catch(NullReferenceException)
            {
                value = string.Empty;
            }
            var valueStr = value == null ? string.Empty : value.ToString();

            var builder = new TagBuilder("select");

            //In case the picklist usage as below
            //PickListFor(model=>model.Address.Zip), the id/name for the picklist should be Address.Zip(<select name="Address.Zip">, instead of using propertyInfo.Name(<select name="Zip">
            
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(propertyExpressionString);
            builder.Attributes.Add("name", fullName);
            //Add jquery validate attribute to the output select
            
            builder.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(propertyExpressionString, propertyMetaData));
            builder.AddCssClass("PickList");
            builder.Attributes.Add("MyLOV", lovType);
            //Add parentLOV Attribute
            if (!string.IsNullOrEmpty(propertyMetaData.AdditionalValues["ParentLOV"] + string.Empty))
            {
                builder.Attributes.Add("ParentLOV", propertyMetaData.AdditionalValues["ParentLOV"].ToString());
            }

            //Replace the '.' to '_' in order to keep aliance with the MVC standard practice
            builder.GenerateId(fullName);
            var stringBuilder = new StringBuilder();
            if (additionalAttr != null)
            {
                var attrs = additionalAttr.GetType().GetProperties();
                foreach (var attr in attrs)
                {
                    builder.Attributes.Add(attr.Name, attr.GetValue(additionalAttr, null).ToString());
                }
            }
            if (blankItem != null)
            {
                stringBuilder.Append(string.Format("<option value=\"{0}\">{0}</option>{1}", blankItem, System.Environment.NewLine));
            }
            foreach (var lov in lovs)
            {
                stringBuilder.Append(string.Format("<option value=\"{0}\" {1} lovType=\"{4}\" lovID=\"{6}\" parentType=\"{5}\" parentLovID=\"{7}\">{2}</option>{3}",
                    lov.Value,
                    lov.Value.Trim() == valueStr.Trim() ? "selected=\"selected\"" : string.Empty,
                    lov.DisplayValue,
                    System.Environment.NewLine,
                    lov.Type,
                    lov.ParentType,
                    lov.ID,
                    lov.ParentID));
            }
            builder.InnerHtml = stringBuilder.ToString();
            return new MvcHtmlString(builder.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <param name="propertyControlMapping">Mapping the LDAP property to specific input id of current view,eg: new object{cn="UserID"}</param>
        /// <returns></returns>
        public static MvcHtmlString RenderGlobalAddressBook<TModel>(this HtmlHelper<TModel> helper,object propertyControlMapping)
        {
            var returnValue = helper.Partial("GlobalAddress", propertyControlMapping);
            return returnValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <param name="prefix">when there is multiple GlobalAddressBook in current view, add a prefix to distingsh each other</param>
        /// <param name="propertyControlMapping">Mapping the LDAP property to specific input id of current view,eg: new object{cn="UserID"}</param>
        /// <returns></returns>
        public static MvcHtmlString RenderGlobalAddressBook<TModel>(this HtmlHelper<TModel> helper, string prefix,object propertyControlMapping)
        {
            ViewDataDictionary globalAddressBookViewData = new ViewDataDictionary();
            globalAddressBookViewData.Add("prefix", prefix);
            var returnValue = helper.Partial("GlobalAddress", propertyControlMapping, globalAddressBookViewData);
            return returnValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <param name="condition">The condition to dislay an HTML element(only support 1 logic word like Model.Property1=="1")</param>
        /// <returns></returns>
        public static MvcHtmlString DisplayWhen<TModel>(this HtmlHelper<TModel> helper,Expression<Func<TModel,bool>> condition)
        {
            if (!(condition.Body is BinaryExpression))
            {
                throw new InvalidOperationException("The displayWhen only support BinaryExpression");
            }
            var propertyAccessor = (condition.Body as BinaryExpression).Left;
            if(!(propertyAccessor is MemberExpression))
            {
                throw new InvalidOperationException("The left side of the condition expression must be a member accessor expression");
            }
            var conditionValueExpression = (condition.Body as BinaryExpression).Right;
            if (!(conditionValueExpression is ConstantExpression))
            {
                throw new InvalidOperationException("The right side of the condition expression must be a constant expression");
            }
            var conditionValue = (conditionValueExpression as ConstantExpression).Value;
            var triger = propertyAccessor.ToString().Substring(propertyAccessor.ToString().IndexOf(".") + 1).Replace(".", "_");
            var condtionType = condition.Body.NodeType.ToString();
            var conditionCalc = condition.Compile();
            var display = conditionCalc(helper.ViewData.Model);
            if (display)
            {
                return new MvcHtmlString(string.Format("ConditionalDisplay = 'true' trigger='{0}' conditionValue='{1}' condition='{2}'", triger, conditionValue,condtionType));
            }
            return new MvcHtmlString(string.Format("ConditionalDisplay = 'true' trigger='{0}' conditionValue='{1}'  condition='{2}' style='display:none;'", triger,conditionValue, condtionType));
        }
        /// <summary> 
        /// Display or hide certain html element when specific condition is met, client side based
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="helper"></param>
        /// <param name="condition">The condition to dislay an HTML element(only support 1 logic word like Model.Property1=="1")</param>
        /// <param name="displayMode">Display mode would be the css block style such as "table-row","table-cell","block"</param>
        /// <returns></returns>
        public static MvcHtmlString DisplayWhen<TModel>(this HtmlHelper<TModel> helper,Expression<Func<TModel,bool>> condition,string displayMode)
        {
            var returnValue = DisplayWhen<TModel>(helper, condition);
            return new MvcHtmlString(returnValue.ToHtmlString() + string.Format(" displayMode=\"{0}\"", displayMode));
        }
        /// <summary>
        /// Display or hide certain html element when specific condition is met, client side based
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="helper"></param>
        /// <param name="condition">The condition to dislay an HTML element(only support 1 logic word like Model.Property1=="1")</param>
        /// <param name="currentField">Current field means that what field is the current DisplayWhen helper conducting on</param>
        /// <returns></returns>
        public static MvcHtmlString DisplayWhen<TModel,TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel,bool>> condition, Expression<Func<TModel,TValue>> currentField)
        {
            var baseReturnValue = DisplayWhen<TModel>(helper, condition).ToString();
            if(!(currentField.Body is MemberExpression))
            {
                throw new InvalidOperationException("The currentField expression must be a member accessor expression");
            }
            var propertyExpressionString = ExpressionHelper.GetExpressionText(currentField);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(propertyExpressionString).Replace(".", "_");
            var returnValue = string.Format("{0} field='{1}'", baseReturnValue, fullName);
            return new MvcHtmlString(returnValue);
        }

        public static MvcHtmlString CalculateField<TModel,TValue>(this HtmlHelper<TModel> helper,Expression<Func<TModel,TValue>> field)
        {
            var memberExpression=field.Body as MemberExpression;
            if(memberExpression==null)
            {
                throw new ArgumentException("field must be property access expression", "field");
            }
            if(memberExpression.Member.MemberType!=MemberTypes.Property)
            {
                throw new ArgumentException("field must be property access expression", "field");
            }
            var expressionText=ExpressionHelper.GetExpressionText(field);
            var fieldMetaData = ModelMetadata.FromLambdaExpression(field, helper.ViewData);
            var calculateExpression = fieldMetaData.AdditionalValues["CalculateExpression"] + "";
            if(calculateExpression==null)
            {
                throw new ArgumentOutOfRangeException("field", "field must have CalculateField attribute");
            }
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var prefix = fullName.Substring(0, fullName.LastIndexOf('.'));
            var regExp=new Regex(@"\[[^\]]+]");
            var calculateArgMatches = regExp.Matches(calculateExpression.ToString());
            StringBuilder traceDomElement = new StringBuilder();
            foreach (Match match in calculateArgMatches)
            {
                //For Property like InstanceName.Property.SubProperty, change to InstanceName_Property_SubProperty
                //For Multiple calculateArgs split with |, eg:InstanceName_Property_SubProperty|InstanceName_Property_SubProperty2
                //This will be used for trace the dom element value change of each field in the calculateExpression
                var fieldName = match.Value.Replace("[", "").Replace("]", "");
                var fieldDomID=(prefix + "_" + fieldName).Replace(".", "_").Replace("[", "_").Replace("]", "_");
                traceDomElement.Append(fieldDomID+ "|");
                calculateExpression = calculateExpression.Replace(match.Value, fieldDomID);
            }
            var returnValue = helper.TextBoxFor(field, new { calculateArgs = traceDomElement.ToString(), calculateExpression = calculateExpression, calculateField = true });
            return returnValue;
            //throw new NotImplementedException();
        }
    }
}
