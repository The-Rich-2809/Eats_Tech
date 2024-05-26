using Eats_Tech.Models;
using Microsoft.AspNetCore.Mvc;

namespace Eats_Tech.Controllers
{
    public class PedidoController : Controller
    {
        private readonly Eats_TechDB _contextDB;
        public PedidoController(Eats_TechDB contextDB)
        {
            _contextDB = contextDB;
        }
        public int IdMesa { get; set; }
        public void Cookies()
        {
            var miCookie = HttpContext.Request.Cookies["Cookie_EatsTech"];

            if (miCookie != null)
            {
                List<Usuario> listaUsuarios = _contextDB.Usuario.ToList();
                foreach (var user in listaUsuarios)
                {
                    if (miCookie == user.Correo)
                    {
                        IdMesa = user.ID;
                        ViewBag.Nombre = user.Nombre;
                        ViewBag.Nivel = user.TipoUsuario;
                        ViewBag.FotoPerfil = user.DireccionImagen;
                    }
                }
            }
            List<Categoria> categorias  = _contextDB.Categoria.ToList();
            ViewBag.Categorias = categorias;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registro(string Nombre)
        {
            Cookies();
            var insetarCliente = new Cliente[]
            {
                new Cliente {Nombre = Nombre, IdMesa = IdMesa, Status = "Empezando", PrecioFinal = 0, Hora = DateTime.Now}
            };
            foreach(var item in insetarCliente)
                _contextDB.Add(item);
            _contextDB.SaveChanges();
            return RedirectToAction("Home", "Pedido");
        }
        [HttpGet]
        public IActionResult Home()
        {
            Cookies();
            return View();
        }
        [HttpGet]
        public IActionResult Menu(string Categoria)
        {
            Cookies();
            List<Menu> menu = _contextDB.Menu.ToList();
            ViewBag.Categoria = Categoria;
            return View(menu);
        }
        [HttpGet]
        public IActionResult Platillo(int Platillo)
        {
            Cookies();
            List<Menu> menu = _contextDB.Menu.ToList();
            foreach (Menu item in menu)
            {
                if(item.Id == Platillo)
                {
                    ViewBag.Nombre = item.NombrePlatillo;
                    ViewBag.Descripcion = item.Descripcion;
                    ViewBag.Costo = item.Costo;
                    ViewBag.Imagen = item.RutaImagen;
                    break;
                }
            }

            return View();
        }
        [HttpGet]
        public IActionResult Orden()
        {
            Cookies();
            return View();
        }

    }
}
