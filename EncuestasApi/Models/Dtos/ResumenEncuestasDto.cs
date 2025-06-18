namespace EncuestasApi.Models.Dtos
{
    public class ResumenEncuestasDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = "";
        public DateTime? FechaCreacion { get; set; }

        public List<PreguntaResumenDto> lstPreguntaEncuesta { get; set; } = new();
        public List<UsuarioResumenDto> lstUsuarioCreador { get; set; } = new();
    }
}
