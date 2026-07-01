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
    public class CrearTipoDocumentoTests : TipoDocumentoServiceTestsBase
    {
        [Fact]
        public async Task Test_CrearTipoDocumentoAsync_usuarioCreacionId_null()
        {
            var usuarioId = Guid.NewGuid();
            var dto = new TipoDocumentoDto
            {
                Nombre = "New Tipe Document",
                Descripcion = "New description"
            };

            UsuarioContextServiceMock.Setup(con => con.ObtenerYValidarUsuarioAsync(usuarioId, "crear un tipo de documento"))
                .ThrowsAsync(new Exception("El usuario no existe o el no esta autorizado para realizar esta acción"));

            var exeption = await Assert.ThrowsAsync<Exception>(() =>
                Service.CrearTipoDocumentoAsync(usuarioId, dto));

            Assert.Contains("no esta autorizado", exeption.Message);

            TipoDocumentoMock.Verify(td => td.CrearTipoDocumentoAsync(It.IsAny<TipoDocumento>()), Times.Never);
        }

        [Fact]
        public async Task Test_CrearTipoDocumentoAsync_usuarioCreacionId_correct()
        {
            var usuarioId = Guid.NewGuid();
            var usuarioCreacionId = usuarioId.ToString();
            var dto = new TipoDocumentoDto
            {
                Nombre = "New Tipe Document",
                Descripcion = "New description"
            };

            UsuarioContextServiceMock.Setup(con => con.ObtenerYValidarUsuarioAsync(usuarioId, "crear un tipo de documento"))
                .ReturnsAsync((new Usuarios(), usuarioCreacionId));

            var dtoSimulation = new TipoDocumento
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre,
                UsuarioCreacion = usuarioCreacionId,
            };

            TipoDocumentoMock.Setup(td => td.CrearTipoDocumentoAsync(It.IsAny<TipoDocumento>()))
                .ReturnsAsync(dtoSimulation);

            var result = await Service.CrearTipoDocumentoAsync(usuarioId, dto);
            Assert.NotNull(result);
            Assert.Equal(dto.Nombre, result.Nombre);
            TipoDocumentoMock.Verify(td => td.CrearTipoDocumentoAsync(It.Is<TipoDocumento>(doc =>
                doc.UsuarioCreacion == usuarioCreacionId &&
                doc.Nombre == dto.Nombre &&
                doc.Descripcion == dto.Descripcion)), Times.Once);
        }
    }
}
