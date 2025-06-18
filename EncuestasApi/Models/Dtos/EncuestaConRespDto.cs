namespace EncuestasApi.Models.Dtos
{
    public class EncuestaConRespDto
    {
        public string NumeroConreol { get; set; }
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public List<PreguntaRespuestaDTO> lstPreguntas { get; set; } = new List<PreguntaRespuestaDTO>();
    }

    public class PreguntaRespuestaDTO
    {
        public int PreguntaId { get; set; }
        public string Texto { get; set; } = null!;
        public string Respuesta { get; set; } = null!;
        public int Escala { get; set; }
    }
}
