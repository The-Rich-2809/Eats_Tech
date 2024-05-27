using Eats_Tech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Eats_Tech.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Eats_TechDB _contextDB;

        public HomeController(ILogger<HomeController> logger, Eats_TechDB contextDB)
        {
            _logger = logger;
            _contextDB = contextDB;
        }
        public static string Correo {  get; set; }
        public IActionResult Index()
        {
            Initialize();
            var miCookie = HttpContext.Request.Cookies["Cookie_EatsTech"];

            if (miCookie != null)
            {
                List<Usuario> listaUsuarios = _contextDB.Usuario.ToList();
                List<Cliente> listaClientes = _contextDB.Cliente.ToList();
                foreach (var user in listaUsuarios)
                {
                    if (miCookie == user.Correo)
                    {
                        Correo = user.Correo;

                        foreach (var item in listaClientes)
                        {
                            if (item.IdMesa == user.ID && item.Status == "Empezando")
                            {
                                return RedirectToAction("Orden", "Pedido");
                            }
                        }

                        if (user.TipoUsuario == "Admin")
                            return RedirectToAction("Index", "Admin");
                        if (user.TipoUsuario == "Mesa")
                            return RedirectToAction("Index", "Pedido");
                        if (user.TipoUsuario == "Cocina")
                            return RedirectToAction("Index", "Cocina");
                    }
                }
            }
            return View();
        }
        [HttpPost]
        public IActionResult Index(string Correo, string Contrasena)
        {
            List<Usuario> listaUsuarios = _contextDB.Usuario.ToList();
            foreach(var user in listaUsuarios)
            {
                if(user.Correo == Correo && user.Contrasena == Contrasena)
                {
                    if(user.Activo != 1)
                    {
                        if(user.Activo != 777)
                        {
                            var u = _contextDB.Usuario.FirstOrDefault(e => e.Correo == user.Correo);
                            u.Activo = 1;
                            _contextDB.Entry(u).State = EntityState.Modified; ;
                            _contextDB.SaveChanges();

                            CookieOptions options = new CookieOptions();
                            options.Expires = DateTime.Now.AddDays(365);
                            options.IsEssential = true;
                            options.Path = "/";
                            HttpContext.Response.Cookies.Append("Cookie_EatsTech", Correo, options);

                            if (user.TipoUsuario == "Admin")
                                return RedirectToAction("Index", "Admin");
                            if (user.TipoUsuario == "Mesa")
                                return RedirectToAction("Index", "Pedido");
                            if (user.TipoUsuario == "Cocina")
                                return RedirectToAction("Index", "Cocina");
                        }
                        ViewBag.ErrorMessage = "Esta mesa ya esta no esta activa";
                        break;
                    }
                    ViewBag.ErrorMessage = "Este usuario ya esta activo";
                    break;
                } 
            }
            ViewBag.ErrorMessage = "Correo y/o contrasena incorrectos";
            return View();
        }
        [HttpGet]
        public IActionResult CerrarSesion()
        {
            var u = _contextDB.Usuario.FirstOrDefault(e => e.Correo == Correo);
            u.Activo = 0;
            _contextDB.Entry(u).State = EntityState.Modified; ;
            _contextDB.SaveChanges();
            HttpContext.Response.Cookies.Delete("Cookie_EatsTech");
            return RedirectToAction("Index");
        }
        public void Initialize()
        {
            _contextDB.Database.EnsureCreated();

            if (_contextDB.Usuario.Any())
            {
                return;
            }

            var insertarusuario = new Usuario[]
            {
                new Usuario {Nombre = "Rich", Contrasena = "1234", Correo = "ricardo_138@outlook.com", TipoUsuario = "Admin", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 1", Contrasena = "1234", Correo = "mesa1@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"}
            };

            var insertarcategoria = new Categoria[]
            {
                new Categoria(){NombreCategoria = "Tacos"},
                new Categoria(){NombreCategoria = "Bebidas"}
            };

            var insetarplatillo = new Menu[]
            {
                new Menu() { NombrePlatillo = "Taco al Pastor", Descripcion = "Carne de cerdo marinada en una mezcla de chiles, achiote y especias, cocinada en un trompo vertical y servida con piña, cilantro, cebolla y salsa.", Costo = 20.50, Categoria = "Tacos", RutaImagen = "../Images/taco-pastor.jpg" },
                new Menu() { NombrePlatillo = "Taco de Carnitas", Descripcion = "Carne de cerdo cocida lentamente en su propia grasa hasta que queda tierna, generalmente acompañada de cebolla, cilantro y salsa.", Costo = 18.75, Categoria = "Tacos", RutaImagen = "../Images/tacos-carnitas.jpg" },
                new Menu() { NombrePlatillo = "Taco de Barbacoa", Descripcion = "Carne de res, cordero o cabra cocida al vapor o en horno enterrado, desmenuzada y servida con cebolla, cilantro y salsa.", Costo = 22.00, Categoria = "Tacos", RutaImagen = "../Images/tacos-de-barbacoa.jpg" },
            };
            var insertarBebida = new Menu[]
            {
                new Menu(){NombrePlatillo = "Coca Cola", Descripcion = "Una cocona bien elodia de 600 ml", Costo = 20, Categoria = "Bebidas", RutaImagen = "../Images/Cocacola.png"},
                new Menu(){NombrePlatillo = "Caguama", Descripcion = "Un caguamon bien muerta",  Costo = 60, Categoria = "Bebidas", RutaImagen = "../Images/caguamon.jpg"},
                new Menu(){NombrePlatillo = "Boing Mango", Descripcion = "Un boing de mango", Costo = 15, Categoria = "Bebidas", RutaImagen = "../Images/BoingMango.png"}
            };

            foreach(var u in insertarusuario)
                _contextDB.Usuario.Add(u);
            _contextDB.SaveChanges();

            foreach(var u in insertarcategoria)
                _contextDB.Categoria.Add(u);
            _contextDB.SaveChanges();

            foreach (var u in insetarplatillo)
                _contextDB.Menu.Add(u);
            _contextDB.SaveChanges();

            foreach(var u in insertarBebida)
                _contextDB.Menu.Add(u);
            _contextDB.SaveChanges();
        }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
