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
    public class ActualizarPersonalTests : PersonalServiceTestsBase
    {
        [Fact]
        public async Task ActualizarPersonalAsync_PersonalNoExiste_LanzaKeyNotFoundException()
        {
            PersonalRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Usuarios)null!);

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                Service.ActualizarPersonalAsync(Guid.NewGuid(), Guid.NewGuid(), new ActualizarPersonalDto()));
            Assert.Equal("El ID del usuario no existe", ex.Message);
        }

        [Fact]
        public async Task ActualizarPersonalAsync_CambiaDniYEsValido_ActualizaTodoYCambiaCredenciales()
        {
            var personalId = Guid.NewGuid();
            var usuarioModificadorId = Guid.NewGuid();
            var personalExistente = new Usuarios { Id = personalId, Dni = "11111111", Apellidos = "Gomez", UserId = Guid.NewGuid() };
            var dto = new ActualizarPersonalDto { Dni = "22222222", Apellidos = "Gomez Silva", OficinaId = Guid.NewGuid() };

            PersonalRepositoryMock.Setup(r => r.GetByIdAsync(personalId)).ReturnsAsync(personalExistente);
            OficinaRepositoryMock.Setup(r => r.GetByIdAsync(dto.OficinaId.Value)).ReturnsAsync(new Oficina());
            PersonalRepositoryMock.Setup(r => r.ObtenerPorDni(dto.Dni)).ReturnsAsync((Usuarios)null!);

            await Service.ActualizarPersonalAsync(personalId, usuarioModificadorId, dto);

            CredencialesPersonalMock.Verify(c => c.ActualizarCredenciales(personalExistente.UserId!.Value, dto.Apellidos, dto.Dni), Times.Once);
            PersonalRepositoryMock.Verify(r => r.ActualizarDatosPersonalAsync(It.Is<Usuarios>(u =>
                u.UsuarioModificacion == usuarioModificadorId.ToString() && u.Dni == "22222222"
            )), Times.Once);
        }

        [Fact]
        public async Task ActualizarPersonalAsync_MismoDniPeroCambiaOficina_SoloValidaOficinaSinValidarDni()
        {
            var personalId = Guid.NewGuid();
            var personalExistente = new Usuarios { Id = personalId, Dni = "11111111", OficinaId = Guid.NewGuid() };
            var dto = new ActualizarPersonalDto { Dni = "11111111", OficinaId = Guid.NewGuid() }; 

            PersonalRepositoryMock.Setup(r => r.GetByIdAsync(personalId)).ReturnsAsync(personalExistente);
            OficinaRepositoryMock.Setup(r => r.GetByIdAsync(dto.OficinaId.Value)).ReturnsAsync(new Oficina());

            PersonalRepositoryMock.Setup(r => r.ObtenerPorDni(null!)).ReturnsAsync((Usuarios)null!);

            await Service.ActualizarPersonalAsync(personalId, Guid.NewGuid(), dto);

            PersonalRepositoryMock.Verify(r => r.ObtenerPorDni(null!), Times.Once);

            PersonalRepositoryMock.Verify(r => r.ActualizarDatosPersonalAsync(personalExistente), Times.Once);
        }
    }
}
