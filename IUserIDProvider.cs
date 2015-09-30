using System;
namespace Dow.SSD.Framework.Infrastructure
{
    public interface IUserIDProvider
    {
        string GetCurrentUserID(object context);
    }
}
