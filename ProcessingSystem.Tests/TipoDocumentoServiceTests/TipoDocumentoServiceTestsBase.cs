using Microsoft.AspNetCore.Identity;
using Moq;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Application.Services;
using ProcessingSystem.Domain.Interfaces;

namespace ProcessingSystem.Tests.TipoDocumentoServiceTests
{
    public abstract class TipoDocumentoServiceTestsBase
    {
        protected readonly Mock<ITipoDocumentoRepository> TipoDocumentoMock;
        protected readonly Mock<IUsuarioRepository> UsuarioRepositoryMock;
        protected readonly Mock<IUsuarioContextService> UsuarioContextServiceMock;
        protected readonly Mock<UserManager<IdentityUser<Guid>>> UserManagerMock;
        protected readonly TipoDocumentoService Service;

        protected TipoDocumentoServiceTestsBase()
        {
            TipoDocumentoMock = new Mock<ITipoDocumentoRepository>();
            UsuarioRepositoryMock = new Mock<IUsuarioRepository>();
            UsuarioContextServiceMock = new Mock<IUsuarioContextService>();

            var storeMock = new Mock<IUserStore<IdentityUser<Guid>>>();
            UserManagerMock = new Mock<UserManager<IdentityUser<Guid>>>(
                storeMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            Service = new TipoDocumentoService(
                TipoDocumentoMock.Object,
                UsuarioRepositoryMock.Object,
                UserManagerMock.Object,
                UsuarioContextServiceMock.Object
            );
        }        
    }
}
