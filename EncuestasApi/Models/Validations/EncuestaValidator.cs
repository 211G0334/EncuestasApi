using EncuestasApi.Models.Dtos;

namespace EncuestasApi.Models.Validations
{
    public class EncuestaValidator
    {
        public bool Validate(EncuestaDto dto, out List<string> errores)
        {
            errores = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Titulo))
            {
                errores.Add("El titulo es obligarorio.");
            }

            if (dto.Titulo.Length > 200)
            {
                errores.Add("El titulo no debe de superar los 200 caracteres");
            }

            return errores.Count == 0;
        }
    }
}
