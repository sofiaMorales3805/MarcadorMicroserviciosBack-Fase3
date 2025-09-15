using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarcadorFaseIIApi.Models;

public class Jugador
{
    [Key] // <-- Esto lo marca como PK
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // autoincremental
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    public int Puntos { get; set; }
    public int Faltas { get; set; }

    // FK con Equipo
    public int EquipoId { get; set; }
    public Equipo Equipo { get; set; } = null!;
}
