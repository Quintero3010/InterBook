using InterBook.API.Interfaces;
using System.Text.RegularExpressions;

namespace InterBook.API.Services
{
    public class ValidadorPassword : IValidadorPassword
    {
        public bool ValidarFormatoPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (password.Length < 8)
                return false;

            if (!Regex.IsMatch(password, @"[A-Z]", RegexOptions.None, TimeSpan.FromSeconds(2)))
                return false;

            if (!Regex.IsMatch(password, @"\d", RegexOptions.None, TimeSpan.FromSeconds(2)))
                return false;

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]", RegexOptions.None, TimeSpan.FromSeconds(2)))
                return false;

            return true;
        }

        public List<string> ObtenerErroresPassword(string password)
        {
            var errores = new List<string>();

            if (string.IsNullOrEmpty(password))
            {
                errores.Add("La contraseña es requerida");
                return errores;
            }

            if (password.Length < 8)
                errores.Add("La contraseña debe tener mínimo 8 caracteres");

            if (!Regex.IsMatch(password, @"[A-Z]", RegexOptions.None, TimeSpan.FromSeconds(2)))
                errores.Add("La contraseña debe contener al menos una mayúscula");

            if (!Regex.IsMatch(password, @"\d", RegexOptions.None, TimeSpan.FromSeconds(2)))
                errores.Add("La contraseña debe contener al menos un número");

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]", RegexOptions.None, TimeSpan.FromSeconds(2)))
                errores.Add("La contraseña debe contener al menos un carácter especial");

            return errores;
        }
    }
}
