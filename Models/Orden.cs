using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Eats_Tech.Models
{
    public class Orden
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdMenu { get; set; }
        public int Cantidad { get; set; }
        public double Costo { get; set; }
        public int IdCliente { get; set; }
        public string Status { get; set; }


    }
}
