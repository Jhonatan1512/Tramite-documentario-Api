using Microsoft.AspNetCore.Identity;
using Moq;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Application.Services;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Tests
{
    public class UserContextServiceTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<UserManager<IdentityUser<Guid>>> _userManagerMock;
        private readonly UsuarioContexService _userContexService;
        public UserContextServiceTests()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();

            var storeMock = new Mock<IUserStore<IdentityUser<Guid>>>();
            _userManagerMock = new Mock<UserManager<IdentityUser<Guid>>>(
                storeMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _userContexService = new UsuarioContexService(
                _usuarioRepositoryMock.Object,
                _userManagerMock.Object
            );
        }

        //ObtenerYValidarUsuarioAsync()
        [Fact]
        public async Task Test_ObtenerYValidarUsuarioAsync_CuandoUsuarioExisteYEstaActivo_RetornaUsuarioDeNegocio()
        {
            var usuarioId = Guid.NewGuid();
            var usuarioDb = new Usuarios { Id = usuarioId, EstaEliminado = false, Nombre = "karla" };
            var accion = "crear un trámite";

            _usuarioRepositoryMock
                .Setup(r => r.ObtenerPorId(usuarioId))
                .ReturnsAsync(usuarioDb);

            var (usuarioResult, auditUserId) = await _userContexService.ObtenerYValidarUsuarioAsync(usuarioId, accion);

            Assert.NotNull(usuarioResult);
            Assert.Equal("karla", usuarioResult.Nombre);
            Assert.Equal(usuarioId.ToString(), auditUserId);

            _userManagerMock.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact] 
        public async Task Test_ObtenerYValidarUsuarioAsync_User_Eliminado()
        {
            var usuarioId = Guid.NewGuid();
            var usuarioDb = new Usuarios { Id = usuarioId, EstaEliminado = true };
            var accion = "editar documento";

            _usuarioRepositoryMock.Setup(repo => repo.ObtenerPorId(usuarioId))
                .ReturnsAsync(usuarioDb);

            var exeption = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _userContexService.ObtenerYValidarUsuarioAsync(usuarioId, accion));

            Assert.Equal($"El usuario se encuentra inactivo. No puede {accion}.", exeption.Message);
        }

        [Fact]
        public async Task Test_ObtenerYValidarUsuarioAsync_UsuarioNegocio_Null_UsuarioIdentity_Existe()
        {
            var usuarioId = Guid.NewGuid() ;
            var usuarioIdentity = new IdentityUser<Guid> { Id = usuarioId, Email = "admin@ejemplo.gob.pe" };
            var accion = "derivar expediente";

            _userManagerMock.Setup(m => m.FindByIdAsync(usuarioId.ToString())).ReturnsAsync(usuarioIdentity);

            _usuarioRepositoryMock.SetupSequence(repo => repo.ObtenerPorId(usuarioId))
                .ReturnsAsync((Usuarios)null!)
                .ReturnsAsync((Usuarios)null!);

            var (usuarioResult, auditUserId) = await _userContexService.ObtenerYValidarUsuarioAsync(usuarioId, accion);

            Assert.NotNull(usuarioResult);
            Assert.Equal(usuarioId, usuarioResult.Id);
            Assert.Equal(usuarioId.ToString(), auditUserId);
        }

        [Fact]
        public async Task TestObtenerYvalidarUsuarioAsync_UsuarioNegocio_null_UsuarioIdentity_null()
        {
            var usuarioId = Guid.NewGuid();
            var accion = "eliminar expediente";

            _usuarioRepositoryMock.Setup(repo => repo.ObtenerPorId(usuarioId)).ReturnsAsync((Usuarios)null!);
            _userManagerMock.Setup(m => m.FindByIdAsync(usuarioId.ToString())).ReturnsAsync((IdentityUser<Guid>)null!);

            var exeption = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _userContexService.ObtenerYValidarUsuarioAsync(usuarioId, accion));

            Assert.Equal($"El usuario no existe o no esta autorizado para {accion}", exeption.Message);
        }

        [Fact]
        public async Task TestObtenerYvalidarUsuarioAsync_UsuarioNegocio_Eliminado_UsuarioIdentity_existe()
        {
            var usuarioId = Guid.NewGuid();
            var usuarioIdentity = new IdentityUser<Guid> { Id = usuarioId };
            var usuarioNegocioInactivo = new Usuarios { Id = usuarioId, EstaEliminado = false }; 
            var accion = "descargar reportes";

            _usuarioRepositoryMock.SetupSequence(r => r.ObtenerPorId(It.IsAny<Guid>()))
                .ReturnsAsync((Usuarios)null!) 
                .ReturnsAsync(usuarioNegocioInactivo); 

            _userManagerMock.Setup(m => m.FindByIdAsync(usuarioId.ToString())).ReturnsAsync(usuarioIdentity);

            var excepcion = await Assert.ThrowsAsync<Exception>(() =>
                _userContexService.ObtenerYValidarUsuarioAsync(usuarioId, accion)
            );

            Assert.Equal($"El perfil de usuario existe pero no se encuentra activo para {accion}.", excepcion.Message);
        }
    }
}
