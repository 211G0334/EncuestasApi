using System;
using System.Collections.Generic;

namespace EncuestasApi.Models.Entities;

public partial class Encuestas
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public DateTime? FechaCreacion { get; set; }

    public int? UsuarioId { get; set; }

    public virtual ICollection<Encuestasaplicadas> Encuestasaplicadas { get; set; } = new List<Encuestasaplicadas>();

    public virtual ICollection<Preguntas> Preguntas { get; set; } = new List<Preguntas>();

    public virtual ICollection<Respuestas> Respuestas { get; set; } = new List<Respuestas>();

    public virtual Usuarios? Usuario { get; set; }
}
