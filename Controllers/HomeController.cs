using Eats_Tech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

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
        [HttpGet]
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
                                return RedirectToAction("Orden", "Pedido");
                            if (item.IdMesa == user.ID && (item.Status != "Empezando" && item.Status != "Terminada") )
                                return RedirectToAction("Comiendo", "Pedido");

                        }

                        if (user.TipoUsuario == "Admin")
                            return RedirectToAction("Index", "Admin");
                        if (user.TipoUsuario == "Mesa")
                            return RedirectToAction("Index", "Pedido");
                        if (user.TipoUsuario == "Cocina")
                            return RedirectToAction("Index", "Cocina");
                        if (user.TipoUsuario == "Mesero")
                            return RedirectToAction("Index", "Mesero");
                        if (user.TipoUsuario == "Recepcion")
                            return RedirectToAction("Index", "Recepcion");
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

                            return RedirectToAction("Index");

                        }
                        ViewBag.ErrorMessage = "Esta mesa ya esta no esta activa";
                        return View();
                    }
                    ViewBag.ErrorMessage = "Este usuario ya esta activo";
                    return View();
                } 
            }
            ViewBag.ErrorMessage = "Correo y/o contrasena incorrectos";
            return View();
        }
        [HttpGet]
        public IActionResult CerrarSesion()
        {
            Correo = Cookies();
            var u = _contextDB.Usuario.FirstOrDefault(e => e.Correo == Correo);
            u.Activo = 0;
            _contextDB.Entry(u).State = EntityState.Modified; ;
            _contextDB.SaveChanges();
            HttpContext.Response.Cookies.Delete("Cookie_EatsTech");
            return RedirectToAction("Index");
        }
        public string Cookies()
        {
            var miCookie = HttpContext.Request.Cookies["Cookie_EatsTech"];

            if (miCookie != null)
            {
                List<Usuario> listaUsuarios = _contextDB.Usuario.ToList();
                foreach (var user in listaUsuarios)
                {
                    if (miCookie == user.Correo)
                    {
                        return user.Correo;
                    }
                }
            }
            return "";
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
                new Usuario {Nombre = "Carlos", Contrasena = "1234", Correo = "camacario01@gmail.com", TipoUsuario = "Admin", Activo = 0, DireccionImagen = "../Images/Users/maca.jpg"},
                new Usuario {Nombre = "Ale", Contrasena = "1234", Correo = "cocinero@eatstech.com", TipoUsuario = "Cocina", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Virginia", Contrasena = "1234", Correo = "cocinero1@eatstech.com", TipoUsuario = "Cocina", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Sandra", Contrasena = "1234", Correo = "cocinero2@eatstech.com", TipoUsuario = "Cocina", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Juan", Contrasena = "1234", Correo = "mesero@eatstech.com", TipoUsuario = "Mesero", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Erik", Contrasena = "1234", Correo = "mesero1@eatstech.com", TipoUsuario = "Mesero", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Alan", Contrasena = "1234", Correo = "mesero2@eatstech.com", TipoUsuario = "Mesero", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Jose", Contrasena = "1234", Correo = "mesero3@eatstech.com", TipoUsuario = "Mesero", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 1", Contrasena = "1234", Correo = "mesa1@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 2", Contrasena = "1234", Correo = "mesa2@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 3", Contrasena = "1234", Correo = "mesa3@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 4", Contrasena = "1234", Correo = "mesa4@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 5", Contrasena = "1234", Correo = "mesa5@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 6", Contrasena = "1234", Correo = "mesa6@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 7", Contrasena = "1234", Correo = "mesa7@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 8", Contrasena = "1234", Correo = "mesa8@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 9", Contrasena = "1234", Correo = "mesa9@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 10", Contrasena = "1234", Correo = "mesa10@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 11", Contrasena = "1234", Correo = "mesa11@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 12", Contrasena = "1234", Correo = "mesa12@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 13", Contrasena = "1234", Correo = "mesa13@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 14", Contrasena = "1234", Correo = "mesa14@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 15", Contrasena = "1234", Correo = "mesa15@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 16", Contrasena = "1234", Correo = "mesa16@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 17", Contrasena = "1234", Correo = "mesa17@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Karen Macho", Contrasena = "1234", Correo = "recepcion1@eatstech.com", TipoUsuario = "Recepcion", Activo = 0, DireccionImagen = "h"}
            };

            var insertarcategoria = new Categoria[]
            {
                new Categoria(){NombreCategoria = "Entradas", Activo = 1},
                new Categoria(){NombreCategoria = "Postres", Activo = 1},
                new Categoria(){NombreCategoria = "Bebidas", Activo = 1}
            };

            var insetarplatillo = new Menu[]
            {
                new Menu() { NombrePlatillo = "Taco al Pastor", Descripcion = "Carne de cerdo marinada en una mezcla de chiles, achiote y especias, cocinada en un trompo vertical y servida con piña, cilantro, cebolla y salsa.", Costo = 20.50, Categoria = "Entradas", RutaImagen = "../Images/taco-pastor.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Taco de Carnitas", Descripcion = "Carne de cerdo cocida lentamente en su propia grasa hasta que queda tierna, generalmente acompañada de cebolla, cilantro y salsa.", Costo = 18.75, Categoria = "Entradas", RutaImagen = "../Images/tacos-carnitas.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Gringa de Pastor", Descripcion = "Tortilla de harina rellena de carne de cerdo al pastor, queso derretido, piña, cebolla y cilantro, servida con salsa.", Costo = 25.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/gringa-pastor.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Alambre de Res", Descripcion = "Platillo mexicano hecho con carne de res, pimientos, cebolla, tocino y queso, todo salteado y servido con tortillas.", Costo = 22.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/alambre-res.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Barbacoa", Descripcion = "Carne de res cocida lentamente al vapor con hojas de maguey y una mezcla de especias, servida con tortillas, cebolla, cilantro y salsa.", Costo = 20.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/barbacoa.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Torta de Pastor", Descripcion = "Pan bolillo relleno de carne de cerdo al pastor, aguacate, frijoles refritos, cebolla, cilantro y salsa.", Costo = 18.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/torta-pastor.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Taco de Pastor", Descripcion = "Taco de tortilla de maíz relleno de carne de cerdo al pastor, piña, cebolla, cilantro y salsa.", Costo = 15.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/taco-pastor.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Burrito de Cecina", Descripcion = "Tortilla de harina rellena de cecina, arroz, frijoles, guacamole, cebolla, cilantro y salsa.", Costo = 20.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/burrito-cecina.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Taco de Pollo", Descripcion = "Taco de tortilla de maíz relleno de pollo desmenuzado, cebolla, cilantro y salsa.", Costo = 12.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/taco-pollo.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Tiramisú", Descripcion = "Postre italiano hecho con capas de bizcocho empapadas en café, mascarpone, y cacao en polvo.", Costo = 8.00, Categoria = "Postres", RutaImagen = "../Images/Platillos/tiramisu.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Cheesecake", Descripcion = "Tarta de queso con una base de galleta, rellena de una mezcla cremosa de queso, y cubierta con frutas o mermelada.", Costo = 7.50, Categoria = "Postres", RutaImagen = "../Images/Platillos/cheesecake.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Churros", Descripcion = "Masa frita espolvoreada con azúcar y canela, a menudo servida con chocolate caliente para mojar.", Costo = 5.00, Categoria = "Postres", RutaImagen = "../Images/Platillos/churros.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Crème Brûlée", Descripcion = "Postre francés de crema de vainilla con una capa superior de azúcar caramelizado.", Costo = 9.00, Categoria = "Postres", RutaImagen = "../Images/Platillos/creeme-brulee.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Brownie", Descripcion = "Denso pastel de chocolate, a menudo servido con nueces y una bola de helado.", Costo = 6.00, Categoria = "Postres", RutaImagen = "../Images/Platillos/brownie.jpg", Activo = 1 }


                //new Menu() { NombrePlatillo = "Chow Mein", Descripcion = "Fideos de huevo salteados con pollo o cerdo y verduras como zanahorias, pimientos y cebolla, en salsa de soja.", Costo = 9.50, Categoria = "Comida", RutaImagen = "../Images/chow-mein.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Haggis", Descripcion = "Plato escocés hecho de vísceras de oveja, avena, cebolla y especias, cocido en el estómago de la oveja.", Costo = 12.00, Categoria = "Comida", RutaImagen = "../Images/haggis.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Lomo Saltado", Descripcion = "Salteado de carne de res con papas fritas, cebolla, tomate y ají, sazonado con salsa de soja y cilantro.", Costo = 10.50, Categoria = "Comida", RutaImagen = "../Images/lomo-saltado.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Baklava", Descripcion = "Dulce hecho de capas de pasta filo rellenas de nueces y bañadas en miel.", Costo = 4.50, Categoria = "Comida", RutaImagen = "../Images/baklava.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Jambalaya", Descripcion = "Arroz con camarones, pollo, salchicha andouille, pimientos, cebolla, apio y tomate, sazonado con especias cajún.", Costo = 13.00, Categoria = "Comida", RutaImagen = "../Images/jambalaya.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Sauerbraten", Descripcion = "Carne de res marinada en vinagre y vino tinto con cebolla, zanahorias y especias, cocida a fuego lento.", Costo = 16.00, Categoria = "Comida", RutaImagen = "../Images/sauerbraten.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Moussaka", Descripcion = "Plato griego de berenjena, carne de cordero o res, tomate, cebolla y bechamel, gratinado con queso.", Costo = 11.50, Categoria = "Comida", RutaImagen = "../Images/moussaka.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Bobotie", Descripcion = "Plato sudafricano de carne molida con cebolla, especias y frutas secas, cubierto con una mezcla de pan, leche y huevo.", Costo = 9.00, Categoria = "Comida", RutaImagen = "../Images/bobotie.jpg", Activo = 1 }
            };
            var insertarBebida = new Menu[]
            {
                new Menu() { NombrePlatillo = "Mojito", Descripcion = "Coctel cubano hecho con ron blanco, azúcar, lima, menta y agua con gas.", Costo = 8.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/mojito.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Caipirinha", Descripcion = "Coctel brasileño hecho con cachaça, azúcar y lima.", Costo = 7.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/caipirinha.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Sake", Descripcion = "Bebida alcohólica japonesa hecha de arroz fermentado.", Costo = 5.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/sake.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Tequila Sunrise", Descripcion = "Coctel mexicano hecho con tequila, jugo de naranja y granadina.", Costo = 7.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/tequila-sunrise.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Piña Colada", Descripcion = "Coctel puertorriqueño hecho con ron, crema de coco y jugo de piña.", Costo = 9.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/pina-colada.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Margarita", Descripcion = "Coctel mexicano hecho con tequila, jugo de lima y Cointreau o triple sec.", Costo = 8.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/margarita.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Irish Coffee", Descripcion = "Café caliente mezclado con whisky irlandés, azúcar y cubierto con crema.", Costo = 6.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/irish-coffee.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Chai Latte", Descripcion = "Bebida india hecha con té negro, especias, leche y azúcar.", Costo = 4.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/chai-latte.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Lassi", Descripcion = "Bebida india hecha con yogur, agua, especias y a veces frutas como mango.", Costo = 4.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/lassi.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Mate", Descripcion = "Infusión tradicional sudamericana hecha de hojas de yerba mate.", Costo = 3.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/mate.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Sangría", Descripcion = "Bebida española hecha con vino tinto, frutas troceadas, azúcar y un toque de brandy.", Costo = 6.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/sangria.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Limoncello", Descripcion = "Licor italiano de limón servido frío como digestivo.", Costo = 5.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/limoncello.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Pisco Sour", Descripcion = "Coctel peruano hecho con pisco, jugo de limón, jarabe de goma, clara de huevo y amargo de angostura.", Costo = 8.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/pisco-sour.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Aperol Spritz", Descripcion = "Coctel italiano hecho con Aperol, prosecco y agua con gas.", Costo = 7.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/aperol-spritz.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Cosmopolitan", Descripcion = "Coctel hecho con vodka, Cointreau, jugo de arándano y jugo de lima.", Costo = 9.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/cosmopolitan.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Bellini", Descripcion = "Coctel italiano hecho con prosecco y puré de durazno.", Costo = 7.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/bellini.jpg", Activo = 1 }

                //new Menu() { NombrePlatillo = "Mai Tai", Descripcion = "Coctel hawaiano hecho con ron blanco y oscuro, jugo de lima, curaçao de naranja y jarabe de orgeat.", Costo = 8.00, Categoria = "Bebidas", RutaImagen = "../Images/mai-tai.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Hot Toddy", Descripcion = "Bebida caliente hecha con whisky, miel, limón y agua caliente.", Costo = 5.00, Categoria = "Bebidas", RutaImagen = "../Images/hot-toddy.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Mimosa", Descripcion = "Coctel hecho con partes iguales de champán y jugo de naranja.", Costo = 6.00, Categoria = "Bebidas", RutaImagen = "../Images/mimosa.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Horchata", Descripcion = "Bebida tradicional mexicana hecha de arroz, leche, vainilla y canela.", Costo = 3.50, Categoria = "Bebidas", RutaImagen = "../Images/horchata.jpg", Activo = 1 }
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
