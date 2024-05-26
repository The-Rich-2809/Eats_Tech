using Microsoft.AspNetCore.Mvc;
using Eats_Tech.Models;

using Eats_Tech.Models;

namespace Eats_Tech.Controllers
{
    public class AdminController : Controller
    {
        private readonly Eats_TechDB _contextDB;
        public static string CorreoS { get; set;}

        public AdminController( Eats_TechDB contextDB)
        {
            _contextDB = contextDB;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Usuarios()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Mesas()
        {
            List<Usuario> usuarios = _contextDB.Usuario.ToList();
            return View(usuarios);
        }
        [HttpGet]
        public IActionResult AgregarMesas()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AgregarMesas(string Mesa, string Contrasena, string Contrasena2)
        {
            AdminModel admin = new AdminModel(_contextDB);
            admin.Mesa = Mesa;
            admin.Contrasena2 = Contrasena2;
            admin.Contrasena = Contrasena;
            if(admin.RegisterMesa())
                return RedirectToAction("Mesas");
            ViewBag.Mensaje = AdminModel.Mensaje;
            return View();
        }
        [HttpGet]
        public IActionResult ModMesas(string Correo)
        {
            CorreoS = Correo;
            return View();
        }
        [HttpPost]
        public IActionResult ModMesas(string Contrasena, string Contrasena2)
        {
            AdminModel admin = new AdminModel(_contextDB);
            admin.Contrasena2 = Contrasena2;
            admin.Contrasena = Contrasena;
            admin.Correo = CorreoS;
            if (admin.ModMesa())
                return RedirectToAction("Mesas");
            ViewBag.Mensaje = AdminModel.Mensaje;
            return View();
        }
        [HttpGet]
        public IActionResult EliMesas(string Correo)
        {
            CorreoS = Correo;
            var Usuario = _contextDB.Usuario.FirstOrDefault(c => c.Correo == Correo);
            return View(Usuario);
        }
        [HttpPost]
        public IActionResult EliMesas()
        {
            AdminModel admin = new AdminModel(_contextDB);
            admin.Correo = CorreoS;
            admin.EliMesa();
            return RedirectToAction("Mesas");
        }
        [HttpGet]
        public IActionResult Clientes()
        {
            List<Usuario> usuarios = _contextDB.Usuario.ToList();
            List<Cliente> clientes = _contextDB.Cliente.ToList();

            var viewmodel = new Tablas
            {
                Usuario = usuarios,
                Cliente = clientes
            };
            return View(viewmodel);
        }
        [HttpGet]
        public IActionResult Ordenes()
        {
            return View();
        }

    }
}
