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
        public void Cookies()
        {
            var miCookie = HttpContext.Request.Cookies["MiCookie"];

            if (miCookie != null)
            {
                List<Usuario> listaUsuarios = _contextDB.Usuario.ToList();
                foreach (var user in listaUsuarios)
                {
                    if (miCookie == user.Correo)
                    {
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
            menu.First().Categoria = Categoria;
            ViewBag.Menu = menu;
            return View();
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
            return View();
        }

    }
}
