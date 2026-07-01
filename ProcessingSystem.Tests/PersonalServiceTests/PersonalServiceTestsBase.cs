using System;
using Microsoft.AspNetCore.Identity;
using Moq;
using ProcessingSystem.Application.Services;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Domain.Interfaces;
using ProcessingSystem.Application.Interfaces;

namespace ProcessingSystem.Tests.Services.PersonalServiceTests;

public abstract class PersonalServiceTestsBase
{
    protected readonly Mock<IPersonalRepository> PersonalRepositoryMock;
    protected readonly Mock<UserManager<IdentityUser<Guid>>> UserManagerMock;
    protected readonly Mock<RoleManager<Rol>> RoleManagerMock;
    protected readonly Mock<IOficinaRepository> OficinaRepositoryMock;
    protected readonly Mock<ICredencialesPersonalService> CredencialesPersonalMock;
    protected readonly PersonalService Service;

    protected PersonalServiceTestsBase()
    {
        PersonalRepositoryMock = new Mock<IPersonalRepository>();
        OficinaRepositoryMock = new Mock<IOficinaRepository>();
        CredencialesPersonalMock = new Mock<ICredencialesPersonalService>();

        var userStoreMock = new Mock<IUserStore<IdentityUser<Guid>>>();
        UserManagerMock = new Mock<UserManager<IdentityUser<Guid>>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!
        );

        var roleStoreMock = new Mock<IRoleStore<Rol>>();
        RoleManagerMock = new Mock<RoleManager<Rol>>(
            roleStoreMock.Object, null!, null!, null!, null!
        );

        Service = new PersonalService(
            PersonalRepositoryMock.Object,
            UserManagerMock.Object,
            RoleManagerMock.Object,
            OficinaRepositoryMock.Object,
            CredencialesPersonalMock.Object
        );
    }
}