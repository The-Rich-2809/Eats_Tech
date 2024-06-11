using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Eats_Tech.Models
{
    public class Menu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string NombrePlatillo {  get; set; }
        public string Descripcion { get; set; }
        public double Costo {  get; set; }
        public string Categoria { get; set; }
        public string RutaImagen { get; set; }
        public int Activo { get; set; }
    }
}
