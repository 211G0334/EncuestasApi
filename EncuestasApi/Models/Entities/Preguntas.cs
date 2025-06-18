using System;
using System.Collections.Generic;

namespace EncuestasApi.Models.Entities;

public partial class Preguntas
{
    public int Id { get; set; }

    public string Texto { get; set; } = null!;

    public int? EncuestaId { get; set; }

    public virtual Encuestas? Encuesta { get; set; }
}
