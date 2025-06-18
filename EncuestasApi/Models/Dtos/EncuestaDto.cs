namespace EncuestasApi.Models.Dtos
{
    public class EncuestaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public List<PreguntaDTO> lstPreguntas { get; set; } = new List<PreguntaDTO>();
    }
    public class PreguntaDTO
    {
        public int Id { get; set; }
        public string Texto { get; set; } = null!;
    }
}
