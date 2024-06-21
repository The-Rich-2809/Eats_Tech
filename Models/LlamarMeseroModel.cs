using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eats_Tech.Models
{
    public class LlamarMeseroModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdMesero { get; set; }
        public int IdMesa { get; set; }
        public int Activo { get; set; }
        public string Nombre { get; set; }
    }
}
