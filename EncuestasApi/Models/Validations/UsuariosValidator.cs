using EncuestasApi.Models.Dtos;
using EncuestasApi.Models.Entities;
using EncuestasApi.Repositories;

namespace EncuestasApi.Models.Validations
{
    public class UsuariosValidator
    {
        private readonly Repository<Usuarios> repository;

        public UsuariosValidator(Repository<Usuarios> repository)
        {
            this.repository = repository;
        }


        public bool Validate(UsuarioDto user, out List<string> errores)
        {
            errores = new List<string>();
            if (string.IsNullOrWhiteSpace(user.Nombre))
            {
                errores.Add("El nombre de usuario esta vacio");
            }
            if (string.IsNullOrWhiteSpace(user.Contraseña))
            {
                errores.Add("La contraseña esta esta vacia");
            }
            if (user.Nombre.Length > 45)
            {
                errores.Add("El nombre debe tener una longitud maxima de 45 caracteres");
            }
            if (user.Contraseña.Length > 12)
            {
                errores.Add("La contraseña debe tener una longitud maxima de 12 caracteres");
            }
            if (repository.GetAll().Any(x => x.Nombre == user.Nombre))
            {
                errores.Add("Ya exsiste este nombre de usuario");
            }
            return errores.Count == 0;
        }
    }
}
