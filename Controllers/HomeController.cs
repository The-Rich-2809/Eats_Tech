using Eats_Tech.Models;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            Initialize();
            var miCookie = HttpContext.Request.Cookies["Cookie_EatsTech"];

            if (miCookie != null)
            {
                List<Usuario> listaUsuarios = _contextDB.Usuario.ToList();
                foreach (var user in listaUsuarios)
                {
                    if (miCookie == user.Correo)
                    {
                        return RedirectToAction("Index", "Pedido");
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
                    if(user.TipoUsuario == "Admin")
                        return RedirectToAction("Index", "Admin");
                    if (user.TipoUsuario == "Mesa")
                        return RedirectToAction("Index", "Pedido");
                    if (user.TipoUsuario == "Cocina")
                        return RedirectToAction("Index", "Cocina");
                } 
            }
            ViewBag.ErrorMessage = "Correo y/o contrasena incorrectos";
            return View();
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
                new Usuario {Nombre = "Rich", Contrasena = "1234", Correo = "ricardo_138@outlook.com", TipoUsuario = "Admin", DireccionImagen = "h"},
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
                new Menu() { NombrePlatillo = "Taco de Carnitas", Descripcion = "Carne de cerdo cocida lentamente en su propia grasa hasta que queda tierna, generalmente acompañada de cebolla, cilantro y salsa.", Costo = 18.75, Categoria = "Tacos", RutaImagen = "taco_de_carnitas.jpg" },
                new Menu() { NombrePlatillo = "Taco de Barbacoa", Descripcion = "Carne de res, cordero o cabra cocida al vapor o en horno enterrado, desmenuzada y servida con cebolla, cilantro y salsa.", Costo = 22.00, Categoria = "Tacos", RutaImagen = "taco_de_barbacoa.jpg" },
                new Menu() { NombrePlatillo = "Taco de Asada", Descripcion = "Carne de res asada a la parrilla, generalmente acompañada de cebolla, cilantro y salsa.", Costo = 21.00, Categoria = "Tacos", RutaImagen = "taco_de_asada.jpg" },
                new Menu() { NombrePlatillo = "Taco de Cochinita Pibil", Descripcion = "Carne de cerdo marinada en achiote y cítricos, cocida lentamente y servida con cebolla morada encurtida y salsa de habanero.", Costo = 23.50, Categoria = "Tacos", RutaImagen = "taco_de_cochinita_pibil.jpg" },
                new Menu() { NombrePlatillo = "Taco de Pescado", Descripcion = "Filete de pescado rebozado y frito o a la parrilla, servido con col, mayonesa, limón y salsa.", Costo = 25.00, Categoria = "Tacos", RutaImagen = "taco_de_pescado.jpg" },
                new Menu() { NombrePlatillo = "Taco de Camarón", Descripcion = "Camarones cocidos a la parrilla o fritos, servidos con col, mayonesa, limón y salsa.", Costo = 28.00, Categoria = "Tacos", RutaImagen = "taco_de_camaron.jpg" },
                new Menu() { NombrePlatillo = "Taco de Suadero", Descripcion = "Carne de res cocida lentamente, con un sabor y textura únicos, generalmente servida con cebolla, cilantro y salsa.", Costo = 19.50, Categoria = "Tacos", RutaImagen = "taco_de_suadero.jpg" },
                new Menu() { NombrePlatillo = "Taco de Tripa", Descripcion = "Tripa de res bien cocida hasta quedar crujiente, servida con cebolla, cilantro y salsa.", Costo = 17.50, Categoria = "Tacos", RutaImagen = "taco_de_tripa.jpg" },
                new Menu() { NombrePlatillo = "Taco de Lengua", Descripcion = "Lengua de res cocida hasta quedar tierna, desmenuzada y servida con cebolla, cilantro y salsa.", Costo = 19.00, Categoria = "Tacos", RutaImagen = "taco_de_lengua.jpg" },
                new Menu() { NombrePlatillo = "Taco de Birria", Descripcion = "Carne de res o chivo cocida en un caldo especiado, desmenuzada y servida con cebolla, cilantro y salsa.", Costo = 24.00, Categoria = "Tacos", RutaImagen = "taco_de_birria.jpg" },
                new Menu() { NombrePlatillo = "Taco de Chorizo", Descripcion = "Chorizo mexicano frito, servido con cebolla, cilantro y salsa.", Costo = 16.00, Categoria = "Tacos", RutaImagen = "taco_de_chorizo.jpg" },
                new Menu() { NombrePlatillo = "Taco de Pollo", Descripcion = "Pollo cocido o asado, desmenuzado y servido con cebolla, cilantro y salsa.", Costo = 18.00, Categoria = "Tacos", RutaImagen = "taco_de_pollo.jpg" },
                new Menu() { NombrePlatillo = "Taco de Nopales", Descripcion = "Tiras de nopal cocidas, a menudo mezcladas con jitomate, cebolla y chile, servidas con queso fresco y salsa.", Costo = 16.50, Categoria = "Tacos", RutaImagen = "taco_de_nopales.jpg" },
                new Menu() { NombrePlatillo = "Taco de Tinga", Descripcion = "Pollo desmenuzado en una salsa de jitomate, cebolla y chile chipotle, servido con crema y queso fresco.", Costo = 19.50, Categoria = "Tacos", RutaImagen = "taco_de_tinga.jpg" }
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
        }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
