namespace EncuestasApi.Models.Dtos
{
    public class ResultadoEncuestaDto
    {
        public string Titulo { get; set; } = null!;
        public List<PreguntaResultadoDTO> lstPreguntas { get; set; } = new();
    }

    public class PreguntaResultadoDTO
    {
        public string Texto { get; set; } = null!;
        public List<OpcionResultadoDTO> Opciones { get; set; } = new();
    }

    public class OpcionResultadoDTO
    {
        public string Descripcion { get; set; } = null!;
        public double Porcentaje { get; set; }
    }
}
