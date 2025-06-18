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
        private readonly Repository<Encuestasaplicadas> aplicacionRepos;
        private readonly Repository<Alumnos> alumnoRepos;
        private readonly Repository<Encuestas> encuestaRepos;
        private readonly Repository<Preguntas> preguntaRepos;
        private readonly Repository<Respuestas> respuestaRepos;
        public ResultadosController(
            Repository<Encuestasaplicadas> aplicacionRepos,
            Repository<Alumnos> alumnoRepos, Repository<Encuestas> encuestaRepos,
            Repository<Preguntas> preguntaRepos, Repository<Respuestas> respuestaRepos

            )
        {
            this.aplicacionRepos = aplicacionRepos;
            this.alumnoRepos = alumnoRepos;
            this.encuestaRepos = encuestaRepos;
            this.preguntaRepos = preguntaRepos;
            this.respuestaRepos = respuestaRepos;
        }
        [HttpGet]
        public IActionResult Get(int idEncuesta)
        {
            var encuesta = encuestaRepos.Get(idEncuesta);
            if (encuesta == null) return NotFound();

            var preguntas = preguntaRepos.GetAll().Where(p => p.EncuestaId == idEncuesta).ToList();
            var respuestas = respuestaRepos.GetAll().Where(r => r.EncuestaId == idEncuesta).ToList();

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

            var alumno = alumnoRepos.GetAll().FirstOrDefault(x => x.Nombre == numerocontrol);

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

            var aplicaciones = aplicacionRepos.GetAll().Where(x => x.AlumnoId == alumno.Id);
            var respuestas = respuestaRepos.GetAll().ToList();
            var preguntas = preguntaRepos.GetAll().ToList();
            var encuestas = encuestaRepos.GetAll().ToList();

            var resultado = new List<EncuestaConRespDto>();

            foreach (var item in aplicaciones)
            {
                var encuesta = encuestas.FirstOrDefault(e => e.Id == item.EncuestaId);

                var preguntasEncuesta = preguntas.Where(p => p.EncuestaId == encuesta.Id).ToList();

                //var r = respuestas.FirstOrDefault(r => r.EncuestaId == encuesta.Id && r.PreguntaId == preguntasEncuesta)?.Escala ?? 0;
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

            var encuesta = encuestaRepos.Get(idEncuesta);
            if (encuesta == null) return NotFound();

            var interpretaciones = new Dictionary<int, string>
            {
                { 1, "Muy satisfecho" },
                { 2, "Satisfecho" },
                { 3, "Neutral" },
                { 4, "Insatisfecho" },
                { 5, "Muy insatisfecho" }
            };

            var alumnos = alumnoRepos.GetAll();
            var aplicaciones = aplicacionRepos.GetAll().Where(x => x.EncuestaId == encuesta.Id);
            var respuestas = respuestaRepos.GetAll().ToList();
            var preguntas = preguntaRepos.GetAll().ToList();
            var encuestas = encuestaRepos.GetAll().ToList();

            var resultado = new List<EncuestaConRespDto>();

            foreach (var item in aplicaciones)
            {
                var alumno = alumnos.FirstOrDefault(a => a.Id == item.AlumnoId);

                var preguntasEncuesta = preguntas.Where(p => p.EncuestaId == encuesta.Id).ToList();

                //var r = respuestas.FirstOrDefault(r => r.EncuestaId == encuesta.Id && r.PreguntaId == preguntasEncuesta)?.Escala ?? 0;
                var preguntaConRespuesta = preguntasEncuesta.Select(p => {

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
    }
}
