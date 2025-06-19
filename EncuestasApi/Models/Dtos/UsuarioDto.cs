namespace EncuestasApi.Models.Dtos
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Contraseña { get; set; } = null!;
    }
}
