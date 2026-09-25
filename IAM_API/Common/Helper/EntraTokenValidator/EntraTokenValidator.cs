using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace IAM_API.Common.Helper.EntraTokenValidator
{

    //Validates a Microsoft Entra ID id_token against the tenant's OIDC metadata: issuer + signing keys from {authority}/.well-known/openid-configuration.
    public class EntraTokenValidator
    {
        private static readonly Dictionary<string, ConfigurationManager<OpenIdConnectConfiguration>> _cfgCache = new();
        private static readonly object _lock = new();  // _lock thread synchronizes access to _cfgCache
        public class EntraIdentity
        {
            public string ObjectId { get; set; } = "";   // unique object id of the user in the tenant, from the "oid" claim
            public string? TenantId { get; set; }
            public string? Upn { get; set; }    // "User Principal Name" used for login, from the "preferred_username" claim
            public string? Email { get; set; }
            public string? DisplayName { get; set; }
        }

        public async Task<EntraIdentity?> ValidateAsync(string idToken, string authority, string clientId)
        {
            var oidcConfigManager = GetConfigManager(authority);
            var oidcConfig = await oidcConfigManager.GetConfigurationAsync(default);

            var parameters = new TokenValidationParameters
            {
                ValidIssuer = oidcConfig.Issuer,
                ValidAudience = clientId,
                IssuerSigningKeys = oidcConfig.SigningKeys,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(idToken, parameters, out _);   // throws if invalid
            string? C(string t) => principal.FindFirst(t)?.Value;

            var id = new EntraIdentity
            {
                ObjectId = C("oid") ?? C("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? "",
                TenantId = C("tid") ?? C("http://schemas.microsoft.com/identity/claims/tenantid"),
                Upn = C("preferred_username") ?? C("upn"),
                Email = C("email") ?? C("preferred_username"),
                DisplayName = C("name")
            };
            return string.IsNullOrEmpty(id.ObjectId) ? null : id;
        }

        private static ConfigurationManager<OpenIdConnectConfiguration> GetConfigManager(string authority)
        {
            var key = authority.TrimEnd('/');

            // thread synchronization
            lock (_lock)
            {
                if (!_cfgCache.TryGetValue(key, out var manager))
                {
                    manager = new ConfigurationManager<OpenIdConnectConfiguration>(key + "/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
                    _cfgCache[key] = manager;
                }
                return manager;
            }
        }

    }
}
