using Microsoft.AspNetCore.Identity;
using Moq;
using ProcessingSystem.Application.DTOs;
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
    public class TipoDocumentoServiceTests
    {
        private readonly Mock<ITipoDocumentoRepository> _tipoDocumentoMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IUsuarioContextService> _usuarioContextServiceMock;
        private readonly Mock<UserManager<IdentityUser<Guid>>> _userManagerMock;
        private readonly TipoDocumentoService _tipoDocumentoService;

        public TipoDocumentoServiceTests()
        {
            _tipoDocumentoMock = new Mock<ITipoDocumentoRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _usuarioContextServiceMock = new Mock<IUsuarioContextService>();

            var storeMock = new Mock<IUserStore<IdentityUser<Guid>>>();
            _userManagerMock = new Mock<UserManager<IdentityUser<Guid>>>(
                storeMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _tipoDocumentoService = new TipoDocumentoService(
                _tipoDocumentoMock.Object,
                _usuarioRepositoryMock.Object,
                _userManagerMock.Object, 
                _usuarioContextServiceMock.Object
            );

            
        }
        //ActuslizarTipoDocumentoAsync
        [Fact]
        public async Task TestActualizarTipoDocumento_usuarioModificacion_null()
        {
            var usuarioId = Guid.NewGuid();
            var documentoId = Guid.NewGuid();
            var dto = new ActualizarTipoDocumentoDto
            {
                Nombre = "Tipo Doc - Actualizado",
                Descripcion = "Nueva descripción",
                FechaActualizacion = DateTime.Now,
            };

            _usuarioContextServiceMock.Setup(con => con.ObtenerYValidarUsuarioAsync(usuarioId, "no puede modificar un tipo de documento"))
                .ThrowsAsync(new Exception("El usuario no existe o no esta autorizado para realizar esta acción"));

            var exeption = await Assert.ThrowsAsync<Exception>(() => 
                _tipoDocumentoService.ActuslizarTipoDocumentoAsync(documentoId, usuarioId, dto));

            Assert.Contains("no esta autorizado", exeption.Message);

            _tipoDocumentoMock.Verify(td => td.GetTipodocumntoByIdAsync(It.IsAny<Guid>()), Times.Never());
            _tipoDocumentoMock.Verify(td => td.ActualizarTipoDocumentoAsync(It.IsAny<TipoDocumento>()), Times.Never);
        }

        [Fact]
        public async Task TestActuslizarTipoDocumentoAsync_documento_null()
        {
            var documentoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            var usuarioModificacionId = Guid.NewGuid().ToString();
            var dto = new ActualizarTipoDocumentoDto
            {
                Nombre = "Tipo Doc - Actualizado",
                Descripcion = "Nueva descripción",
                FechaActualizacion = DateTime.Now,
            };

            _usuarioContextServiceMock.Setup(c => c.ObtenerYValidarUsuarioAsync(usuarioId, "no puede modificar un tipo de documento"))
                .ReturnsAsync((new Usuarios(), usuarioModificacionId));

            _tipoDocumentoMock.Setup(td => td.GetTipodocumntoByIdAsync(documentoId))
                .ReturnsAsync((TipoDocumento)null!);

            var exeption = await Assert.ThrowsAsync<Exception>(() =>
                _tipoDocumentoService.ActuslizarTipoDocumentoAsync(documentoId, usuarioId, dto));

            Assert.Equal("El registro no existe", exeption.Message);

            _tipoDocumentoMock.Verify(td => td.ActualizarTipoDocumentoAsync(It.IsAny<TipoDocumento>()), Times.Never);
        }



        //public async Task ActuslizarTipoDocumentoAsync(Guid id, Guid usuarioId, ActualizarTipoDocumentoDto dto)
        //{
        //    var (_, usuarioModificacionId) = await _usuarioContext.ObtenerYValidarUsuarioAsync(usuarioId, "no puede modificar un tipo de documento");

        //    var registroExiste = await _tipoDocumentoRepository.GetTipodocumntoByIdAsync(id);
        //    if (registroExiste == null)
        //    {
        //        throw new Exception("El registro no existe");
        //    }

        //    registroExiste.Nombre = dto.Nombre;
        //    registroExiste.Descripcion = dto.Descripcion;
        //    registroExiste.FechaModificacion = dto.FechaActualizacion;
        //    registroExiste.UsuarioModificacion = usuarioModificacionId;

        //    await _tipoDocumentoRepository.ActualizarTipoDocumentoAsync(registroExiste);
        //}
    }
}
