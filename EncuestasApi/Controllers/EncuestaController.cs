﻿using EncuestasApi.Hubs;
using EncuestasApi.Models.Dtos;
using EncuestasApi.Models.Entities;
using EncuestasApi.Models.Validations;
using EncuestasApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;

namespace EncuestasApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EncuestaController : ControllerBase
    {

        private readonly IHubContext<HubEncuesta> _hubContext;
        private readonly Repository<Encuestas> repository;
        private readonly Repository<Preguntas> preguntasRepository;
        private readonly EncuestaValidator valitador;
        private readonly Repository<Usuarios> usuarioRepository;
        private readonly Repository<Encuestasaplicadas> aplicacionRepository;
        public EncuestaController(
            Repository<Encuestas> repository,
            Repository<Preguntas> preguntasRepository,
            EncuestaValidator valitador,
            Repository<Usuarios> usuarioRepository,
            Repository<Encuestasaplicadas> aplicacionRepository)
        {
            this.repository = repository;
            this.preguntasRepository = preguntasRepository;
            this.valitador = valitador;
            this.usuarioRepository = usuarioRepository;
            this.aplicacionRepository = aplicacionRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var id = int.Parse(User.FindFirst("Id")?.Value ?? "0");

            var lista = repository.GetAll()
                .Where(x => x.UsuarioId == id)
                .Select(x => new
                {
                    x.Id,
                    x.Titulo
                });

            return Ok(new
            {
                lstEncusta = lista
            });
        }

        [HttpGet("todas")]
        [AllowAnonymous]
        public IActionResult GetTodas()
        {
            var lista = repository.GetAll()
                .Select(x => new
                {
                    x.Id,
                    x.Titulo
                });

            return Ok(new
            {
                lstEncusta = lista
            });
        }
        //agrega encuesta y la guarda correctamente
        [HttpPost("addEncuesta")]
        public IActionResult Post(EncuestaDto dto)
        {
            if (valitador.Validate(dto, out List<string> errores))
            {
                var idUsuario = int.Parse(User.FindFirst("Id")?.Value ?? "0");

                var encuesta = new Encuestas
                {
                    Titulo = dto.Titulo,
                    UsuarioId = idUsuario,
                    FechaCreacion = DateTime.Now
                };

                repository.Insert(encuesta);

                foreach (var item in dto.lstPreguntas)
                {
                    var pregunta = new Preguntas
                    {
                        Texto = item.Texto,
                        EncuestaId = encuesta.Id
                    };

                    preguntasRepository.Insert(pregunta);
                }

                return Ok();
            }
            else
            {
                return BadRequest(errores);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetEncuestaConPreguntas(int id)
        {
            var encuesta = repository.Get(id);
            if (encuesta == null)
                return NotFound("Encuesta no encontrada");

            var preguntas = preguntasRepository.GetAll()
                .Where(p => p.EncuestaId == id)
                .Select(p => new PreguntaDTO
                {
                    Id = p.Id,
                    Texto = p.Texto
                }).ToList();

            var dto = new EncuestaDto
            {
                Id = encuesta.Id,
                Titulo = encuesta.Titulo,
                lstPreguntas = preguntas
            };

            return Ok(dto);
        }




        //eliminar encuesta por id
        [HttpDelete("{id}")]
        public IActionResult EliminarEncuesta(int id)
        {
            try
            {
                var encuesta = repository.Get(id);
                if (encuesta == null)
                    return NotFound("Encuesta no encontrada");

                // Eliminar preguntas relacionadas primero
                var preguntas = preguntasRepository.GetAll()
                    .Where(p => p.EncuestaId == id)
                    .ToList();

                foreach (var p in preguntas)
                {
                    preguntasRepository.Delete(p.Id);
                }

                repository.Delete(id);
                return Ok("Encuesta eliminada correctamente");
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return BadRequest("No se puede eliminar la encuesta porque ya fue aplicada a uno o más alumnos.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error inesperado al intentar eliminar la encuesta: " + ex.Message);
            }
        }

    }
}
