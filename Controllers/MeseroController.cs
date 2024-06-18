using Eats_Tech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace Eats_Tech.Controllers
{
    public class MeseroController : Controller
    {
        private readonly Eats_TechDB _contextDB;
        public static string CorreoS { get; set; }

        public MeseroController(Eats_TechDB contextDB)
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
        public IActionResult OrdenesEnviar()
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
                if (o.Status == "Preparada")
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
        public IActionResult OrdenesEnviar(int IdCliente)
        {
            List<Cliente> clientes = _contextDB.Cliente.ToList();
            List<Orden> orden = _contextDB.Orden.ToList();

            foreach (Orden orden1 in orden)
            {
                if (orden1.IdCliente == IdCliente)
                {
                    var c = _contextDB.Orden.FirstOrDefault(o => o.IdCliente == IdCliente && o.Id == orden1.Id);
                    c.Status = "Servida";
                    _contextDB.Entry(c).State = EntityState.Modified; ;
                    _contextDB.SaveChanges();
                }
            }

            var u = _contextDB.Cliente.FirstOrDefault(o => o.Id == IdCliente);
            u.Status = "Comiendo";
            _contextDB.Entry(u).State = EntityState.Modified; ;
            _contextDB.SaveChanges();
            return RedirectToAction("OrdenesEntregadas");
        }
        public IActionResult OrdenesEntregadas()
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
                if (o.Status == "Servida" || o.Status == "Terminada" || o.Status == "Por terminar")
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
        [HttpGet]
        public IActionResult OrdenesCobrar()
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
                if (o.Status == "Por terminar")
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
        public IActionResult OrdenesCobrar(int IdCliente)
        {
            List<Menu> menu = _contextDB.Menu.ToList();
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

            EnviarCorreo(IdCliente, orden, menu, clientes);

            var u = _contextDB.Cliente.FirstOrDefault(o => o.Id == IdCliente);
            u.Status = "Terminada";
            _contextDB.Entry(u).State = EntityState.Modified; ;
            _contextDB.SaveChanges();
            return RedirectToAction("OrdenesTerminar");
        }
        public IActionResult OrdenesTerminar()
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
                if (o.Status == "Terminada")
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

        public void EnviarCorreo(int IdCliente, List<Orden> ordenes, List<Menu> menu, List<Cliente> clientes)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("eatstech777@gmail.com", "vbufjpszqvzezthq"),
                EnableSsl = true,
            };

            var Lista = "";
            double Total = 0;
            double SubTotal = 0;
            string Nombre = "";
            string Correo = "";

            foreach (var i in clientes)
            {
                if (i.Id == IdCliente)
                {
                    Nombre = i.Nombre;
                    Correo = i.Correo;
                    break;
                }
            }

            foreach (var i in ordenes) 
            {
                if(i.IdCliente == IdCliente)
                {
                    foreach(var o in menu)
                    {
                        if(i.IdMenu == o.Id)
                        {
                            SubTotal = o.Costo * i.Cantidad;
                            Total += SubTotal;
                            Lista += $@"
                            <tbody>
                                <tr>
                                    <td>{o.NombrePlatillo}</td>
                                    <td>{i.Cantidad}</td>
                                    <td>${o.Costo}</td>
                                    <td>${SubTotal}</td>
                                </tr>
                            </tbody>";
                        }
                    }
                }
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress("eatstech777@gmail.com"),
                Subject = "Confirmación de la compra",
                Body = $@"
                    <html lang=""es"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Tabla de Platillos</title>
                    </head>
                    <body>

                    <font face=""Arial"">
                    <h2>Ticket</h2>

                    <p><strong>Nombre del Cliente: </strong>{Nombre}</p>
                    <p><strong>Fecha :</strong>{DateTime.Now}</p>

                    <table border=""2"" width=""100%"" cellpadding=""8"" cellspacing=""0"">
                        <thead>
                            <tr>
                                <th>Nombre platillo</th>
                                <th>Cantidad</th>
                                <th>P.U</th>
                                <th>Total</th>
                            </tr>
                        </thead>
                        {Lista}
                        <tfoot>
                            <tr>
                                <td colspan=""3"" align=""right""><strong>Precio Final</strong></td>
                                <td><strong>${Total}</strong></td>
                            </tr>
                        </tfoot>
                    </table>
                    </font>

                    </body>
                    </html>",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(Correo);
            smtpClient.Send(mailMessage);
        }
    }
}
