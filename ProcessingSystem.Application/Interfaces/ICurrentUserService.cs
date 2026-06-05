using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid GetUserId();
        Guid GetOficinaId();
    }
}
