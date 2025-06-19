using EncuestasApi.Hubs;
using EncuestasApi.Models.Dtos;
using EncuestasApi.Models.Entities;
using EncuestasApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EncuestasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AplicacionesController : ControllerBase
    {
        private readonly Repository<Encuestasaplicadas> aplicacionRepository;
    private readonly Repository<Encuestas> encuestasRepository;
    private readonly Repository<Preguntas> preguntaRepository;
    private readonly Repository<Alumnos> alummnoRepository;
    private readonly Repository<Respuestas> respuestaRepository;
    private readonly Repository<Usuarios> usuarioRepository;
    private readonly IHubContext<HubEncuesta> _hub;

    public AplicacionesController(
        Repository<Encuestasaplicadas> aplicacionRepository,
        Repository<Encuestas> encuestasRepository,
        Repository<Preguntas> preguntaRepository,
        Repository<Alumnos> alummnoRepository,
        Repository<Respuestas> respuestaRepository,
        Repository<Usuarios> usuarioRepository,
        IHubContext<HubEncuesta> hub)
    {
        this.aplicacionRepository = aplicacionRepository;
        this.encuestasRepository = encuestasRepository;
        this.preguntaRepository = preguntaRepository;
        this.alummnoRepository = alummnoRepository;
        this.respuestaRepository = respuestaRepository;
        this.usuarioRepository = usuarioRepository;
        _hub = hub;
    }


    [HttpGet]
    public IActionResult Get(int idEncuensta, string num)
    {

        var encontraralumno = alummnoRepository.GetAll().FirstOrDefault(x => x.NumeroControl == num);
        var encontrarEnuesta = encuestasRepository.Get(idEncuensta);
        var yaAplicada = aplicacionRepository.GetAll().FirstOrDefault(x => x.EncuestaId == idEncuensta && x.AlumnoId == encontraralumno.Id);

        if (yaAplicada != null)
        {
            return BadRequest("Este alumnos ya ha conestado esta encuesta");
        }

        if (encontrarEnuesta != null)
        {
            List<Preguntas> EncontrarPreguntas = preguntaRepository.GetAll().Where(x => x.EncuestaId == encontrarEnuesta.Id).ToList();

            EncuestaDto mostarEncuesta = new()
            {
                Id = encontrarEnuesta.Id,
                Titulo = encontrarEnuesta.Titulo,
                lstPreguntas = new List<PreguntaDTO>()
            };


            foreach (var item in EncontrarPreguntas)
            {
                mostarEncuesta.lstPreguntas.Add(new PreguntaDTO
                {
                    Id = item.Id,
                    Texto = item.Texto
                });
            }

            return Ok(mostarEncuesta);
        }
        else
        {
            return BadRequest("Encuesta no encontrada");
        }

    }

    [HttpGet("mostarDashboard")]
    public IActionResult MostarDashboard()
    {
        var encuestas = encuestasRepository.GetAll().ToList();
        var preguntas = preguntaRepository.GetAll().ToList();
        var usuario = usuarioRepository.GetAll().ToList();

        DatosActualizadosDto nuevo = new()
        {
            CantidadEncuestas = encuestasRepository.GetAll().Count(),
            RespuestasRecibidas = respuestaRepository.GetAll().Count(),
            CantidadDeAlumnos = aplicacionRepository.GetAll().Select(x => x.AlumnoId).Count()
        };

        nuevo.lstEncuestasDisponibles = encuestas.Select(e => new ResumenEncuestasDto
        {
            Id = e.Id,
            Titulo = e.Titulo,
            FechaCreacion = e.FechaCreacion,
            lstUsuarioCreador = usuario
                .Where(u => u.Id == e.UsuarioId)
                .Select(u => new UsuarioResumenDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre
                }).ToList(),
            lstPreguntaEncuesta = preguntas
                .Where(p => p.EncuestaId == e.Id)
                .Select(p => new PreguntaResumenDto
                {
                    Id = p.Id,
                    Texto = p.Texto
                }).ToList()
        }).ToList();

        return Ok(nuevo);
    }




    [HttpPost("aplicar")]
    public async Task<IActionResult> AplicarEncuesta([FromBody] EncuestaAplicadaDto dto)
    {
        if (dto == null || dto.Respuestas == null || dto.Respuestas.Count == 0)
            return BadRequest("Datos inválidos");

        var alumno = alummnoRepository.GetAll().FirstOrDefault(x => x.NumeroControl == dto.NumControl);
        var creadorEncuesta = encuestasRepository.GetAll().FirstOrDefault(x => x.Id == dto.EncuestaId);

        // Crear registro de aplicación
        var aplicacion = new Encuestasaplicadas
        {
            EncuestaId = dto.EncuestaId,
            AlumnoId = alumno.Id,
            UsuarioId = creadorEncuesta.UsuarioId,
            FechaAplicacion = DateTime.Now
        };

        aplicacionRepository.Insert(aplicacion); // Aquí se guarda y se asigna el Id

        // Registrar respuestas
        foreach (var respuesta in dto.Respuestas)
        {
            var nueva = new Respuestas
            {
                EncuestaId = dto.EncuestaId,
                PreguntaId = respuesta.PreguntaId,
                Escala = respuesta.Valor,
            };

            respuestaRepository.Insert(nueva);
            //respuesta.Insert(nueva);
        }



        DatosActualizadosDto nuevo = new()
        {
            CantidadEncuestas = encuestasRepository.GetAll().Count(),
            RespuestasRecibidas = respuestaRepository.GetAll().Count(),
            CantidadDeAlumnos = aplicacionRepository.GetAll().Select(x => x.AlumnoId).Count()
        };
        var encuestas = encuestasRepository.GetAll().ToList();
        var preguntas = preguntaRepository.GetAll().ToList();
        var usuario = usuarioRepository.GetAll().ToList();


        nuevo.lstEncuestasDisponibles = encuestas.Select(e => new ResumenEncuestasDto
        {
            Id = e.Id,
            Titulo = e.Titulo,
            FechaCreacion = e.FechaCreacion,
            lstUsuarioCreador = usuario
                .Where(u => u.Id == e.UsuarioId)
                .Select(u => new UsuarioResumenDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre
                }).ToList(),
            lstPreguntaEncuesta = preguntas
                .Where(p => p.EncuestaId == e.Id)
                .Select(p => new PreguntaResumenDto
                {
                    Id = p.Id,
                    Texto = p.Texto
                }).ToList()
        }).ToList();
        await _hub.Clients.All.SendAsync("RespuestasRecibidas", nuevo);
        return Ok();
    }
}
}
