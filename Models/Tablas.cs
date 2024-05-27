namespace Eats_Tech.Models
{
    public class Tablas
    {
        public IEnumerable<Cliente> Cliente { get; set; }
        public IEnumerable<Usuario> Usuario { get; set; }
        public IEnumerable<Menu> Menu { get; set; }
        public IEnumerable<Orden> Orden { get; set; }

    }
}
