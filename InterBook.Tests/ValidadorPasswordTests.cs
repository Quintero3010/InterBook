using System.Diagnostics.CodeAnalysis;
using InterBook.API.Services;
using Xunit;

namespace InterBook.API.Tests
{
    [ExcludeFromCodeCoverage]
    public class ValidadorPasswordTests
    {
        private readonly ValidadorPassword _validador;

        public ValidadorPasswordTests()
        {
            _validador = new ValidadorPassword();
        }

        [Fact]
        public void ValidarFormatoPassword_DebeRetornarTrue_CuandoPasswordEsValido()
        {
            // Arrange
            string password = "Password123!";

            // Act
            bool resultado = _validador.ValidarFormatoPassword(password);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void ValidarFormatoPassword_DebeFallar_SiNoTiene8Caracteres()
        {
            // Arrange
            string password = "Pass1!";

            // Act
            bool resultado = _validador.ValidarFormatoPassword(password);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void ValidarFormatoPassword_DebeFallar_SinMayuscula()
        {
            // Arrange
            string password = "password123!";

            // Act
            bool resultado = _validador.ValidarFormatoPassword(password);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void ValidarFormatoPassword_DebeFallar_SinNumero()
        {
            // Arrange
            string password = "Password!";

            // Act
            bool resultado = _validador.ValidarFormatoPassword(password);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void ValidarFormatoPassword_DebeFallar_SinCaracterEspecial()
        {
            // Arrange
            string password = "Password123";

            // Act
            bool resultado = _validador.ValidarFormatoPassword(password);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void ObtenerErroresPassword_DebeRetornarListaDeErrores_CuandoPasswordInvalido()
        {
            // Arrange
            string password = "pass";

            // Act
            var errores = _validador.ObtenerErroresPassword(password);

            // Assert
            Assert.NotEmpty(errores);
            Assert.Contains(errores, e => e.Contains("mínimo 8 caracteres"));
        }
    }
}