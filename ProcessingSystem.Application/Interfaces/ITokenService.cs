using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerarTokenAsync(IdentityUser<Guid> user, IList<string> roles, string displayName);
    }
}
