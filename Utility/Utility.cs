using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Dow.SSD.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using Dow.SSD.Framework;

namespace xboxeer.HTMLExtension.Utility
{
    public static class Utility
    {
        private static ModelMetadataProvider metaDataProvider = new CachedDataAnnotationsModelMetadataProvider();

        /// <summary>
        /// Export List<string> as simple html list in string
        /// </summary>
        /// <param name="sourceStringList"></param>
        /// <returns></returns>
        public static string ListToHTMLListString(this List<string> sourceStringList)
        {
            var textWriter = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(textWriter);
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);
            sourceStringList.ForEach(item =>
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                writer.WriteLine(item);
                writer.RenderEndTag();
            });
            writer.RenderEndTag();
            return textWriter.ToString();
        }

        /// <summary>
        /// Export List<T> as simple html table in string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceList"></param>
        /// <returns></returns>
        public static string ListToHTMLTable<T>(this List<T> sourceList)
        {
            var textWriter = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(textWriter);
            #region Render table
            writer.AddAttribute(HtmlTextWriterAttribute.Border, "1");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            ModelMetadata metaData = new ModelMetadata(metaDataProvider, null, null, typeof(T), typeof(T).Name);
            var propertiesMetaData = metaData.Properties.Where(item => item.IsComplexType == false).ToList();
            var propertyDisplayNameMapping = propertiesMetaData.Select(item => new { DisplayName = item.DisplayName, PropertyName = item.PropertyName, Ignored = !item.ShowForDisplay, Order = item.Order }).OrderBy(item => item.Order);

            #region Render table title
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            foreach (var property in propertyDisplayNameMapping)
            {
                var customDisplayName = property.DisplayName;
                if (customDisplayName != null)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(customDisplayName);

                }
                else
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(property.PropertyName);
                }
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
            #endregion End Render Table title

            #region Render Table Content
            sourceList.ForEach(item =>
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                foreach (var propertyMetaData in propertyDisplayNameMapping)
                {
                    if (propertyMetaData.Ignored)
                    {
                        continue;
                    }
                    var property = typeof(T).GetProperty(propertyMetaData.PropertyName);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    var value = property.GetValue(item, null);
                    if (value != null)
                    {
                        writer.Write(value);
                    }
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();
            });
            #endregion End render table content
            writer.RenderEndTag();
            #endregion End Render table
            return textWriter.ToString();
        }

        /// <summary>
        /// Loop the property of the queryTemplate, if the property is not null and the property is not attributed as IgnoreInTemplateQueryAttribute, bind to expression, 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="queryTemplate"></param>
        /// <returns></returns>
        public static Expression<Func<TModel, bool>> BuildTemplateQueryExpression<TModel>(TModel queryTemplate)
        {
            var condition = Expression.Equal(Expression.Constant(1), Expression.Constant(1));
            var param = Expression.Parameter(typeof(TModel));
            if (queryTemplate != null)
            {
                var properties = queryTemplate.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var ignoreAttribute = property.GetCustomAttributes(typeof(IgnoreInTemplateQueryAttribute), true).FirstOrDefault() as IgnoreInTemplateQueryAttribute;
                    if (ignoreAttribute != null && ignoreAttribute.IngoreInTemplateQuery)
                    {
                        continue;
                    }
                    var templateValue = property.GetValue(queryTemplate, null);
                    if (templateValue != null &&
                        !string.IsNullOrEmpty(templateValue.ToString())
                        //For ID property, in case it is default to 0, skip such kind of case
                        && templateValue.ToString() != "0")
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            var propertyExpression = Expression.Property(param, property.Name);
                            var trimMethod = Expression.Call(propertyExpression, typeof(string).GetMethod("Trim", new Type[0]));
                            var toLowerMethod = Expression.Call(trimMethod, typeof(string).GetMethod("ToLower", new Type[0]));
                            var startWithParam = Expression.Constant(templateValue.ToString().ToLower().Trim());
                            var startWithMethod = Expression.Call(toLowerMethod, typeof(string).GetMethod("StartsWith", new Type[1] { typeof(string) }), startWithParam);
                            var newCondition = Expression.Equal(startWithMethod, Expression.Constant(true));
                            condition = Expression.AndAlso(condition, newCondition);
                        }
                        else
                        {
                            //For nullable type
                            var getValueOrDefaultMethod = property.PropertyType.GetMethod("GetValueOrDefault", new Type[0]);
                            if (getValueOrDefaultMethod == null)
                            {
                                var newCondition = Expression.Equal(Expression.Property(param, property.Name), Expression.Constant(templateValue));
                                condition = Expression.AndAlso(condition, newCondition);
                            }
                            else
                            {
                                var propertyExpression = Expression.Property(param, property.Name);
                                var callGetValueOrDefaultMethod = Expression.Call(propertyExpression, getValueOrDefaultMethod);
                                var newCondition = Expression.Equal(callGetValueOrDefaultMethod, Expression.Constant(templateValue));
                                condition = Expression.AndAlso(condition, newCondition);
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            var lambdaExpression = Expression.Lambda<System.Func<TModel, bool>>(condition, param);
            return lambdaExpression;
        }

        /// <summary>
        /// Read Excel file by stream
        /// </summary>
        /// <typeparam name="T">Entity of the EXCEL Sheet reference to</typeparam>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public static List<T> ReadExcelFromStream<T>(Stream inputStream)
            where T : new()
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream", "InputStream for source excel can not be null");
            }
            ModelMetadata metaData = new ModelMetadata(metaDataProvider, null, null, typeof(T), typeof(T).Name);
            var propertiesMetaData = metaData.Properties.Where(item => item.IsComplexType == false).ToList();
            var propertyDisplayNameMapping = propertiesMetaData.Select(item => new { DisplayName = item.DisplayName, PropertyName = item.PropertyName });
            var excelColumnIndexPropertyMapping = new Dictionary<int, string>();
            List<T> result = null;

            using (var document = SpreadsheetDocument.Open(inputStream, false))
            {
                var workbook = document.WorkbookPart;
                var sheet = workbook.WorksheetParts.FirstOrDefault();
                if (sheet != null)
                {
                    var rows = sheet.Worksheet.Descendants<Row>();
                    var headerRow = rows.FirstOrDefault();
                    if (headerRow != null)
                    {
                        result = new List<T>();
                        int count = 0;
                        var cells = headerRow.Descendants<Cell>();
                        foreach (var cell in cells)
                        {
                            var value = ExcelHelper.GetCellValue(workbook, cell);
                            if (!string.IsNullOrEmpty(value))
                            {
                                var mappedProperty = propertyDisplayNameMapping.Where(item =>
                                    item.DisplayName == null ? item.PropertyName.ToLower().Trim() == value.Trim().ToLower() : item.DisplayName.Trim().ToLower() == value.Trim().ToLower()).FirstOrDefault();
                                if (mappedProperty != null)
                                {
                                    excelColumnIndexPropertyMapping.Add(count, mappedProperty.PropertyName);
                                }
                            }
                            count++;
                        }
                    }
                    else
                    {
                        throw new ArgumentNullException("No Header row in the source excel file");
                    }
                    var contentRows = rows.Skip(1);

                    var modelType = typeof(T);
                    var properties = modelType.GetProperties();

                    foreach (var row in contentRows)
                    {
                        var newItem = new T();
                        var cells = row.Descendants<Cell>().ToList();
                        for (int i = 0; i < cells.Count(); i++)
                        {
                            var currentCell = cells[i];
                            var propertyName = string.Empty;
                            if (!excelColumnIndexPropertyMapping.TryGetValue(i, out propertyName))
                            {
                                continue;
                            }
                            var currentProperty = properties.Where(item => item.Name == propertyName).FirstOrDefault();
                            var currentPropertyType = currentProperty.PropertyType;
                            var isNullable = currentPropertyType.Name.Contains("Nullable");
                            var cellValue = ExcelHelper.GetCellValue(workbook, currentCell);
                            if (currentProperty != null)
                            {
                                if (isNullable)
                                {
                                    var nullableUnderLyingType = Nullable.GetUnderlyingType(currentPropertyType);
                                    if (nullableUnderLyingType == typeof(DateTime))
                                    {
                                        currentProperty.SetValue(newItem, DateTime.FromOADate(Convert.ToDouble(cellValue)), null);
                                    }
                                    else
                                    {
                                        var nullableUnderLyingValue = Convert.ChangeType(cellValue, nullableUnderLyingType);
                                        currentProperty.SetValue(newItem, nullableUnderLyingValue, null);
                                    }
                                }
                                else
                                {
                                    currentProperty.SetValue(newItem, Convert.ChangeType(cellValue, currentPropertyType), null);
                                }
                            }
                        }
                        result.Add(newItem);
                    }
                }
            }
            return result;
        }


        public static List<TChangeLogType> GetChanges<TEntity, TChangeLogType>(TEntity oldEntity, TEntity newEntiy)
            where TEntity : IModelCommon
            where TChangeLogType : class,IChangeLog, new()
        {
            var returnValue = new List<TChangeLogType>();
            ModelMetadata metaData = new ModelMetadata(metaDataProvider, null, null, typeof(TEntity), typeof(TEntity).Name);
            if (oldEntity == null)
            {
                var newAddChange = new TChangeLogType();
                newAddChange.ChangeType = ChangeType.Add.ToString();
                newAddChange.ChangedField = metaData.GetDisplayName();
                newAddChange.NewValue = newEntiy.ToString();
                returnValue.Add(newAddChange);
                return returnValue;
            }

            if (newEntiy == null)
            {
                var newDeleteChange = new TChangeLogType();
                newDeleteChange.ChangeType = ChangeType.Delete.ToString();
                newDeleteChange.ChangedField = metaData.GetDisplayName();
                newDeleteChange.OldValue = oldEntity.ToString();
                returnValue.Add(newDeleteChange);
                return returnValue;
            }

            
            var type = typeof(TEntity);
            var properties = metaData.Properties;
            foreach (var property in properties)
            {
                if (property.IsComplexType)
                {
                    continue;
                }
                object skip = false;
                property.AdditionalValues.TryGetValue("Skip",out skip);
                if(skip!=null&&(bool)skip)
                {
                    continue;
                }
                var propertyName = property.PropertyName;
                var propertyInfo = type.GetProperty(propertyName);
                var oldValue = propertyInfo.GetValue(oldEntity, null);
                var newValue = propertyInfo.GetValue(newEntiy, null);
                if (!object.Equals(oldValue,newValue))
                {
                    returnValue.Add(new TChangeLogType()
                    {
                        ChangedField = property.GetDisplayName(),
                        ChangeType = ChangeType.Modify.ToString(),
                        Modified = DateTime.Now,
                        ModifiedBy = RoleProviderConfig.GetUserIDProvider().GetCurrentUserID(null),
                        NewValue = newValue == null ? string.Empty : newValue.ToString(),
                        OldValue = oldValue == null ? string.Empty : oldValue.ToString()
                    });
                }
            }
            return returnValue;
        }
    }
}
