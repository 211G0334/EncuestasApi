using System;
using System.Collections.Generic;

namespace EncuestasApi.Models.Entities;

public partial class Respuestas
{
    public int Id { get; set; }

    public int? EncuestaId { get; set; }

    public int Escala { get; set; }

    public int PreguntaId { get; set; }

    public virtual Encuestas? Encuesta { get; set; }
}
