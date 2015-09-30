using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dow.SSD.Framework.Infrastructure
{
    public interface IChangeLog
    {
        string ModifiedBy { get; set; }
        Nullable<System.DateTime> Modified { get; set; }
        string ChangedField { get; set; }
        string OldValue { get; set; }
        string NewValue { get; set; }
        string ChangeType { get; set; }
    }
}
