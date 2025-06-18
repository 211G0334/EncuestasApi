namespace EncuestasApi.Models.Dtos
{
    public class EditarEncuestasDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public List<PreguntaDTO> Preguntas { get; set; } = new();
    }
}
