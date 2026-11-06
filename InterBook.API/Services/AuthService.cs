using InterBook.API.Data;
using InterBook.API.DTOs;
using InterBook.API.Interfaces;
using InterBook.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InterBook.API.Services
{
    public class AuthService : IAuthService
    {
     private readonly InterBookDbContext _context;
  private readonly IConfiguration _configuration;

        public AuthService(
 InterBookDbContext context,
  IConfiguration configuration)
  {
  _context = context;
      _configuration = configuration;
        }

      public async Task<AuthResponseDto> Login(LoginDto loginDto)
{
    try
   {
      var usuario = await _context.Usuarios
  .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Activo);

   if (usuario == null)
      {
    return new AuthResponseDto
  {
       Exitoso = false,
    Mensaje = "Credenciales inválidas",
      Errores = new List<string> { "Usuario no encontrado" }
     };
     }

       bool passwordValido = BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.PasswordHash);

   if (!passwordValido)
    {
   return new AuthResponseDto
     {
   Exitoso = false,
   Mensaje = "Credenciales inválidas",
  Errores = new List<string> { "Contraseña incorrecta" }
    };
    }

string token = GenerarTokenJWT(usuario);

      return new AuthResponseDto
{
      Exitoso = true,
  Mensaje = "Inicio de sesión exitoso",
     Token = token,
  Usuario = new UsuarioDto
           {
            Id = usuario.Id,
     Nombre = usuario.Nombre,
      Email = usuario.Email,
       Telefono = usuario.Telefono,
    Rol = usuario.Rol,
   Ciudad = usuario.Ciudad,
      EmailVerificado = usuario.EmailVerificado,
     IntercambiosRealizados = usuario.IntercambiosRealizados
       }
       };
   }
  catch (Exception ex)
  {
   return new AuthResponseDto
   {
   Exitoso = false,
   Mensaje = "Error en el servidor",
    Errores = new List<string> { ex.Message }
         };
   }
        }

        public async Task<AuthResponseDto> Registro(RegistroDto registroDto)
   {
     try
   {
   var errores = new List<string>();

   if (string.IsNullOrWhiteSpace(registroDto.Email) || !registroDto.Email.Contains("@"))
     {
      errores.Add("El formato del email es inválido");
      }

    if (string.IsNullOrWhiteSpace(registroDto.Password) || registroDto.Password.Length < 8)
   {
 errores.Add("La contraseña debe tener al menos 8 caracteres");
    }

if (string.IsNullOrWhiteSpace(registroDto.Nombre))
      {
  errores.Add("El nombre es requerido");
 }

       if (string.IsNullOrWhiteSpace(registroDto.Telefono))
         {
  errores.Add("El teléfono es requerido");
   }

     if (string.IsNullOrWhiteSpace(registroDto.Ciudad))
   {
  errores.Add("La ciudad es requerida");
        }

  if (!string.IsNullOrWhiteSpace(registroDto.Email))
       {
      var emailExiste = await _context.Usuarios
       .AnyAsync(u => u.Email == registroDto.Email);

if (emailExiste)
   {
     errores.Add("El email ya está registrado");
        }
      }

        if (!string.IsNullOrWhiteSpace(registroDto.Telefono))
    {
 var telefonoExiste = await _context.Usuarios
      .AnyAsync(u => u.Telefono == registroDto.Telefono);

        if (telefonoExiste)
         {
   errores.Add("El teléfono ya está registrado");
  }
         }

 if (errores.Any())
    {
     return new AuthResponseDto
   {
     Exitoso = false,
  Mensaje = "Error en los datos de registro",
  Errores = errores
    };
      }

       var nuevoUsuario = new Usuario
      {
    Nombre = registroDto.Nombre!.Trim(),
Email = registroDto.Email!.Trim().ToLower(),
     Telefono = registroDto.Telefono!.Trim(),
Ciudad = registroDto.Ciudad!.Trim(),
       Barrio = registroDto.Barrio?.Trim() ?? string.Empty,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(registroDto.Password),
       FechaRegistro = DateTime.UtcNow,
        Rol = "Usuario",
Activo = true,
   EmailVerificado = true,
      TelefonoVerificado = false,
IntentosLoginFallidos = 0,
       IntercambiosRealizados = 0,
  CalificacionPromedio = 0,
        Biografia = string.Empty,
FotoPerfilUrl = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(registroDto.Nombre ?? "Usuario")}&background=random"
  };

      _context.Usuarios.Add(nuevoUsuario);
      await _context.SaveChangesAsync();

 return new AuthResponseDto
   {
    Exitoso = true,
   Mensaje = "Registro exitoso. Ahora puedes iniciar sesión.",
       Usuario = new UsuarioDto
    {
  Id = nuevoUsuario.Id,
  Nombre = nuevoUsuario.Nombre,
  Email = nuevoUsuario.Email,
        Telefono = nuevoUsuario.Telefono,
   Rol = nuevoUsuario.Rol,
 Ciudad = nuevoUsuario.Ciudad,
  EmailVerificado = nuevoUsuario.EmailVerificado,
  IntercambiosRealizados = nuevoUsuario.IntercambiosRealizados
  }
  };
 }
 catch (DbUpdateException dbEx)
    {
        return new AuthResponseDto
      {
  Exitoso = false,
  Mensaje = "Error al guardar en la base de datos",
  Errores = new List<string> { 
  "Error de base de datos. Verifica que todos los campos estén correctos.",
       dbEx.InnerException?.Message ?? dbEx.Message 
      }
        };
  }
  catch (Exception ex)
    {
    return new AuthResponseDto
      {
    Exitoso = false,
       Mensaje = "Error en el servidor",
    Errores = new List<string> { ex.Message, ex.InnerException?.Message ?? "" }
    };
     }
  }

  public async Task<bool> CambiarPassword(int usuarioId, CambiarPasswordDto cambiarPasswordDto)
    {
  try
    {
var usuario = await _context.Usuarios.FindAsync(usuarioId);
   if (usuario == null) return false;

     bool passwordValido = BCrypt.Net.BCrypt.Verify(
         cambiarPasswordDto.PasswordActual,
     usuario.PasswordHash);

 if (!passwordValido) return false;

usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(cambiarPasswordDto.PasswordNueva);
await _context.SaveChangesAsync();

      return true;
  }
  catch
    {
   return false;
 }
 }

        public async Task<Usuario> ObtenerUsuarioPorId(int id)
        {
      return await _context.Usuarios
 .FirstOrDefaultAsync(u => u.Id == id && u.Activo);
 }

    public async Task<Usuario> ObtenerUsuarioPorEmail(string email)
        {
 return await _context.Usuarios
  .FirstOrDefaultAsync(u => u.Email == email && u.Activo);
 }

      private string GenerarTokenJWT(Usuario usuario)
 {
   var jwtKey = _configuration["Jwt:Key"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
 var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

   var claims = new[]
   {
     new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
 new Claim(ClaimTypes.Email, usuario.Email),
 new Claim(ClaimTypes.Name, usuario.Nombre),
   new Claim(ClaimTypes.Role, usuario.Rol),
   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
 };

   var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
   audience: _configuration["Jwt:Audience"],
claims: claims,
  expires: DateTime.UtcNow.AddMinutes(
    Convert.ToDouble(_configuration["Jwt:ExpireMinutes"] ?? "1440")),
     signingCredentials: credentials
   );

    return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}