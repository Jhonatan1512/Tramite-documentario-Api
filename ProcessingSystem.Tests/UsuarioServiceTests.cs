using Mapster;
using Microsoft.AspNetCore.Identity;
using Moq;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Application.Services;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;
using Xunit;

namespace ProcessingSystem.Tests
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<ICredencialesCiudadanosService> _credencialesCiudadanoMock;
        private readonly Mock<IIdentityCiudadanoiService> _identityCiudadanoMock;
        private readonly Mock<UserManager<IdentityUser<Guid>>> _userManagerMock;
        private readonly UsuarioService _usuarioService;
        public UsuarioServiceTests()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _credencialesCiudadanoMock = new Mock<ICredencialesCiudadanosService>();
            _identityCiudadanoMock = new Mock<IIdentityCiudadanoiService>();

            var storeMock = new Mock<IUserStore<IdentityUser<Guid>>>();
            _userManagerMock = new Mock<UserManager<IdentityUser<Guid>>>(
                storeMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _usuarioService = new UsuarioService(
                _usuarioRepositoryMock.Object,
                _credencialesCiudadanoMock.Object,
                _identityCiudadanoMock.Object,
                _userManagerMock.Object
            );

        }

        //ActualizarContrasenaAsync
        [Fact]
        public async Task TestActualizarContraseniaCuandoUsuarioNull()
        {
            var idInexistente = Guid.NewGuid();
            var dto = new ActualizarContrasenaDto {  ContrasenaActual = "123", ContrasenaNueva = "1234" };

            _usuarioRepositoryMock.Setup(repo => repo.ObtenerPorId(idInexistente))
                .ReturnsAsync((Usuarios)null!);

            var exeption = await Assert.ThrowsAsync<KeyNotFoundException>(()=> 
                _usuarioService.ActualizarContrasenaAsync(idInexistente, dto));

            Assert.Equal("El usuario no existe", exeption.Message);

            _identityCiudadanoMock.Verify(identity =>
                identity.ActualizarContrasenaAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async Task TestActualizarContraseniaCuandoUsuarioExiste()
        {
            var idExiste = Guid.NewGuid();
            var usuarioExiste = new Usuarios { Id = idExiste };
            var dto = new ActualizarContrasenaDto { ContrasenaActual = "PasswordActual*1", ContrasenaNueva = "PasswordNueva*1" };

            _usuarioRepositoryMock.Setup(repo => repo.ObtenerPorId(idExiste))
                .ReturnsAsync(usuarioExiste);

            await _usuarioService.ActualizarContrasenaAsync(idExiste, dto);

            _identityCiudadanoMock.Verify(identity =>
                identity.ActualizarContrasenaAsync(idExiste, dto.ContrasenaActual, dto.ContrasenaNueva), Times.Once);
        }

        //ActualizarUsuarioAsync
        [Fact]
        public async Task TestActualizarCiudadanoUsuarioNull()
        {
            var idInexistente = Guid.NewGuid();
            var dtoUpdate = new ActualizarUsuarioDto { Nombre = "Diana", Apellidos = "Gonzales", Dni = "1234" };

            _usuarioRepositoryMock.Setup(repo => repo.ObtenerPorId(idInexistente))
                .ReturnsAsync((Usuarios)null!);

            var exeption = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _usuarioService.ActualizarUsuarioAsync(idInexistente, dtoUpdate));

            Assert.Equal("El usuario no existe", exeption.Message);

            _credencialesCiudadanoMock.Verify(s => s.VerificarDni(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _usuarioRepositoryMock.Verify(u => u.ActualizarUsuarioAsync(It.IsAny<Usuarios>()), Times.Never);
        }

        [Fact]
        public async Task TestActualizarCiudadanoUsuarioExiste()
        {
            var idExiste = Guid.NewGuid();
            var usuarioExiste = new Usuarios { Id = idExiste, Nombre = "Jhoselin", Apellidos = "Cano", Dni = "12345678" };
            var dto = new ActualizarUsuarioDto { Nombre = "Jhoselin Daneri", Apellidos = "Cano Vásquez", Dni = "12345678" };

            _usuarioRepositoryMock.Setup(repo => repo.ObtenerPorId(idExiste))
                .ReturnsAsync(usuarioExiste);

            await _usuarioService.ActualizarUsuarioAsync(idExiste, dto);

            _credencialesCiudadanoMock.Verify(service =>
                service.VerificarDni("12345678", "12345678"), Times.Once);

            _usuarioRepositoryMock.Verify(repo => repo.ActualizarUsuarioAsync(It.Is<Usuarios>(u => 
                u.Nombre == "Jhoselin Daneri" && u.Apellidos == "Cano Vásquez" &&
                u.UsuarioModificacion == idExiste.ToString() && 
                u.FechaModificacion > DateTime.Now.AddSeconds(-5))), 
                Times.Once);
        }

        //CrearUsuarioAsync
        [Fact]
        public async Task TestCrearUsuario_dni_duplicate()
        {
            var dto = new UsuariosDto { Email = "nuevo@gmail.com", Dni = "12345678", Password = "Passowrd1*" };

            _credencialesCiudadanoMock.Setup(s => s.ValidarRegistrosDuplicados(dto.Email, dto.Dni))
                .ThrowsAsync(new ArgumentException("El DNI ya esta registrado"));

            var exeption = await Assert.ThrowsAsync<ArgumentException>(() =>
                _usuarioService.CrearUsuarioAsync(dto));

            Assert.Equal("El DNI ya esta registrado", exeption.Message);

            _identityCiudadanoMock.Verify(c => c.CrearCuentaAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never); 
        }

        [Fact]
        public async Task TestCrearUsuario_Object_datos_correct()
        {
            var dtoOriginal = new UsuariosDto { Email = "ciudadano@correo.com", Dni = "99999999", Password = "PasswordSeguro123!" };

            var identityIdFicticio = Guid.NewGuid();

            _identityCiudadanoMock
                .Setup(identity => identity.CrearCuentaAsync(dtoOriginal.Email, dtoOriginal.Password))
                .ReturnsAsync(identityIdFicticio);

            GetUsuarioDto resultadoFinal = await _usuarioService.CrearUsuarioAsync(dtoOriginal);

            Assert.NotNull(resultadoFinal);

            Assert.Equal(dtoOriginal.Email, resultadoFinal.Email);
             
            _credencialesCiudadanoMock.Verify(service =>
                service.ValidarRegistrosDuplicados("ciudadano@correo.com", "99999999"),
                Times.Once);

            _identityCiudadanoMock.Verify(identity =>
                identity.AsignarRoleAsync(identityIdFicticio, "Ciudadano"),
                Times.Once);

            _usuarioRepositoryMock.Verify(repo =>
                repo.CrearUsuarioAsync(It.Is<Usuarios>(u => u.UserId == identityIdFicticio)),
                Times.Once);
        }
        //
        [Fact]
        public async Task TestGetProfileUsuario_usuario_null()
        {
            var idInexistente = Guid.NewGuid();

            _usuarioRepositoryMock.Setup(repo => repo.ObtenerPorId(idInexistente))
                .ReturnsAsync((Usuarios)null!);

            var exeption = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _usuarioService.ObtenerPerfilAsync(idInexistente));

            Assert.Equal("El usuario no existe", exeption.Message);
            _userManagerMock.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Never);            
        }

        [Fact]
        public async Task TestGetProfileUsuario_usuarioIdentity_null()
        {
            var idExiste = Guid.NewGuid();
            var usuarioDb = new Usuarios { Id = idExiste };

            _usuarioRepositoryMock.Setup(repo => repo.ObtenerPorId(idExiste))
                .ReturnsAsync(usuarioDb);

            _userManagerMock.Setup(m => m.FindByIdAsync(idExiste.ToString()))
                .ReturnsAsync((IdentityUser<Guid>)null!);

            var exeption = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _usuarioService.ObtenerPerfilAsync(idExiste));

            Assert.Equal("Usuario de seguridad no encontrado", exeption.Message);
        }

        [Fact]
        public async Task TestFetProfileUsuario_usuario_existe()
        {
            var id = Guid.NewGuid();
            var usuarioDb = new Usuarios { Id = id, Nombre = "Jhoselin", Apellidos = "Cano", Oficina = null };
            var userIdentity = new IdentityUser<Guid>() { Id = id, Email = "jhoselin@gmail.com"};

            _usuarioRepositoryMock.Setup(r => r.ObtenerPorId(id)).ReturnsAsync(usuarioDb);
            _userManagerMock.Setup(m => m.FindByIdAsync(id.ToString())).ReturnsAsync(userIdentity);

            GetPerfilDto result = await _usuarioService.ObtenerPerfilAsync(id);

            Assert.NotNull(result);
            Assert.Equal("jhoselin@gmail.com", result.Email);
            Assert.Equal("Ciudadano", result.Oficina);
        }
    }
}
