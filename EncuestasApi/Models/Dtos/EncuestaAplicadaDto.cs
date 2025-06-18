namespace EncuestasApi.Models.Dtos
{
    public class EncuestaAplicadaDto
    {
        public int EncuestaId { get; set; }
        public string NumControl { get; set; } = null!;
        public List<RespuestaDTO> Respuestas { get; set; } = new();
    }



    public class RespuestaDTO
    {
        public int id { get; set; }
        public int PreguntaId { get; set; }
        public int Valor { get; set; } // Entre 1 y 5

    }
}
