using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Eats_Tech.Models
{
    public class Comentario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int Calificacion { get; set; }
        public string Comentarios { get; set; }
        public DateTime DateTime { get; set; }
        public int Activo { get; set; }
    }
}
