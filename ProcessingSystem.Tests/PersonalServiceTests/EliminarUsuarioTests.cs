using Moq;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Tests.Services.PersonalServiceTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Tests.PersonalServiceTest
{
    public class EliminarUsuarioTests : PersonalServiceTestsBase
    {
        [Fact]
        public async Task EliminarUsuario_IdNoExiste_LanzaKeyNotFoundException()
        {
            PersonalRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Usuarios)null!);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                Service.EliminarUsuario(Guid.NewGuid(), Guid.NewGuid(), new DesactivarActivarPersonalDto()));
        }

        [Fact]
        public async Task EliminarUsuario_EstaEliminadoTrue_ModificaAuditoriaYRetornaMensajeDesactivado()
        {
            var personalId = Guid.NewGuid();
            var usuarioModificadorId = Guid.NewGuid();
            var personalExistente = new Usuarios { Id = personalId, EstaEliminado = false };
            var dto = new DesactivarActivarPersonalDto { EstaEliminado = true };

            PersonalRepositoryMock.Setup(r => r.GetByIdAsync(personalId)).ReturnsAsync(personalExistente);

            var mensaje = await Service.EliminarUsuario(personalId, usuarioModificadorId, dto);

            Assert.Equal("El usuario ha sido descativado", mensaje); 
            Assert.True(personalExistente.EstaEliminado);
            Assert.Equal(usuarioModificadorId.ToString(), personalExistente.UsuarioModificacion);
            PersonalRepositoryMock.Verify(r => r.ActualizarDatosPersonalAsync(personalExistente), Times.Once);
        }

        [Fact]
        public async Task EliminarUsuario_EstaEliminadoFalse_ModificaAuditoriaYRetornaMensajeActivado()
        {
            var personalId = Guid.NewGuid();
            var usuarioModificadorId = Guid.NewGuid();
            var personalExistente = new Usuarios { Id = personalId, EstaEliminado = true };
            var dto = new DesactivarActivarPersonalDto { EstaEliminado = false };

            PersonalRepositoryMock.Setup(r => r.GetByIdAsync(personalId)).ReturnsAsync(personalExistente);

            var mensaje = await Service.EliminarUsuario(personalId, usuarioModificadorId, dto);

            Assert.Equal("El usuario ha sido activado", mensaje);
            Assert.False(personalExistente.EstaEliminado);
            Assert.Equal(usuarioModificadorId.ToString(), personalExistente.UsuarioModificacion);
            PersonalRepositoryMock.Verify(r => r.ActualizarDatosPersonalAsync(personalExistente), Times.Once);
        }
    }
}
