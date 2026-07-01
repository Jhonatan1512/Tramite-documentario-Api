using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Tests.Services.PersonalServiceTests;
using Xunit;

namespace ProcessingSystem.Tests.PersonalServiceTest;

public class CrearPersonalTests : PersonalServiceTestsBase
{ 
    [Fact]
    public async Task CrearPersonalAsync_CaminoFeliz_RetornaGetPersonalDto()
    {
        var usuarioCreacionId = Guid.NewGuid();
        var oficinaId = Guid.NewGuid();
        var dto = new PersonalDto { Nombre = "Ana", Apellidos = "Silva", Dni = "12345678", OficinaId = oficinaId };

        OficinaRepositoryMock.Setup(r => r.GetByIdAsync(oficinaId)).ReturnsAsync(new Oficina());
        PersonalRepositoryMock.Setup(r => r.ObtenerPorDni(dto.Dni)).ReturnsAsync((Usuarios)null!);
        CredencialesPersonalMock.Setup(c => c.GenerarParaPersonal(dto.Apellidos, dto.Dni)).Returns(("ana@test.com", "Pass123!"));
        UserManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser<Guid>>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        RoleManagerMock.Setup(r => r.RoleExistsAsync("Personal")).ReturnsAsync(true);
        UserManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser<Guid>>(), "Personal")).ReturnsAsync(IdentityResult.Success);

        var result = await Service.CrearPersonalAsync(usuarioCreacionId, dto);

        Assert.NotNull(result);
        Assert.Equal("ana@test.com", result.Email);
        PersonalRepositoryMock.Verify(r => r.CrearPersonalAsync(It.IsAny<Usuarios>()), Times.Once);
    }

    [Fact]
    public async Task CrearPersonalAsync_OficinaNoExiste_LanzaKeyNotFoundException()
    {
        var dto = new PersonalDto { Dni = "12345678", OficinaId = Guid.NewGuid() };
        OficinaRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Oficina)null!);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            Service.CrearPersonalAsync(Guid.NewGuid(), dto));
        Assert.Equal("La oficina especificada no existe en el sistema", ex.Message);
    }

    [Fact]
    public async Task CrearPersonalAsync_DniDuplicado_LanzaInvalidOperationException()
    {
        var dto = new PersonalDto { Dni = "12345678" };
        PersonalRepositoryMock.Setup(r => r.ObtenerPorDni(dto.Dni)).ReturnsAsync(new Usuarios());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Service.CrearPersonalAsync(Guid.NewGuid(), dto));
        Assert.Equal("El DNI que esta intentando registra ya existe en la BD", ex.Message);
    }

    [Fact]
    public async Task CrearPersonalAsync_IdentityFalla_LanzaInvalidOperationExceptionYNoGuardaEnDb()
    {
        var dto = new PersonalDto { Dni = "12345678" };
        PersonalRepositoryMock.Setup(r => r.ObtenerPorDni(dto.Dni)).ReturnsAsync((Usuarios)null!);
        CredencialesPersonalMock.Setup(c => c.GenerarParaPersonal(It.IsAny<string>(), It.IsAny<string>())).Returns(("a@a.com", "1"));

        var identityError = IdentityResult.Failed(new IdentityError { Description = "Password Inseguro" });
        UserManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser<Guid>>(), It.IsAny<string>())).ReturnsAsync(identityError);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Service.CrearPersonalAsync(Guid.NewGuid(), dto));
        Assert.Contains("Password Inseguro", ex.Message);
        PersonalRepositoryMock.Verify(r => r.CrearPersonalAsync(It.IsAny<Usuarios>()), Times.Never);
    }

    [Fact]
    public async Task CrearPersonalAsync_FallaSincronizacionDeRol_EjecutaRollbackBorrandoUsuario()
    {
        var dto = new PersonalDto { Dni = "12345678" };
        PersonalRepositoryMock.Setup(r => r.ObtenerPorDni(dto.Dni)).ReturnsAsync((Usuarios)null!);
        CredencialesPersonalMock.Setup(c => c.GenerarParaPersonal(It.IsAny<string>(), It.IsAny<string>())).Returns(("a@a.com", "Pass123!"));
        UserManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser<Guid>>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

        RoleManagerMock.Setup(r => r.RoleExistsAsync("Personal")).ReturnsAsync(true);
        UserManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser<Guid>>(), "Personal"))
            .ThrowsAsync(new Exception("Error de servidor de Roles"));

        await Assert.ThrowsAsync<Exception>(() => Service.CrearPersonalAsync(Guid.NewGuid(), dto));

        UserManagerMock.Verify(u => u.DeleteAsync(It.IsAny<IdentityUser<Guid>>()), Times.Once);
    }
}