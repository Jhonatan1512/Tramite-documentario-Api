using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using ProcessingSystem.Application.Interfaces;
using ProcessingSystem.Application.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingSystem.Tests
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly TokenService _tokenService;
        public TokenServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _tokenService = new TokenService(
                _configurationMock.Object);
        }

        [Fact]
        public void GenerarTokenAsync_DatosValidos_RetornarJSONToken_Claims_Configuration()
        {
            var userId = Guid.NewGuid();    
            var user = new IdentityUser<Guid> { Id = userId, Email = "jhonatan@ejemplo.gob.pe" };
            var roles = new List<string> {"Ciudadano", "Personal" };
            var displayName = "Jhonatan Cruzado";
            var oficinaId = "OF-01";
            var nameOfice = "Dirección General";

            _configurationMock.Setup(c => c["Jwt:key"]).Returns("esta_es_una_llave_secreta_super_larga_y_segura_de_32_bytes!");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("MiWebApi");
            _configurationMock.Setup(c => c["Jwt:Audiences"]).Returns("AngularFrontendApp");

            string tokenGenerado = _tokenService.GenerarTokenAsync(user, roles, displayName, oficinaId, nameOfice);
            Assert.False(string.IsNullOrEmpty(tokenGenerado));

            var tokenHandle = new JwtSecurityTokenHandler();
            var jwtDecodificado = tokenHandle.ReadJwtToken(tokenGenerado);

            Assert.Equal("MiWebApi", jwtDecodificado.Issuer);
            Assert.Contains("AngularFrontendApp", jwtDecodificado.Audiences);

            Assert.Equal(userId.ToString(), jwtDecodificado.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
            Assert.Equal("jhonatan@ejemplo.gob.pe", jwtDecodificado.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
            Assert.Equal("Jhonatan Cruzado", jwtDecodificado.Claims.First(c => c.Type == "nombre").Value);
            Assert.Equal(oficinaId, jwtDecodificado.Claims.First(c => c.Type == "oficinaId").Value);
            Assert.Equal(nameOfice, jwtDecodificado.Claims.First(c => c.Type == "nombreOficina").Value);

            var claimsDeRoles = jwtDecodificado.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            Assert.Equal(2, claimsDeRoles.Count);
            Assert.Contains("Ciudadano", claimsDeRoles);
            Assert.Contains("Personal", claimsDeRoles);

            Assert.True(jwtDecodificado.ValidTo > DateTime.UtcNow.AddHours(23));
        }

        [Fact]
        public void GenerarTokenAsync_CuandoFaltaLlaveEnConfiguracion_LanzaArgumentNullException()
        {
            var user = new IdentityUser<Guid> { Id = Guid.NewGuid(), Email = "jhonatan@correo.com" };
            var roles = new List<string> { "Ciudadano" };

            _configurationMock.Setup(c => c["Jwt:key"]).Returns((string)null!);

            Assert.Throws<ArgumentNullException>(() =>
                _tokenService.GenerarTokenAsync(user, roles, "Jhonatan", "OF-01", "Mesa Partes")
            );
        }

        [Fact]
        public void GenerarTokenAsync_CuandoLlaveEsDemasiadoCorta_LanzaArgumentOutOfRangeException()
        {
            var user = new IdentityUser<Guid> { Id = Guid.NewGuid(), Email = "jhonatan@correo.com" };
            var roles = new List<string> { "Ciudadano" };

            _configurationMock.Setup(c => c["Jwt:key"]).Returns("12345");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("WebApi");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("Angular");

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _tokenService.GenerarTokenAsync(user, roles, "Jhonatan", "OF-01", "Mesa Partes")
            );
        }
    }
}
