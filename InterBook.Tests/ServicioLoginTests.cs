////using System.Diagnostics.CodeAnalysis;
//using InterBook.API.DTOs;
//using InterBook.API.Interfaces;
//using InterBook.API.Models;
//using InterBook.API.Services;
//using Moq;
//using Xunit;

//namespace InterBook.API.Tests
//{
//    [ExcludeFromCodeCoverage]
//    public class ServicioLoginTests
//    {
//        private readonly Mock<IRepositorioUsuario> _mockRepositorio;
//        private readonly Mock<IServicioCifrado> _mockCifrado;
//        private readonly Mock<IServicioToken> _mockToken;
//        private readonly ServicioLogin _servicio;

//        public ServicioLoginTests()
//        {
//            _mockRepositorio = new Mock<IRepositorioUsuario>();
//            _mockCifrado = new Mock<IServicioCifrado>();
//            _mockToken = new Mock<IServicioToken>();

//            _servicio = new ServicioLogin(
//                _mockRepositorio.Object,
//                _mockCifrado.Object,
//                _mockToken.Object
//            );
//        }

//        [Fact]
//        public async Task IniciarSesion_DebeRetornarError_CuandoUsuarioNoExiste()
//        {
//            // Arrange
//            var dto = new LoginDto
//            {
//                EmailOTelefono = "noexiste@test.com",
//                Password = "Password123!"
//            };

//            _mockRepositorio.Setup(r => r.ObtenerPorEmail(It.IsAny<string>()))
//                .ReturnsAsync((Usuario)null);

//            // Act
//            var resultado = await _servicio.IniciarSesion(dto, "192.168.1.1");

//            // Assert
//            Assert.False(resultado.Exitoso);
//            Assert.Contains("Credenciales inválidas", resultado.Errores);
//        }

//        [Fact]
//        public async Task IniciarSesion_DebeRetornarError_CuandoPasswordIncorrecto()
//        {
//            // Arrange
//            var usuario = new Usuario
//            {
//                Id = 1,
//                Email = "test@test.com",
//                PasswordHash = "hashedpassword",
//                EmailVerificado = true,
//                IntentosLoginFallidos = 0
//            };

//            var dto = new LoginDto
//            {
//                EmailOTelefono = "test@test.com",
//                Password = "PasswordIncorrecto123!"
//            };

//            _mockRepositorio.Setup(r => r.ObtenerPorEmail(It.IsAny<string>()))
//                .ReturnsAsync(usuario);
//            _mockCifrado.Setup(c => c.VerificarPassword(It.IsAny<string>(), It.IsAny<string>()))
//                .Returns(false);

//            // Act
//            var resultado = await _servicio.IniciarSesion(dto, "192.168.1.1");

//            // Assert
//            Assert.False(resultado.Exitoso);
//            Assert.Contains("Credenciales inválidas", resultado.Errores);
//        }

//        [Fact]
//        public async Task IniciarSesion_DebeBloquearCuenta_Despues5IntentosFallidos()
//        {
//            // Arrange
//            var usuario = new Usuario
//            {
//                Id = 1,
//                Email = "test@test.com",
//                PasswordHash = "hashedpassword",
//                EmailVerificado = true,
//                IntentosLoginFallidos = 4
//            };

//            var dto = new LoginDto
//            {
//                EmailOTelefono = "test@test.com",
//                Password = "PasswordIncorrecto123!"
//            };

//            _mockRepositorio.Setup(r => r.ObtenerPorEmail(It.IsAny<string>()))
//                .ReturnsAsync(usuario);
//            _mockCifrado.Setup(c => c.VerificarPassword(It.IsAny<string>(), It.IsAny<string>()))
//                .Returns(false);

//            // Act
//            var resultado = await _servicio.IniciarSesion(dto, "192.168.1.1");

//            // Assert
//            Assert.False(resultado.Exitoso);
//            _mockRepositorio.Verify(r => r.ActualizarIntentosLogin(
//                usuario.Id,
//                5,
//                It.Is<DateTime?>(d => d.HasValue)
//            ), Times.Once);
//        }

//        [Fact]
//        public async Task IniciarSesion_DebeRetornarBloqueo_CuandoCuentaBloqueada()
//        {
//            // Arrange
//            var usuario = new Usuario
//            {
//                Id = 1,
//                Email = "test@test.com",
//                PasswordHash = "hashedpassword",
//                EmailVerificado = true,
//                IntentosLoginFallidos = 5,
//                BloqueoHasta = DateTime.UtcNow.AddMinutes(10)
//            };

//            var dto = new LoginDto
//            {
//                EmailOTelefono = "test@test.com",
//                Password = "Password123!"
//            };

//            _mockRepositorio.Setup(r => r.ObtenerPorEmail(It.IsAny<string>()))
//                .ReturnsAsync(usuario);

//            // Act
//            var resultado = await _servicio.IniciarSesion(dto, "192.168.1.1");

//            // Assert
//            Assert.False(resultado.Exitoso);
//            Assert.True(resultado.CuentaBloqueada);
//            Assert.NotNull(resultado.BloqueoHasta);
//        }

//        [Fact]
//        public async Task IniciarSesion_DebeRetornarToken_CuandoCredencialesCorrectas()
//        {
//            // Arrange
//            var usuario = new Usuario
//            {
//                Id = 1,
//                Email = "test@test.com",
//                PasswordHash = "hashedpassword",
//                EmailVerificado = true,
//                IntentosLoginFallidos = 0
//            };

//            var dto = new LoginDto
//            {
//                EmailOTelefono = "test@test.com",
//                Password = "Password123!"
//            };

//            _mockRepositorio.Setup(r => r.ObtenerPorEmail(It.IsAny<string>()))
//                .ReturnsAsync(usuario);
//            _mockCifrado.Setup(c => c.VerificarPassword(It.IsAny<string>(), It.IsAny<string>()))
//                .Returns(true);
//            _mockToken.Setup(t => t.GenerarTokenJWT(It.IsAny<Usuario>()))
//                .Returns("token-jwt-valido");

//            // Act
//            var resultado = await _servicio.IniciarSesion(dto, "192.168.1.1");

//            // Assert
//            Assert.True(resultado.Exitoso);
//            Assert.Equal("token-jwt-valido", resultado.Token);
//            Assert.NotNull(resultado.Usuario);
//        }

//        [Fact]
//        public async Task IniciarSesion_DebeRetornarError_CuandoEmailNoVerificado()
//        {
//            // Arrange
//            var usuario = new Usuario
//            {
//                Id = 1,
//                Email = "test@test.com",
//                PasswordHash = "hashedpassword",
//                EmailVerificado = false,
//                TelefonoVerificado = false
//            };

//            var dto = new LoginDto
//            {
//                EmailOTelefono = "test@test.com",
//                Password = "Password123!"
//            };

//            _mockRepositorio.Setup(r => r.ObtenerPorEmail(It.IsAny<string>()))
//                .ReturnsAsync(usuario);
//            _mockCifrado.Setup(c => c.VerificarPassword(It.IsAny<string>(), It.IsAny<string>()))
//                .Returns(true);

//            // Act
//            var resultado = await _servicio.IniciarSesion(dto, "192.168.1.1");

//            // Assert
//            Assert.False(resultado.Exitoso);
//            Assert.Contains("Debes verificar tu cuenta", resultado.Errores);

//        }

//        [Fact]
//        public async Task IniciarSesion_DebeBuscarPorTelefono_CuandoNoContieneArroba()
//        {
//            // Arrange
//            var usuario = new Usuario
//            {
//                Id = 1,
//                Telefono = "3001234567",
//                PasswordHash = "hashedpassword",
//                TelefonoVerificado = true
//            };

//            var dto = new LoginDto
//            {
//                EmailOTelefono = "3001234567",
//                Password = "Password123!"
//            };

//            _mockRepositorio.Setup(r => r.ObtenerPorTelefono("3001234567"))
//                .ReturnsAsync(usuario);
//            _mockCifrado.Setup(c => c.VerificarPassword(It.IsAny<string>(), It.IsAny<string>()))
//                .Returns(true);
//            _mockToken.Setup(t => t.GenerarTokenJWT(It.IsAny<Usuario>()))
//                .Returns("token-jwt-valido");

//            // Act
//            var resultado = await _servicio.IniciarSesion(dto, "192.168.1.1");

//            // Assert
//            Assert.True(resultado.Exitoso);
//            _mockRepositorio.Verify(r => r.ObtenerPorTelefono("3001234567"), Times.Once);
//        }
//    }
//}