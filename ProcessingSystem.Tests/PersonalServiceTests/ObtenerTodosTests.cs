using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using ProcessingSystem.Domain.Entities;
using ProcessingSystem.Tests.Services.PersonalServiceTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Tests.PersonalServiceTest
{
    public class ObtenerTodosTests : PersonalServiceTestsBase
    {
        [Fact]
        public async Task ObtenerTodosAsync_RepositorioVacio_RetornaColeccionVacia()
        {
            PersonalRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Usuarios>());

            var result = await Service.ObtenerTodosAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task BaseDeDatosTieneRegistros_PeroNingunoTieneElRolPersonal_RetornaColeccionVacia()
        {
            var listaDb = new List<Usuarios> { new Usuarios { Dni = "123", UserId = Guid.NewGuid() } };
            PersonalRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(listaDb);

            UserManagerMock.Setup(u => u.GetUsersInRoleAsync("Personal")).ReturnsAsync(new List<IdentityUser<Guid>>());

            var result = await Service.ObtenerTodosAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task ObtenerTodosAsync_CaminoFeliz_CruzaInformacionDeIdentityYOficinas()
        {
            var userId = Guid.NewGuid();
            var oficinaId = Guid.NewGuid();

            var personalDb = new List<Usuarios>
            {
                new Usuarios { Dni = "99999999", UserId = userId, OficinaId = oficinaId }
            };
            PersonalRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(personalDb);

            var usuariosRol = new List<IdentityUser<Guid>> { new IdentityUser<Guid> { Id = userId } };
            UserManagerMock.Setup(u => u.GetUsersInRoleAsync("Personal")).ReturnsAsync(usuariosRol);

            var listaUsuariosQueryable = new List<IdentityUser<Guid>>
            {
                new IdentityUser<Guid> { Id = userId, Email = "exito@sistema.com" }
            }.AsQueryable();

            var mockDbSet = new Mock<IQueryable<IdentityUser<Guid>>>();
            mockDbSet.As<IAsyncEnumerable<IdentityUser<Guid>>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<IdentityUser<Guid>>(listaUsuariosQueryable.GetEnumerator()));

            mockDbSet.Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<IdentityUser<Guid>>(listaUsuariosQueryable.Provider));
            mockDbSet.Setup(m => m.Expression).Returns(listaUsuariosQueryable.Expression);
            mockDbSet.Setup(m => m.ElementType).Returns(listaUsuariosQueryable.ElementType);
            mockDbSet.Setup(m => m.GetEnumerator()).Returns(listaUsuariosQueryable.GetEnumerator());

            UserManagerMock.Setup(u => u.Users).Returns(mockDbSet.Object);

            OficinaRepositoryMock.Setup(o => o.GetAllAsync()).ReturnsAsync(new List<Oficina>
            {
                new Oficina { Id = oficinaId, Nombre = "Gerencia TI" }
            });
            var result = await Service.ObtenerTodosAsync();

            var list = result.ToList();
            Assert.Single(list);
            Assert.Equal("exito@sistema.com", list.First().Email);
            Assert.Equal("Gerencia TI", list.First().NombreOficina);
        }

        // INFRAESTRUCTURA DE SIMULACIÓN EF CORE ASYNC (HELPERS CORREGIDOS) 

        private class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;
            internal TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

            public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);
            public IQueryable<TResult> CreateQuery<TResult>(Expression expression) => new TestAsyncEnumerable<TResult>(expression);

            public object Execute(Expression expression) => _inner.Execute(expression)!;
            public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default) => _inner.Execute<TResult>(expression);
        }

        private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public TestAsyncEnumerable(Expression expression) : base(expression) { }
            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
            public T Current => _inner.Current;
            public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
        }
    }
}
