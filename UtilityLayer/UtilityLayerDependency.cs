using Microsoft.Extensions.DependencyInjection;
using UtilityLayer.Interfaces;
using UtilityLayer.Services;
namespace DataAccess
{
    public static class UtilityLayerDependency
    {
        public static void AddUtilityLayerServices(this IServiceCollection services)
        {
            services.AddScoped<UtilityDependencyService>(); // Change from Singleton to Scoped
            services.AddScoped<IJWTService, JWTService>();
            services.AddTransient<ICryptographyService, CryptographyService>();
            services.AddScoped<IGenerateAvatarService, GenerateAvatarService>();
            services.AddScoped<ICacheService, CacheService>();
        }
    }
    public class UtilityDependencyService
    {
        public readonly IJWTService jwt;
        public readonly ICryptographyService cryptography;
        public readonly IGenerateAvatarService avatar;
        public readonly ICacheService cache;

        public UtilityDependencyService(
            IJWTService Jwt,
            ICryptographyService Cryptography,
			IGenerateAvatarService Avatar,
			ICacheService Cache

		)
        {
            jwt = Jwt;
            cryptography = Cryptography;
            avatar = Avatar;
            cache = Cache;
        }
    }
}
