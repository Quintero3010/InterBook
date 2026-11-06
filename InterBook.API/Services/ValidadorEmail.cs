using InterBook.API.Interfaces;
using System.Text.RegularExpressions;

namespace InterBook.API.Services
{
    public class ValidadorEmail : IValidadorEmail
    {
        public bool ValidarFormatoEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, patron, RegexOptions.None, TimeSpan.FromSeconds(2));
        }
    }
}