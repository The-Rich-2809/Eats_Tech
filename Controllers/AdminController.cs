using Microsoft.AspNetCore.Mvc;
using Eats_Tech.Models;

using Eats_Tech.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Eats_Tech.Controllers
{
    public class AdminController : Controller
    {
        private readonly Eats_TechDB _contextDB;
        public static string CorreoS { get; set;}
        public static int IdCat { get; set;}

        public AdminController( Eats_TechDB contextDB)
        {
            _contextDB = contextDB;
        }
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
                        ViewBag.Mesa = user.Nombre;
                    }
                }
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Menu()
        {
            List<Categoria> categoria = _contextDB.Categoria.ToList();
            return View(categoria);
        }

        [HttpGet]
        public IActionResult AgregarMenu()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AgregarMenu(string Categoria)
        {
            List<Categoria> categoria = _contextDB.Categoria.ToList();
            foreach(var i in categoria)
            {
                if(i.NombreCategoria == Categoria)
                {
                    ViewBag.Mensaje = "Esta categoria ya existe";
                    return View();
                }
            }

            var insertarcategoria = new Categoria[]
            {
                new Categoria(){NombreCategoria = Categoria, Activo = 1},
            };

            foreach (var u in insertarcategoria)
                _contextDB.Categoria.Add(u);
            _contextDB.SaveChanges();

            return RedirectToAction("Menu");
        }
        [HttpGet]
        public IActionResult ModMenu(string Categoria, int IdCategoria)
        {
            ViewBag.Categoria = Categoria;
            IdCat = IdCategoria;
            return View();
        }
        [HttpPost]
        public IActionResult ModMenu(string Categoria)
        {
            List<Categoria> categoria = _contextDB.Categoria.ToList();
            foreach (var i in categoria)
            {
                if (i.NombreCategoria == Categoria)
                {
                    ViewBag.Mensaje = "Esta categoria ya existe";
                    return View();
                }
            }

            var u = _contextDB.Categoria.FirstOrDefault(o => o.Id == IdCat);
            string cat = u.NombreCategoria;
            u.NombreCategoria = Categoria;
            _contextDB.Entry(u).State = EntityState.Modified; ;
            _contextDB.SaveChanges();

            List<Menu> menu = _contextDB.Menu.ToList();

           for(int i = 0; i < menu.Count; i++)
           {
                var j = _contextDB.Menu.FirstOrDefault(o => o.Categoria == cat);
                if(j != null)
                {
                    j.Categoria = Categoria;
                    _contextDB.Entry(j).State = EntityState.Modified; ;
                    _contextDB.SaveChanges();
                }
           }

            return RedirectToAction("Menu");
        }
        [HttpGet]
        public IActionResult EliMenu(int IdCategoria)
        {
            var Cat = _contextDB.Categoria.FirstOrDefault(c => c.Id == IdCategoria);
            IdCat = IdCategoria;
            return View(Cat);
        }
        [HttpPost]
        public IActionResult EliMenu()
        {
            var u = _contextDB.Categoria.FirstOrDefault(o => o.Id == IdCat);
            u.Activo = 0;
            _contextDB.Entry(u).State = EntityState.Modified; ;
            _contextDB.SaveChanges();
            return RedirectToAction("Menu");
        }

        [HttpGet]
        public IActionResult Platillo(string Categoria)
        {
            ViewBag.NombreCategoria = Categoria;
            List<Menu> menu = _contextDB.Menu.ToList();
            return View(menu);
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
            List<Usuario> usuarios = _contextDB.Usuario.ToList();
            List<Cliente> clientes = _contextDB.Cliente.ToList();
            foreach(Cliente cliente in clientes)
            {
                foreach(Usuario usuario in usuarios)
                {
                    if(usuario.Correo == CorreoS && usuario.ID == cliente.IdMesa && cliente.Status != "Finalizado")
                    {
                        ViewBag.Mensaje = "No puedes eliminar esta mesa ya que se esta ocupando Xd";
                        var Usuario = _contextDB.Usuario.FirstOrDefault(c => c.Correo == CorreoS);
                        return View(Usuario);
                    }
                }
            }
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
            Cookies();
            List<Usuario> usuarios = _contextDB.Usuario.ToList();
            List<Cliente> clientes = _contextDB.Cliente.ToList();
            List<Orden> orden = _contextDB.Orden.ToList();
            List<Menu> menu = _contextDB.Menu.ToList();

            string[,] cliente = new string[clientes.Count,2];
            int i = 0;
            int j = 0;
            string[,] ordenes = new string[orden.Count,6];
            ViewBag.Rango = orden.Count;

            foreach (var o in orden)
            {
                if (o.Status == "Preparando")
                {
                    foreach (var c in clientes)
                    {
                        if (c.Id == o.IdCliente)
                        {
                            foreach (var u in usuarios)
                            {
                                if (u.ID == c.IdMesa)
                                {
                                    foreach (var m in menu)
                                    {
                                        if (m.Id == o.IdMenu)
                                        {
                                            ordenes[i, 0] = Convert.ToString(o.IdCliente);
                                            ordenes[i, 1] = m.RutaImagen;
                                            ordenes[i, 2] = m.NombrePlatillo;
                                            ordenes[i, 3] = Convert.ToString(o.Cantidad);
                                            ordenes[i, 4] = o.Status;
                                            ordenes[i, 5] = Convert.ToString(o.Costo);
                                            ViewBag.Ordenes = ordenes;
                                            i++;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }

                }
            }

            var viewmodel = new Tablas
            {
                Usuario = usuarios,
                Cliente = clientes
            };
            return View(viewmodel);
        }

    }
}
