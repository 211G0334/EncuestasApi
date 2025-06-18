namespace EncuestasApi.Models.Dtos
{
    public class DatosActualizadosDto
    {
        public int CantidadEncuestas { get; set; }
        public int RespuestasRecibidas { get; set; }
        public int EntrevistadoresActivos { get; set; }
        public int CantidadDeAlumnos { get; set; }
        public List<EncuestaResumenDTO> lstEncuestasDisponibles { get; set; } = new();
    }
}
