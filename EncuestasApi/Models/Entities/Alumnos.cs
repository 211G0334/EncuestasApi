using System;
using System.Collections.Generic;

namespace EncuestasApi.Models.Entities;

public partial class Alumnos
{
    public int Id { get; set; }

    public string NumeroControl { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Encuestasaplicadas> Encuestasaplicadas { get; set; } = new List<Encuestasaplicadas>();
}
