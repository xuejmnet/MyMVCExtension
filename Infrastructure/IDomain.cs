using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure.Infrastructure
{
    public interface IDomain<TDomainRoot>
        where TDomainRoot:IModelCommon
    {
        IDomain<TDomainRoot> FindByID(int id);
        List<IDomain<TDomainRoot>> GetAll();
        List<IDomain<TDomainRoot>> Query(TDomainRoot queryTemplate, Expression<Func<TDomainRoot, bool>> additionalCondition);
        List<IDomain<TDomainRoot>> Query(int startIndex, int pageSize, ref int totalCount);
        List<IDomain<TDomainRoot>> Query(int startIndex, int pageSize, ref int totalCount, TDomainRoot QueryTemplate, Expression<Func<TDomainRoot, bool>> additionalCondition);

        bool Add(IDomain<TDomainRoot> entity);
        bool Add(List<IDomain<TDomainRoot>> entities);
        bool Update(IDomain<TDomainRoot> entity);
        bool Delete(IDomain<TDomainRoot> entity);
        bool Delete(List<IDomain<TDomainRoot>> entities);
    }
}
