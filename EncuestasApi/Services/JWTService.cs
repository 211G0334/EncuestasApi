using EncuestasApi.Models.Dtos;
using EncuestasApi.Models.Entities;
using EncuestasApi.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EncuestasApi.Services
{
    public class JWTService(IConfiguration configuration, Repository<Usuarios> repository)
    {
        public IConfiguration Configuration { get; } = configuration;
        public Repository<Usuarios> Repository { get; } = repository;

        public string? GenerarToken(UsuarioDto dto)
        {
            // Validar si la configuración es nula o si faltan claves esenciales
            var issuer = Configuration["Jwt:Issuer"];
            var audience = Configuration["Jwt:Audience"];
            var key = Configuration?["Jwt:Key"];

            if (issuer == null || audience == null || key == null)
            {

                return null;
            }

            // Buscar si existe en la bd el usuario
            var usuario = Repository.GetAll()
                .FirstOrDefault(x => x.Nombre == dto.Nombre &&
                                     x.Contraseña == dto.Contraseña);

            if (usuario == null)
            {
                return null;
            }

            // Crear las claims
            List<Claim> claims = new List<Claim>()
    {
        new Claim("Id", usuario.Id.ToString()),
        new Claim(ClaimTypes.Name, usuario.Nombre)
    };

            // Crear el token descriptor
            var descriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256)
            );

            // Generar el token
            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(descriptor);
        }

    }
}
