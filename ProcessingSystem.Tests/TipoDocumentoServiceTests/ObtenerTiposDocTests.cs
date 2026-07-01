using Moq;
using ProcessingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Tests.TipoDocumentoServiceTests
{
    public class ObtenerTiposDocTests : TipoDocumentoServiceTestsBase
    {
        [Fact]
        public async Task Test_ObtenerTodosAsync_userCreacion_null()
        {
            var mensajeErrorEsperado = "El usuario no existe o no esta autorizado para realizar esta acción";
            var listaConUnElemento = new List<TipoDocumento>
            {
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nombre = "DNI",
                    UsuarioCreacion = Guid.NewGuid().ToString()
                }
            };

            TipoDocumentoMock.Setup(td => td.GetAllTiposDocumentoAsync())
                .ReturnsAsync(listaConUnElemento);

            UsuarioContextServiceMock.Setup(con => con.ObtenerYValidarUsuarioAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception(mensajeErrorEsperado));

            var exception = await Assert.ThrowsAsync<Exception>(() =>
                Service.ObtenerTodosAsync());

            Assert.Equal(mensajeErrorEsperado, exception.Message);
        }

        [Fact]
        public async Task Test_ObtenerTodosAsync_userCreacion_correct()
        {
            var usuarioId = Guid.NewGuid();
            var usuarioAuditId = usuarioId.ToString();
            var listaConUnElemento = new List<TipoDocumento>
            {
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nombre = "DNI",
                    UsuarioCreacion = usuarioId.ToString()
                }
            };

            TipoDocumentoMock
                .Setup(td => td.GetAllTiposDocumentoAsync())
                .ReturnsAsync(listaConUnElemento);

            UsuarioContextServiceMock
                .Setup(con => con.ObtenerYValidarUsuarioAsync(usuarioId, "ver la lista"))
                .ReturnsAsync((new Usuarios(), usuarioAuditId));

            var result = await Service.ObtenerTodosAsync();

            Assert.NotNull(result);
            var listaResult = result.ToList();
            Assert.Single(listaResult);

            var documento = listaResult.First();
            Assert.Equal("Administrador del Distema", documento.NombreUsuarioCreacion);
        }
    }
}
