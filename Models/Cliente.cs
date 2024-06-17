using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Eats_Tech.Models
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public int IdMesa { get; set; }
        public double PrecioFinal { get; set; }
        public double Propina { get; set; }
        public int MetodoPago { get; set; }
        public DateTime Hora { get; set; }
        public string Status { get; set; }
    }
}
