using Eats_Tech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eats_Tech.Controllers
{
    public class CocinaController : Controller
    {
        private readonly Eats_TechDB _contextDB;
        public static string CorreoS { get; set; }

        public CocinaController(Eats_TechDB contextDB)
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
                        CorreoS = user.Correo;
                    }
                }
            }
        }
        public IActionResult Index()
        {
            Cookies();
            List<Usuario> user = _contextDB.Usuario.ToList();
            foreach (var u in user)
            {
                if (u.Correo == CorreoS)
                {
                    ViewBag.ImagenPerfil = u.DireccionImagen;
                    return View();
                }

            }
            return View();
        }
        [HttpGet]
        public IActionResult OrdenesCocinar()
        {
            Cookies();
            List<Usuario> usuarios = _contextDB.Usuario.ToList();
            List<Cliente> clientes = _contextDB.Cliente.ToList();
            List<Orden> orden = _contextDB.Orden.ToList();
            List<Menu> menu = _contextDB.Menu.ToList();

            string[,] cliente = new string[clientes.Count, 2];
            int i = 0;
            int j = 0;
            string[,] ordenes = new string[orden.Count, 6];
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
        [HttpPost]
        public IActionResult OrdenesCocinar(int IdCliente)
        {
            List<Cliente> clientes = _contextDB.Cliente.ToList();
            List<Orden> orden = _contextDB.Orden.ToList();

            foreach (Orden orden1 in orden)
            {
                if (orden1.IdCliente == IdCliente)
                {
                    var c = _contextDB.Orden.FirstOrDefault(o => o.IdCliente == IdCliente && o.Id == orden1.Id);
                    c.Status = "Preparada";
                    _contextDB.Entry(c).State = EntityState.Modified; ;
                    _contextDB.SaveChanges();
                }
            }

            var u = _contextDB.Cliente.FirstOrDefault(o => o.Id == IdCliente);
            u.Status = "Por enviar";
            _contextDB.Entry(u).State = EntityState.Modified; ;
            _contextDB.SaveChanges();
            return RedirectToAction("OrdenesCocinadas");
        }
        [HttpGet]
        public IActionResult OrdenesCocinadas()
        {
            Cookies();
            List<Usuario> usuarios = _contextDB.Usuario.ToList();
            List<Cliente> clientes = _contextDB.Cliente.ToList();
            List<Orden> orden = _contextDB.Orden.ToList();
            List<Menu> menu = _contextDB.Menu.ToList();

            string[,] cliente = new string[clientes.Count, 2];
            int i = 0;
            int j = 0;
            string[,] ordenes = new string[orden.Count, 6];
            ViewBag.Rango = orden.Count;

            foreach (var o in orden)
            {
                if (o.Status != "Preparando")
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
