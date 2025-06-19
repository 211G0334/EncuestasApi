using EncuestasApi.Models.Dtos;
using EncuestasApi.Models.Entities;
using EncuestasApi.Models.Validations;
using EncuestasApi.Repositories;
using EncuestasApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EncuestasApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        Repository<Usuarios> _repository;
        Repository<Alumnos> _repository2;
        UsuariosValidator _validator;
        JWTService _jwtService;
        public UsuarioController(
            Repository<Usuarios> repo,
            Repository<Alumnos>repo2,
            UsuariosValidator validador,
            JWTService Jwt)
        {
            _jwtService = Jwt;
            _repository = repo;
            _repository2 = repo2;
            _validator = validador;
        }
        //Ingreso de usuarios registrados

        [HttpPost("login")]
        public IActionResult Login(UsuarioDto usuario)
        {
            var Token = _jwtService.GenerarToken(usuario);
            if (Token != null) 
            {

            return Ok(Token);
            }
            return Unauthorized("Error al ingresae usuario o contraseña");
        }
        //Registro de usuarios y valida

        [HttpPost("registro")]
        public IActionResult Registrar(UsuarioDto usuarioDto)
        {

            var existe = _repository2.GetAll().FirstOrDefault(x => x.NumeroControl == usuarioDto.Contraseña);
            if (existe != null)
            {
                return Ok(new
                {
                    mensaje = "Inicio exitoso",
                });
            }

            if (_validator.Validate(usuarioDto, out List<string> error))
            {
                Usuarios usuarios = new Usuarios()
                {
                    Contraseña = usuarioDto.Contraseña,
                    Nombre = usuarioDto.Nombre
                };
                return Ok();
            }
            else
            {
                return BadRequest(error);
            }
        }

    }
}
