using Microsoft.Extensions.Caching.Memory;
using ProcessingSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Services
{
    public class MemoryCacheTokenBlacklistService : ITokenBlacklistService
    {
        private readonly IMemoryCache _memoryCache;
        private const string CachePrefix = "blacklist";

        public MemoryCacheTokenBlacklistService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public Task<bool> EsTokenInvalidoAsync(string token)
        {
            if(string.IsNullOrEmpty(token)) return Task.FromResult(true);

            var existe = _memoryCache.TryGetValue($"{CachePrefix}{token}", out _);
            return Task.FromResult(existe);
        }

        public Task InvalidTokenAsync(string token)
        {
            if(string.IsNullOrEmpty(token)) return Task.CompletedTask;

            if(token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = token.Substring(7).Trim();

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var expiration = jwtToken.ValidTo;
                var tiempoRestante = expiration - DateTime.UtcNow;

                if(tiempoRestante.TotalSeconds > 0)
                {
                    _memoryCache.Set($"{CachePrefix}{token}", true, tiempoRestante);
                }
            } catch
            {

            }

            return Task.CompletedTask;
        }
    }
}
