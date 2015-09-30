using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure
{
    public interface IRepository<T>:IQuery<T>
    {
        bool Add(T entity);
        bool Add(List<T> entities);
        bool Update(T entity);
        bool Delete(T entity);
        bool Delete(List<T> entities);
        bool Delete(int id);
        bool Delete(List<int> ids);
    }
}
