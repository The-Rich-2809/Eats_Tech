using Eats_Tech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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
        public static int IdM {  get; set; }
        public static int IdPlatillo {  get; set; }
        public static int IdCliente { get; set; }
        public static double Total { get; set; }
        public static int IdMesero { get; set; }
        public static string NombreMesa { get; set; }
        public void Cookies()
        {
            List<Orden> listaOrden = _contextDB.Orden.ToList();
            var miCookie = HttpContext.Request.Cookies["Cookie_EatsTech"];

            if (miCookie != null)
            {
                List<Usuario> listaUsuarios = _contextDB.Usuario.ToList();
                foreach (var user in listaUsuarios)
                {
                    if (miCookie == user.Correo)
                    {
                        IdM = user.ID;
                        IdMesa = user.ID;
                        NombreMesa = user.Nombre;
                        ViewBag.Mesa = user.Nombre;
                        ViewBag.Nombre = user.Nombre;
                        ViewBag.Nivel = user.TipoUsuario;
                        ViewBag.FotoPerfil = user.DireccionImagen;
                        break;
                    }
                }
            }
            List<Cliente> listaCliente = _contextDB.Cliente.ToList();
            foreach(var cliente in listaCliente)
            {
                if (cliente.IdMesa == IdM && cliente.Status != "Terminada")
                {
                    IdCliente = cliente.Id;
                    ViewBag.NombreCliente = cliente.Nombre;

                    Total = 0;
                    foreach(var orden in listaOrden)
                    {
                        if(orden.IdCliente == IdCliente)
                        {
                            Total += orden.Costo;
                        }
                    }
                    ViewBag.Total = Total;
                    break;
                }
            }
            List<Categoria> categorias  = _contextDB.Categoria.ToList();
            ViewBag.Categorias = categorias;


        }
        public IActionResult Index()
        {
            Cookies();
            List<Cliente> clientes  = _contextDB.Cliente.ToList();
            foreach(Cliente cliente in clientes)
            {
                if(cliente.IdMesa == IdM && cliente.Status == "Empezando")
                {
                    return RedirectToAction("Orden", "Pedido");
                }
            }

            return View();
        }
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registro(string Nombre, string Correo)
        {
            Cookies();
            var insetarCliente = new Cliente[]
            {
                new Cliente {Nombre = Nombre, Correo = Correo, IdMesa = IdMesa, Status = "Empezando", PrecioFinal = 0, Hora = DateTime.Now, Comentarios = ""}
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
            List<Menu> menu = _contextDB.Menu.ToList();

            Random random = new Random();
            ViewBag.R1 = random.Next(1, menu.Count);

            do
            {
                ViewBag.R2 = random.Next(1, menu.Count);
            } while(ViewBag.R1 == ViewBag.R2);

            do
            {
                ViewBag.R3 = random.Next(1, menu.Count);
            } while (ViewBag.R1 == ViewBag.R3 || ViewBag.R2 == ViewBag.R3);

            return View(menu);
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
                    IdPlatillo = item.Id;
                    ViewBag.Nombre = item.NombrePlatillo;
                    ViewBag.Descripcion = item.Descripcion;
                    ViewBag.Costo = item.Costo;
                    ViewBag.Imagen = item.RutaImagen;
                    break;
                }
            }
            return View();
        }
        [HttpPost]
        public IActionResult Platillo(string Cantidad)
        {
            Cookies();
            int Can = Convert.ToInt32(Cantidad);
            double Total = 0;
            double costo = 0;


            List<Orden> orden = _contextDB.Orden.ToList();
            foreach(Orden o in orden)
            {
                if (o.IdCliente == IdCliente && o.IdMenu == IdPlatillo && o.Status == "Por pedir")
                {
                    var u = _contextDB.Orden.FirstOrDefault(o => o.IdCliente == IdCliente && o.IdMenu == IdPlatillo && o.Status == "Por pedir");
                    costo = o.Costo / o.Cantidad;
                    Can += u.Cantidad;
                    u.Cantidad = Can;
                    u.Costo = u.Cantidad * costo;
                    _contextDB.Entry(u).State = EntityState.Modified; ;
                    _contextDB.SaveChanges();
                    return RedirectToAction("Orden");
                }
            }


            List<Menu> menu = _contextDB.Menu.ToList();
            foreach (Menu item in menu)
            {
                if (item.Id == IdPlatillo)
                {
                    Total = item.Costo * Can;
                    var insertarOrden = new Orden[]
                    {
                        new Orden {IdCliente = IdCliente, IdMenu = IdPlatillo, Cantidad = Can ,Costo = Total, Status = "Por pedir"}
                    };
                    foreach (var u in insertarOrden)
                        _contextDB.Orden.Add(u);
                    _contextDB.SaveChanges();
                }
            }
            return RedirectToAction("Orden");
        }

        [HttpGet]
        public IActionResult Orden()
        {
            Cookies();

            ViewBag.IdCliente = IdCliente;
            List<Usuario> usuarios = _contextDB.Usuario.ToList();
            List<Cliente> clientes = _contextDB.Cliente.ToList();
            List<Menu> menu = _contextDB.Menu.ToList();
            List<Orden> orden = _contextDB.Orden.ToList();


            var viewmodel = new Tablas
            {
                Orden = orden,
                Menu = menu,
                Usuario = usuarios,
                Cliente = clientes
            };
            return View(viewmodel);
        }
        [HttpPost]
        public IActionResult Orden(double Total)
        {
            Cookies();
            List<Usuario> usuarios = _contextDB.Usuario.ToList();
            List<Cliente> clientes = _contextDB.Cliente.ToList();
            List<Menu> menu = _contextDB.Menu.ToList();
            List<Orden> orden = _contextDB.Orden.ToList();

            foreach (var i in orden)
            {
                if(i.IdCliente == IdCliente)
                {
                    foreach(var m in menu)
                    {
                        if(m.Id == i.IdMenu)
                        {
                            var u = _contextDB.Orden.FirstOrDefault(o => o.IdCliente == IdCliente && o.IdMenu == i.IdMenu);
                            u.Status = "Preparando";
                            _contextDB.Entry(u).State = EntityState.Modified; ;
                            _contextDB.SaveChanges();
                            break;
                        }

                    }
                }
            }

            var c = _contextDB.Cliente.FirstOrDefault(o => o.Id == IdCliente);
            c.Status = "Por recibir";
            c.PrecioFinal = Convert.ToDouble(Total);
            _contextDB.Entry(c).State = EntityState.Modified; ;
            _contextDB.SaveChanges();
            return RedirectToAction("Comiendo");
        }
        [HttpPost]
        public IActionResult LlamarMesero()
        {
            Cookies();
            var insertarLlamada = new LlamarMeseroModel[]
            {
                new LlamarMeseroModel{ IdMesa = IdMesa, IdMesero = IdMesero, Activo = 1, Nombre = NombreMesa}
            };

            foreach (var u in insertarLlamada)
                _contextDB.LlamarMesero.Add(u);
            _contextDB.SaveChanges();

            return RedirectToAction("Comiendo");
        }
        [HttpGet]
        public IActionResult Comiendo()
        {
            Cookies();

            ViewBag.IdCliente = IdCliente;

            List<Usuario> usuarios = _contextDB.Usuario.ToList();
            List<Cliente> clientes = _contextDB.Cliente.ToList();
            List<Menu> menu = _contextDB.Menu.ToList();
            List<Orden> orden = _contextDB.Orden.ToList();

            foreach (Cliente cliente in clientes)
            {
                if(cliente.Id == IdCliente)
                {
                    if(cliente.Status == "Por recibir")
                        ViewBag.Mensaje = "Gracias por comer con nosotros, en un tiempo aproxiamdo de 15 minutos estara lista su comida";

                    if (cliente.Status == "Por enviar")
                        ViewBag.Mensaje = "Gracias por su tiempo de espera, en un momento uno de nuestros meseros vendra a entregarle su comida";

                    if (cliente.Status == "Comiendo")
                    {
                        foreach(var i in usuarios)
                        {
                            if(cliente.IdMesero == i.ID)
                            {
                                IdMesa = cliente.IdMesa;
                                IdMesero = cliente.IdMesero;
                                ViewBag.Mensaje = "Esperando que sea de su agrado sus alimentos, cuando gusten pueden pedir su cuenta. \r Tu mesero acargo es " + i.Nombre;
                                ViewBag.Status = "Comiendo";
                            }
                        }
                    }

                    if (cliente.Status == "Cobrar")
                    {
                        foreach (var i in usuarios)
                        {
                            if (cliente.IdMesero == i.ID)
                            {
                                ViewBag.Mensaje = "En un momento tu mesero " + i.Nombre + " vendra para hacer el cobro";
                                ViewBag.Status = "Cobrar";
                            }
                        }
                    }

                    if(cliente.Status == "Por Terminar")
                    {
                        ViewBag.Mensaje = "Por Terminar";
                        ViewBag.Nombre = cliente.Nombre;
                    }
                    break;
                }
            }


            var viewmodel = new Tablas
            {
                Orden = orden,
                Menu = menu,
                Usuario = usuarios,
                Cliente = clientes
            };
            return View(viewmodel);
        }
        [HttpPost]
        public IActionResult Comiendo(int MetodoPago)
        {
            List<Cliente> clientes = _contextDB.Cliente.ToList();
            List<Orden> orden = _contextDB.Orden.ToList();

            foreach (Orden orden1 in orden)
            {
                if (orden1.IdCliente == IdCliente)
                {
                    var c = _contextDB.Orden.FirstOrDefault(o => o.IdCliente == IdCliente && o.Id == orden1.Id);
                    c.Status = "Por terminar";
                    _contextDB.Entry(c).State = EntityState.Modified; ;
                    _contextDB.SaveChanges();
                }
            }

            var u = _contextDB.Cliente.FirstOrDefault(o => o.Id == IdCliente);
            u.Status = "Cobrar";
            u.MetodoPago = MetodoPago;
            _contextDB.Entry(u).State = EntityState.Modified; ;
            _contextDB.SaveChanges();
            return RedirectToAction("Comiendo");
        }
        [HttpPost]
        public IActionResult Comentario(int rating, string comentario)
        {
            var u = _contextDB.Cliente.FirstOrDefault(o => o.Id == IdCliente);
            u.Comentarios = comentario;
            u.Status = "Terminada";
            u.Calificacion = rating;
            u.DateTimeComentario = DateTime.Now;
            ViewBag.Comentario = u.Comentarios;
            _contextDB.Entry(u).State = EntityState.Modified; ;
            _contextDB.SaveChanges();

            List<Cliente> clientes = _contextDB.Cliente.ToList();
            List<Orden> orden = _contextDB.Orden.ToList();

            foreach (Orden orden1 in orden)
            {
                if (orden1.IdCliente == IdCliente)
                {
                    var c = _contextDB.Orden.FirstOrDefault(o => o.IdCliente == IdCliente && o.Id == orden1.Id);
                    c.Status = "Terminada";
                    _contextDB.Entry(c).State = EntityState.Modified; ;
                    _contextDB.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult BorrarPedido(int idord)
        {
            var prodcarrito = _contextDB.Orden.FirstOrDefault(p => p.Id == idord);
            _contextDB.Orden.Remove(prodcarrito);
            _contextDB.SaveChanges();
            return RedirectToAction(nameof(Orden));
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var data = DateTime.Now.ToString("T");
            return RedirectToAction("Comiendo");
        }
    }
}
