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
                new Usuario {Nombre = "Juan", Contrasena = "1234", Correo = "mesero@eatstech.com", TipoUsuario = "Mesero", Activo = 0, DireccionImagen = "h"},
                new Usuario {Nombre = "Mesa 1", Contrasena = "1234", Correo = "mesa1@eatstech.com", TipoUsuario = "Mesa", Activo = 0, DireccionImagen = "h"},
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
                new Menu() { NombrePlatillo = "Taco al Pastor", Descripcion = "Carne de cerdo marinada en una mezcla de chiles, achiote y especias, cocinada en un trompo vertical y servida con pi�a, cilantro, cebolla y salsa.", Costo = 20.50, Categoria = "Entradas", RutaImagen = "../Images/taco-pastor.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Taco de Carnitas", Descripcion = "Carne de cerdo cocida lentamente en su propia grasa hasta que queda tierna, generalmente acompa�ada de cebolla, cilantro y salsa.", Costo = 18.75, Categoria = "Entradas", RutaImagen = "../Images/tacos-carnitas.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Sushi", Descripcion = "Arroz de sushi acompa�ado de pescado crudo como at�n o salm�n, con aguacate, pepino, y servido con salsa de soja y wasabi.", Costo = 12.50, Categoria = "Entradas", RutaImagen = "../Images/Platillos/sushi.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Paella", Descripcion = "Arroz cocido con azafr�n, mariscos como mejillones y gambas, pollo, chorizo, pimientos y guisantes.", Costo = 18.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/paella.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Pad Thai", Descripcion = "Fideos de arroz salteados con camarones o pollo, huevo, brotes de soja, cacahuetes y salsa de tamarindo.", Costo = 15.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/pad-thai.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Pizza Margherita", Descripcion = "Pizza con una base de masa delgada, salsa de tomate, mozzarella fresca y albahaca.", Costo = 10.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/pizza-margarita.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Croissant", Descripcion = "Panader�a francesa hecho de una masa laminada con mantequilla, formando una textura hojaldrada.", Costo = 3.50, Categoria = "Entradas", RutaImagen = "../Images/Platillos/croissant.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Biryani", Descripcion = "Arroz basmati cocido con pollo o cordero, yogur y una mezcla de especias como comino y c�rcuma.", Costo = 14.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/biryani.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Churrasco", Descripcion = "Carne de res asada a la parrilla, sazonada con sal gruesa y a menudo acompa�ada de ajo y aceite de oliva.", Costo = 22.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/churrasco.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Shakshuka", Descripcion = "Huevos escalfados en una salsa de tomates, pimientos y cebolla, condimentados con comino y piment�n.", Costo = 7.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/shakshuka.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Poutine", Descripcion = "Papas fritas cubiertas con queso en grano y salsa gravy.", Costo = 9.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/poutine.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Ratatouille", Descripcion = "Plato de verduras guisadas como berenjena, calabac�n, pimientos y tomates, sazonadas con hierbas de Provenza.", Costo = 7.50, Categoria = "Entradas", RutaImagen = "../Images/Platillos/ratatouille.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Ceviche", Descripcion = "Pescado fresco marinado en jugo de lim�n, mezclado con cebolla morada, cilantro, aj� y acompa�ado de camote y ma�z.", Costo = 11.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/ceviche.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Kebab", Descripcion = "Carne de cordero o pollo marinada en yogur y especias, cocida a la parrilla y servida en pan pita.", Costo = 8.50, Categoria = "Entradas", RutaImagen = "../Images/Platillos/kebab.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Kimchi", Descripcion = "Col fermentada con r�bano, ajo, jengibre y pimiento rojo en polvo.", Costo = 4.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/kimchi.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Bratwurst", Descripcion = "Salchicha de cerdo a la parrilla, servida en un pan con mostaza y chucrut.", Costo = 6.50, Categoria = "Entradas", RutaImagen = "../Images/Platillos/bratwurst.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Falafel", Descripcion = "Bolitas fritas de garbanzo, ajo, cebolla y especias, servidas en pan pita con salsa de yogur.", Costo = 6.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/falafel.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Pho", Descripcion = "Sopa de fideos de arroz con caldo de ternera, ternera o pollo, hierbas frescas y brotes de soja.", Costo = 11.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/pho.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Gringa de Pastor", Descripcion = "Tortilla de harina rellena de carne de cerdo al pastor, queso derretido, pi�a, cebolla y cilantro, servida con salsa.", Costo = 25.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/gringa-pastor.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Alambre de Res", Descripcion = "Platillo mexicano hecho con carne de res, pimientos, cebolla, tocino y queso, todo salteado y servido con tortillas.", Costo = 22.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/alambre-res.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Barbacoa", Descripcion = "Carne de res cocida lentamente al vapor con hojas de maguey y una mezcla de especias, servida con tortillas, cebolla, cilantro y salsa.", Costo = 20.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/barbacoa.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Torta de Pastor", Descripcion = "Pan bolillo relleno de carne de cerdo al pastor, aguacate, frijoles refritos, cebolla, cilantro y salsa.", Costo = 18.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/torta-pastor.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Taco de Pastor", Descripcion = "Taco de tortilla de ma�z relleno de carne de cerdo al pastor, pi�a, cebolla, cilantro y salsa.", Costo = 15.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/taco-pastor.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Burrito de Cecina", Descripcion = "Tortilla de harina rellena de cecina, arroz, frijoles, guacamole, cebolla, cilantro y salsa.", Costo = 20.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/burrito-cecina.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Taco de Pollo", Descripcion = "Taco de tortilla de ma�z relleno de pollo desmenuzado, cebolla, cilantro y salsa.", Costo = 12.00, Categoria = "Entradas", RutaImagen = "../Images/Platillos/taco-pollo.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Tiramis�", Descripcion = "Postre italiano hecho con capas de bizcocho empapadas en caf�, mascarpone, y cacao en polvo.", Costo = 8.00, Categoria = "Postres", RutaImagen = "../Images/Platillos/tiramisu.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Cheesecake", Descripcion = "Tarta de queso con una base de galleta, rellena de una mezcla cremosa de queso, y cubierta con frutas o mermelada.", Costo = 7.50, Categoria = "Postres", RutaImagen = "../Images/Platillos/cheesecake.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Churros", Descripcion = "Masa frita espolvoreada con az�car y canela, a menudo servida con chocolate caliente para mojar.", Costo = 5.00, Categoria = "Postres", RutaImagen = "../Images/Platillos/churros.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Cr�me Br�l�e", Descripcion = "Postre franc�s de crema de vainilla con una capa superior de az�car caramelizado.", Costo = 9.00, Categoria = "Postres", RutaImagen = "../Images/Platillos/creeme-brulee.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Brownie", Descripcion = "Denso pastel de chocolate, a menudo servido con nueces y una bola de helado.", Costo = 6.00, Categoria = "Postres", RutaImagen = "../Images/Platillos/brownie.jpg", Activo = 1 }


                //new Menu() { NombrePlatillo = "Chow Mein", Descripcion = "Fideos de huevo salteados con pollo o cerdo y verduras como zanahorias, pimientos y cebolla, en salsa de soja.", Costo = 9.50, Categoria = "Comida", RutaImagen = "../Images/chow-mein.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Haggis", Descripcion = "Plato escoc�s hecho de v�sceras de oveja, avena, cebolla y especias, cocido en el est�mago de la oveja.", Costo = 12.00, Categoria = "Comida", RutaImagen = "../Images/haggis.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Lomo Saltado", Descripcion = "Salteado de carne de res con papas fritas, cebolla, tomate y aj�, sazonado con salsa de soja y cilantro.", Costo = 10.50, Categoria = "Comida", RutaImagen = "../Images/lomo-saltado.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Baklava", Descripcion = "Dulce hecho de capas de pasta filo rellenas de nueces y ba�adas en miel.", Costo = 4.50, Categoria = "Comida", RutaImagen = "../Images/baklava.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Jambalaya", Descripcion = "Arroz con camarones, pollo, salchicha andouille, pimientos, cebolla, apio y tomate, sazonado con especias caj�n.", Costo = 13.00, Categoria = "Comida", RutaImagen = "../Images/jambalaya.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Sauerbraten", Descripcion = "Carne de res marinada en vinagre y vino tinto con cebolla, zanahorias y especias, cocida a fuego lento.", Costo = 16.00, Categoria = "Comida", RutaImagen = "../Images/sauerbraten.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Moussaka", Descripcion = "Plato griego de berenjena, carne de cordero o res, tomate, cebolla y bechamel, gratinado con queso.", Costo = 11.50, Categoria = "Comida", RutaImagen = "../Images/moussaka.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Bobotie", Descripcion = "Plato sudafricano de carne molida con cebolla, especias y frutas secas, cubierto con una mezcla de pan, leche y huevo.", Costo = 9.00, Categoria = "Comida", RutaImagen = "../Images/bobotie.jpg", Activo = 1 }
            };
            var insertarBebida = new Menu[]
            {
                new Menu() { NombrePlatillo = "Mojito", Descripcion = "Coctel cubano hecho con ron blanco, az�car, lima, menta y agua con gas.", Costo = 8.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/mojito.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Caipirinha", Descripcion = "Coctel brasile�o hecho con cacha�a, az�car y lima.", Costo = 7.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/caipirinha.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Sake", Descripcion = "Bebida alcoh�lica japonesa hecha de arroz fermentado.", Costo = 5.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/sake.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Tequila Sunrise", Descripcion = "Coctel mexicano hecho con tequila, jugo de naranja y granadina.", Costo = 7.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/tequila-sunrise.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Pi�a Colada", Descripcion = "Coctel puertorrique�o hecho con ron, crema de coco y jugo de pi�a.", Costo = 9.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/pina-colada.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Margarita", Descripcion = "Coctel mexicano hecho con tequila, jugo de lima y Cointreau o triple sec.", Costo = 8.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/margarita.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Irish Coffee", Descripcion = "Caf� caliente mezclado con whisky irland�s, az�car y cubierto con crema.", Costo = 6.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/irish-coffee.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Chai Latte", Descripcion = "Bebida india hecha con t� negro, especias, leche y az�car.", Costo = 4.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/chai-latte.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Lassi", Descripcion = "Bebida india hecha con yogur, agua, especias y a veces frutas como mango.", Costo = 4.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/lassi.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Mate", Descripcion = "Infusi�n tradicional sudamericana hecha de hojas de yerba mate.", Costo = 3.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/mate.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Sangr�a", Descripcion = "Bebida espa�ola hecha con vino tinto, frutas troceadas, az�car y un toque de brandy.", Costo = 6.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/sangria.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Limoncello", Descripcion = "Licor italiano de lim�n servido fr�o como digestivo.", Costo = 5.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/limoncello.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Pisco Sour", Descripcion = "Coctel peruano hecho con pisco, jugo de lim�n, jarabe de goma, clara de huevo y amargo de angostura.", Costo = 8.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/pisco-sour.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Aperol Spritz", Descripcion = "Coctel italiano hecho con Aperol, prosecco y agua con gas.", Costo = 7.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/aperol-spritz.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Cosmopolitan", Descripcion = "Coctel hecho con vodka, Cointreau, jugo de ar�ndano y jugo de lima.", Costo = 9.00, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/cosmopolitan.jpg", Activo = 1 },
                new Menu() { NombrePlatillo = "Bellini", Descripcion = "Coctel italiano hecho con prosecco y pur� de durazno.", Costo = 7.50, Categoria = "Bebidas", RutaImagen = "../Images/Bebidas/bellini.jpg", Activo = 1 }

                //new Menu() { NombrePlatillo = "Mai Tai", Descripcion = "Coctel hawaiano hecho con ron blanco y oscuro, jugo de lima, cura�ao de naranja y jarabe de orgeat.", Costo = 8.00, Categoria = "Bebidas", RutaImagen = "../Images/mai-tai.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Hot Toddy", Descripcion = "Bebida caliente hecha con whisky, miel, lim�n y agua caliente.", Costo = 5.00, Categoria = "Bebidas", RutaImagen = "../Images/hot-toddy.jpg", Activo = 1 },
                //new Menu() { NombrePlatillo = "Mimosa", Descripcion = "Coctel hecho con partes iguales de champ�n y jugo de naranja.", Costo = 6.00, Categoria = "Bebidas", RutaImagen = "../Images/mimosa.jpg", Activo = 1 },
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
