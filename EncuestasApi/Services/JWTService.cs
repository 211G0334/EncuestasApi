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
            //Buscar si existe en la bd el usuario
            var usuario = Repository.GetAll()
                .FirstOrDefault(x => x.Nombre == dto.Nombre &&
                x.Contraseña == dto.Contraseña);

            if (usuario == null)
            {
                return null;
            }
            else
            {
                //1. Crear las claims

                List<Claim> claims = new List<Claim>()
                {
                    new Claim("Id", usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre)
                };

                //2. Crear un descriptor de token

                var descriptor = new JwtSecurityToken(
                    issuer: Configuration["Jwt:Issuer"],
                    audience: Configuration["Jwt:Audience"],
                    claims: claims,

                    expires: DateTime.UtcNow.AddMinutes(5),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256)
                    );
                //3. Crear un JWT
                var handler = new JwtSecurityTokenHandler();
                return handler.WriteToken(descriptor);
            }

        }
    }
}
