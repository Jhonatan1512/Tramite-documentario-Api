using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface ITokenBlacklistService
    {
        Task InvalidTokenAsync(string token);
        Task<bool> EsTokenInvalidoAsync(string token); 
    }
}
