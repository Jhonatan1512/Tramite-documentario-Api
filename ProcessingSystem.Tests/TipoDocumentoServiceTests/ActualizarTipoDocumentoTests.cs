using Moq;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Tests.TipoDocumentoServiceTests
{
    public class ActualizarTipoDocumentoTests : TipoDocumentoServiceTestsBase
    {
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

            UsuarioContextServiceMock.Setup(con => con.ObtenerYValidarUsuarioAsync(usuarioId, "no puede modificar un tipo de documento"))
                .ThrowsAsync(new Exception("El usuario no existe o no esta autorizado para realizar esta acción"));

            var exeption = await Assert.ThrowsAsync<Exception>(() =>
                Service.ActuslizarTipoDocumentoAsync(documentoId, usuarioId, dto));

            Assert.Contains("no esta autorizado", exeption.Message);

            TipoDocumentoMock.Verify(td => td.GetTipodocumntoByIdAsync(It.IsAny<Guid>()), Times.Never());
            TipoDocumentoMock.Verify(td => td.ActualizarTipoDocumentoAsync(It.IsAny<TipoDocumento>()), Times.Never);
        }

        [Fact]
        public async Task TestActualizarTipoDocumentoAsync_documentoId_null()
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

            UsuarioContextServiceMock.Setup(c => c.ObtenerYValidarUsuarioAsync(usuarioId, "no puede modificar un tipo de documento"))
                .ReturnsAsync((new Usuarios(), usuarioModificacionId));

            TipoDocumentoMock.Setup(td => td.GetTipodocumntoByIdAsync(documentoId))
                .ReturnsAsync((TipoDocumento)null!);

            var exeption = await Assert.ThrowsAsync<Exception>(() =>
                Service.ActuslizarTipoDocumentoAsync(documentoId, usuarioId, dto));

            Assert.Equal("El registro no existe", exeption.Message);

            TipoDocumentoMock.Verify(td => td.ActualizarTipoDocumentoAsync(It.IsAny<TipoDocumento>()), Times.Never);
        }

        [Fact]
        public async Task TestActualizarTipoDocumentoAsync_documentoId_correct()
        {
            var usuarioId = Guid.NewGuid();
            var documentoId = Guid.NewGuid();
            var usuarioModificacionId = Guid.NewGuid().ToString();
            var fechaFicticia = DateTime.Now;
            var dto = new ActualizarTipoDocumentoDto
            {
                Nombre = "Tipo Doc - Actualizado",
                Descripcion = "Nueva descripción",
                FechaActualizacion = DateTime.Now,
            };

            var registroOriginal = new TipoDocumento
            {
                Id = documentoId,
            };

            UsuarioContextServiceMock.Setup(c => c.ObtenerYValidarUsuarioAsync(usuarioId, "no puede modificar un tipo de documento"))
                .ReturnsAsync((new Usuarios(), usuarioModificacionId));

            TipoDocumentoMock.Setup(td => td.GetTipodocumntoByIdAsync(documentoId))
                .ReturnsAsync(registroOriginal);

            await Service.ActuslizarTipoDocumentoAsync(documentoId, usuarioId, dto);

            Assert.Equal("Tipo Doc - Actualizado", registroOriginal.Nombre);
            Assert.Equal("Nueva descripción", registroOriginal.Descripcion);
            Assert.Equal(fechaFicticia, registroOriginal.FechaModificacion, TimeSpan.FromSeconds(1));
            Assert.Equal(usuarioModificacionId, registroOriginal.UsuarioModificacion);

            TipoDocumentoMock.Verify(r => r.ActualizarTipoDocumentoAsync(registroOriginal), Times.Once);
        }
    }
}
