using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure
{
    public interface ISortableQuery<T>:IQuery<T>
    {
        List<T> GetAll(Expression<Func<T, object>> sortKeySelector);
        List<T> Query(T queryTemplate, Expression<Func<T, bool>> additionalCondition, Expression<Func<T, dynamic>> sortKeySelector);
        List<T> Query(int startIndex, int pageSize, ref int totalCount, Expression<Func<T, dynamic>> sortKeySelector);
        List<T> Query(int startIndex, int pageSize, ref int totalCount, T QueryTemplate, Expression<Func<T, bool>> additionalCondition, Expression<Func<T, dynamic>> sortKeySelector);
    }
}
