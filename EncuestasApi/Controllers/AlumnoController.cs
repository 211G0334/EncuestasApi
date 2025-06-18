using EncuestasApi.Models.Dtos;
using EncuestasApi.Models.Entities;
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

    public class AlumnoController : ControllerBase
    {


        private readonly Repository<Alumnos> repo;
        private readonly JWTService service;

        public AlumnoController(Repository<Alumnos> repo, JWTService service)
        {
            this.repo = repo;
            this.service = service;
        }


        [HttpPost("login")]
        public IActionResult Login(AlumnoDto dto)
        {
            var existe = repo.GetAll().FirstOrDefault(x => x.NumeroControl == dto.NumeroControl);

            if (existe != null)
            {
                return Ok(new
                {
                    mensaje = "Inicio exitoso",
                    idEncuesta = dto.IdEncuestas
                });
            }
            else if (existe == null)
            {
                Alumnos user = new()
                {
                    Nombre = dto.Nombre,
                    NumeroControl = dto.NumeroControl
                };
                repo.Insert(user);
                return Ok(new
                {
                    mensaje = "Resgistro exitoso",
                    idEncuesta = dto.IdEncuestas
                });
            }
            else
            {
                return BadRequest();
            }
        }
    }
}

