using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure
{
    public interface IQuery<T>
    {
        T FindByID(int ID);
        List<T> GetAll();
        List<T> Query(T queryTemplate, Expression<Func<T, bool>> additionalCondition);
        List<T> Query(int startIndex, int pageSize, ref int totalCount);
        List<T> Query(int startIndex, int pageSize, ref int totalCount, T QueryTemplate, Expression<Func<T, bool>> additionalCondition);
    }
}
