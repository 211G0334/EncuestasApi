using System;
using System.Collections.Generic;

namespace EncuestasApi.Models.Entities;

public partial class Usuarios
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public virtual ICollection<Encuestas> Encuestas { get; set; } = new List<Encuestas>();

    public virtual ICollection<Encuestasaplicadas> Encuestasaplicadas { get; set; } = new List<Encuestasaplicadas>();
}
