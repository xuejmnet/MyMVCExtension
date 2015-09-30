using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure
{
    public static class QueryableExtension
    {
        public static IOrderedQueryable<T> ObjectSort<T>(this IQueryable<T> source, Expression<Func<T, object>> sortKeySelector)
        {
            var convertToObjectMethodExression = sortKeySelector.Body as UnaryExpression;
            if (convertToObjectMethodExression != null)
            {
                var memberAccessor = convertToObjectMethodExression.Operand as MemberExpression;
                if(memberAccessor==null)
                {
                    throw new InvalidOperationException("The arguments for convert method must be a member accessor expression");
                }
                var memberAccessorInstance=sortKeySelector.Parameters;
                if (memberAccessor.Type == typeof(string))
                {
                    var newExpression = Expression.Lambda<Func<T, string>>(memberAccessor, memberAccessorInstance);
                    return source.OrderBy(newExpression);
                }
                if (memberAccessor.Type == typeof(float))
                {
                    var newExpression = Expression.Lambda<Func<T, float>>(memberAccessor, memberAccessorInstance);
                    return source.OrderBy(newExpression);
                }
                if (memberAccessor.Type == typeof(float?))
                {
                    var newExpression = Expression.Lambda<Func<T, float?>>(memberAccessor, memberAccessorInstance);
                    return source.OrderBy(newExpression);
                }
                if (memberAccessor.Type == typeof(double))
                {
                    var newExpression = Expression.Lambda<Func<T, double>>(memberAccessor, memberAccessorInstance);
                    return source.OrderBy(newExpression);
                }
                if (memberAccessor.Type == typeof(double?))
                {
                    var newExpression = Expression.Lambda<Func<T, double?>>(memberAccessor, memberAccessorInstance);
                    return source.OrderBy(newExpression);
                }
                if (memberAccessor.Type == typeof(long))
                {
                    var newExpression = Expression.Lambda<Func<T, long>>(memberAccessor, memberAccessorInstance);
                    return source.OrderBy(newExpression);
                }
                if (memberAccessor.Type == typeof(long?))
                {
                    var newExpression = Expression.Lambda<Func<T, long?>>(memberAccessor, memberAccessorInstance);
                    return source.OrderBy(newExpression);
                }
                if(memberAccessor.Type==typeof(int))
                {
                    var newExpression= Expression.Lambda<Func<T, int>>(memberAccessor, memberAccessorInstance);
                    return source.OrderBy(newExpression);
                }
                if (memberAccessor.Type == typeof(int?))
                {
                    var newExpression = Expression.Lambda<Func<T, int?>>(memberAccessor, memberAccessorInstance);
                    return source.OrderBy(newExpression);
                }
                if (memberAccessor.Type == typeof(DateTime))
                {
                    var newExpression = Expression.Lambda<Func<T, DateTime>>(memberAccessor, memberAccessorInstance);
                    return source.OrderBy(newExpression);
                }
                if (memberAccessor.Type == typeof(DateTime?))
                {
                    var newExpression = Expression.Lambda<Func<T, DateTime?>>(memberAccessor, memberAccessorInstance);
                    return source.OrderBy(newExpression);
                }
                throw new NotSupportedException(string.Format("Sort for type {0} is not supported", memberAccessor.Type.Name));
            }
            return null;
        }
    }
}
