using System.Diagnostics.CodeAnalysis;
using InterBook.API.Services;
using Xunit;

namespace InterBook.API.Tests
{
    [ExcludeFromCodeCoverage]
    public class ValidadorEmailTests
    {
        private readonly ValidadorEmail _validador;

        public ValidadorEmailTests()
        {
            _validador = new ValidadorEmail();
        }

        [Theory]
        [InlineData("usuario@ejemplo.com")]
        [InlineData("test.user@dominio.co")]
        [InlineData("admin@interbook.com.co")]
        public void ValidarFormatoEmail_DebeRetornarTrue_CuandoEmailEsValido(string email)
        {
            // Act
            bool resultado = _validador.ValidarFormatoEmail(email);

            // Assert
            Assert.True(resultado);
        }

        [Theory]
        [InlineData("")]
        [InlineData("invalido")]
        [InlineData("@ejemplo.com")]
        [InlineData("usuario@")]
        [InlineData("usuario@@ejemplo.com")]
        public void ValidarFormatoEmail_DebeRetornarFalse_CuandoEmailEsInvalido(string email)
        {
            // Act
            bool resultado = _validador.ValidarFormatoEmail(email);

            // Assert
            Assert.False(resultado);
        }
    }
}