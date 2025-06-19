using EncuestasApi.Models.Dtos;
using EncuestasApi.Models.Entities;
using EncuestasApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EncuestasApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ResultadosController : ControllerBase
    {
        private readonly Repository<Encuestasaplicadas> _aplicacionRepos;
        private readonly Repository<Alumnos> _alumnoRepos;
        private readonly Repository<Encuestas> _encuestaRepos;
        private readonly Repository<Preguntas> _preguntaRepos;
        private readonly Repository<Respuestas> _respuestaRepos;
        public ResultadosController(
            Repository<Encuestasaplicadas> aplicacionRepos,
            Repository<Alumnos> alumnoRepos, Repository<Encuestas> encuestaRepos,
            Repository<Preguntas> preguntaRepos, Repository<Respuestas> respuestaRepos

            )
        {
            _aplicacionRepos = aplicacionRepos;
            _alumnoRepos = alumnoRepos;
            _encuestaRepos = encuestaRepos;
            _preguntaRepos = preguntaRepos;
            _respuestaRepos = respuestaRepos;
        }
        [HttpGet]
        public IActionResult Get(int idEncuesta)
        {
            var encuesta = _encuestaRepos.Get(idEncuesta);
            if (encuesta == null) return NotFound();

            var preguntas = _preguntaRepos.GetAll().Where(p => p.EncuestaId == idEncuesta).ToList();
            var respuestas = _respuestaRepos.GetAll().Where(r => r.EncuestaId == idEncuesta).ToList();

            var resultado = new ResultadoEncuestaDto
            {
                Titulo = encuesta.Titulo,
                lstPreguntas = preguntas.Select(p => new PreguntaResultadoDTO
                {
                    Texto = p.Texto,
                    Opciones = Enumerable.Range(1, 5).Select(valor =>
                    {
                        var total = respuestas.Count(r => r.PreguntaId == p.Id);
                        var cantidad = respuestas.Count(r => r.PreguntaId == p.Id && r.Escala == valor);
                        double porcentaje = total > 0 ? (cantidad * 100.0 / total) : 0;
                        return new OpcionResultadoDTO
                        {
                            Descripcion = $"Valor {valor}",
                            Porcentaje = porcentaje
                        };
                    }).ToList()
                }).ToList()
            };

            return Ok(resultado);
        }
        [HttpGet("por-alumno")]
        public IActionResult GetEncuestaPorAlumno(string numerocontrol)
        {

            var alumno = _alumnoRepos.GetAll().FirstOrDefault(x => x.Nombre == numerocontrol);

            if (alumno == null)
            {
                return NotFound("Alumno no encotrado");
            }

            var interpretaciones = new Dictionary<int, string>
            {
               { 1, "Excelente" },
               { 2, "Bueno" },
               { 3, "Regular" },
               { 4, "Deficiente" },
               { 5, "Muy deficiente" }

            };

            var aplicaciones = _aplicacionRepos.GetAll().Where(x => x.AlumnoId == alumno.Id);
            var respuestas = _respuestaRepos.GetAll().ToList();
            var preguntas = _preguntaRepos.GetAll().ToList();
            var encuestas = _encuestaRepos.GetAll().ToList();

            var resultado = new List<EncuestaConRespDto>();

            foreach (var item in aplicaciones)
            {
                var encuesta = encuestas.FirstOrDefault(e => e.Id == item.EncuestaId);
                if (encuesta == null) { return BadRequest(); }
                var preguntasEncuesta = preguntas.Where(p => p.EncuestaId == encuesta.Id).ToList();

                var preguntaConRespuesta = preguntasEncuesta.Select(p =>
                {

                    var r = respuestas.FirstOrDefault(r => r.EncuestaId == encuesta.Id && r.PreguntaId == p.Id);
                    var escala = r?.Escala ?? 0;

                    string interpretacion = interpretaciones.ContainsKey(escala) ? interpretaciones[escala] : "Sin respuesta";
                    return new PreguntaRespuestaDTO
                    {
                        PreguntaId = p.Id,
                        Texto = p.Texto,
                        Escala = escala,
                        Respuesta = interpretacion
                    };
                }).ToList();

                resultado.Add(new EncuestaConRespDto
                {
                    NumeroConreol = alumno.Nombre,
                    Id = encuesta.Id,
                    Titulo = encuesta.Titulo,
                    lstPreguntas = preguntaConRespuesta
                });
            }

            return Ok(resultado);
        }
        [HttpGet("porencuesta")]
        public IActionResult GetPorEncuesta(int idEncuesta)
        {

            var encuesta = _encuestaRepos.Get(idEncuesta);
            if (encuesta == null) return NotFound();

            var interpretaciones = new Dictionary<int, string>
            {
               { 1, "Excelente" },
               { 2, "Bueno" },
               { 3, "Regular" },
               { 4, "Deficiente" },
               { 5, "Muy deficiente" }
            };

            var alumnos = _alumnoRepos.GetAll();
            var aplicaciones = _aplicacionRepos.GetAll().Where(x => x.EncuestaId == encuesta.Id);
            var respuestas = _respuestaRepos.GetAll().ToList();
            var preguntas = _preguntaRepos.GetAll().ToList();
            var encuestas = _encuestaRepos.GetAll().ToList();

            var resultado = new List<EncuestaConRespDto>();

            foreach (var item in aplicaciones)
            {

                var preguntasEncuesta = preguntas.Where(p => p.EncuestaId == encuesta.Id).ToList();

                var preguntaConRespuesta = preguntasEncuesta.Select(p =>
                {

                    var r = respuestas.FirstOrDefault(r => r.EncuestaId == encuesta.Id && r.PreguntaId == p.Id);
                    var escala = r?.Escala ?? 0;

                    string interpretacion = interpretaciones.ContainsKey(escala) ? interpretaciones[escala] : "Sin respuesta";
                    return new PreguntaRespuestaDTO
                    {
                        PreguntaId = p.Id,
                        Texto = p.Texto,
                        Escala = escala,
                        Respuesta = interpretacion
                    };
                }).ToList();
                var alumno = alumnos.FirstOrDefault(a => a.Id == item.AlumnoId);
                if (alumno != null)
                {

                    resultado.Add(new EncuestaConRespDto
                    {
                        NumeroConreol = alumno.Nombre,
                        Id = encuesta.Id,
                        Titulo = encuesta.Titulo,
                        lstPreguntas = preguntaConRespuesta
                    });
                }
            }
                    return Ok(resultado);

        }

    }
}

