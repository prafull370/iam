using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLayer.Interfaces
{
    public interface ICacheService
    {
        string SetCache(int userId, string token, int expireMinutes = 10);
        string? GetCache(int userId);
    }

}
