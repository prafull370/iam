using Microsoft.Extensions.Configuration;
using NPOI.SS.Formula.Functions;
using UtilityLayer.Model;

namespace UtilityLayer.Interfaces
{
    public interface IJWTService
    {
        string GenerateToken(ClaimsModel claimsmodel,int ExpireTimeInMinutes);
    }
}
