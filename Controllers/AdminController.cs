using Microsoft.AspNetCore.Mvc;
using Eats_Tech.Models;

using Eats_Tech.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Eats_Tech.Providers;
using Eats_Tech.Helpers;

namespace Eats_Tech.Controllers
{
    public class AdminController : Controller
    {
        private HelperUploadFiles helperUpload;
        private readonly Eats_TechDB _contextDB;
        public static string CorreoS { get; set;}
        public static int IdCat { get; set;}
        public static int IdPlat { get; set; }
        public static int IdU {  get; set; }
        public static string NameCat { get; set;}

        public AdminController(HelperUploadFiles helperUpload,  Eats_TechDB contextDB)
        {
            _contextDB = contextDB;
            this.helperUpload = helperUpload;
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
            foreach(var u in user)
            {
                if(u.Correo == CorreoS)
                {
                    ViewBag.ImagenPerfil = u.DireccionImagen;
                    return View();
                }
                    
            }
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
                    if(i.Activo == 0)
                    {
                        var u = _contextDB.Categoria.FirstOrDefault(o => o.NombreCategoria == Categoria);
                        u.Activo = 1;
                        _contextDB.Entry(u).State = EntityState.Modified; ;
                        _contextDB.SaveChanges();
                        return RedirectToAction("Menu");
                    }
                    else
                    {
                        ViewBag.Mensaje = "Esta categoria ya existe";
                        return View();
                    }
                    
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
            var u = _contextDB.Categoria.FirstOrDefault(o => o.Id == IdCat);
            string cat = u.NombreCategoria;

            List<Categoria> categoria = _contextDB.Categoria.ToList();
            foreach (var i in categoria)
            {
                if (i.NombreCategoria == Categoria)
                {
                    if (i.Activo == 0)
                    {
                        var r = _contextDB.Categoria.FirstOrDefault(o => o.NombreCategoria == cat);
                        r.NombreCategoria = Categoria;
                        _contextDB.Entry(r).State = EntityState.Modified; ;
                        _contextDB.SaveChanges();
                        return RedirectToAction("Menu");
                    }
                    else
                    {
                        ViewBag.Mensaje = "Esta categoria ya existe";
                        return View();
                    }
                }
            }

            
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
            if (Categoria != null)
                NameCat = Categoria;

            ViewBag.NombreCategoria = NameCat;
            List<Menu> menu = _contextDB.Menu.ToList();
            return View(menu);
        }
        [HttpGet]
        public IActionResult AgregarPlatillo()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AgregarPlatillo(IFormFile Imagen, string NombrePlatillo, string Descripcion, double Precio)
        {
            List<Menu> menu = _contextDB.Menu.ToList();
            foreach(var m in menu)
            {
                if(m.NombrePlatillo == NombrePlatillo && m.Activo == 1)
                {
                    ViewBag.Mensaje = "Esta platillo ya existe";
                    return View();
                }
            }

            string nombreImagen = "";
            List<Categoria> categorias = _contextDB.Categoria.ToList();
            foreach(var i in categorias)
            {
                if(i.NombreCategoria == NameCat)
                {
                    nombreImagen = Convert.ToString(i.Id) + "_" + Convert.ToString(menu.Count + 1) + "_" + Imagen.FileName;
                    await this.helperUpload.UploadFilesAsync(Imagen, nombreImagen, Folders.Platillos);
                }
            }

            var insertarPlatillo = new Menu[]
            {
                new Menu(){NombrePlatillo = NombrePlatillo, Costo = Precio, Descripcion = Descripcion, RutaImagen = "../Images/Platillos/" + nombreImagen, Categoria = NameCat, Activo = 1},
            };

            foreach (var u in insertarPlatillo)
                _contextDB.Menu.Add(u);
            _contextDB.SaveChanges();

            return RedirectToAction("Platillo");
        }
        [HttpGet]
        public IActionResult ModPlatillo(int IdPlatillo)
        {
            IdPlat = IdPlatillo;
            List<Menu> menu = _contextDB.Menu.ToList();
            foreach (var m in menu)
            {
                if(m.Id == IdPlatillo)
                {
                    NameCat = m.Categoria;
                    ViewBag.Nombre = m.NombrePlatillo;
                    ViewBag.Costo = m.Costo;
                    ViewBag.Descripcion = m.Descripcion;
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ModPlatillo(IFormFile Imagen, string NombrePlatillo, string Descripcion, double Precio)
        {
            List<Menu> menu = _contextDB.Menu.ToList();
            foreach (var m in menu)
            {
                if (m.NombrePlatillo == NombrePlatillo && m.Activo == 1 && m.Id != IdPlat)
                {
                    ViewBag.Mensaje = "Esta platillo ya existe";
                    return View();
                }
            }

            var u = _contextDB.Menu.FirstOrDefault(o => o.Id == IdPlat);
            string nombreImagen = "";
            if(Imagen != null)
            {
                List<Categoria> categorias = _contextDB.Categoria.ToList();
                foreach (var i in categorias)
                {
                    if (i.NombreCategoria == NameCat)
                    {
                        nombreImagen = Convert.ToString(i.Id) + "_" + Convert.ToString(menu.Count + 1) + "_" + Imagen.FileName;
                        await this.helperUpload.UploadFilesAsync(Imagen, nombreImagen, Folders.Platillos);
                        u.RutaImagen = "../Images/Platillos/" + nombreImagen;
                    }
                }
            }

            
            u.NombrePlatillo = NombrePlatillo;
            u.Descripcion = Descripcion;
            u.Costo = Precio;
            _contextDB.Entry(u).State = EntityState.Modified; ;
            _contextDB.SaveChanges();
            return RedirectToAction("Platillo");
        }
        public IActionResult EliPlatillo(int IdPlatillo)
        {
            var Cat = _contextDB.Menu.FirstOrDefault(c => c.Id == IdPlatillo);
            IdPlat = IdPlatillo;
            return View(Cat);
        }
        [HttpPost]
        public IActionResult EliPlatillo()
        {
            var u = _contextDB.Menu.FirstOrDefault(o => o.Id == IdPlat);
            u.Activo = 0;
            _contextDB.Entry(u).State = EntityState.Modified; ;
            _contextDB.SaveChanges();
            return RedirectToAction("Platillo");
        }

        [HttpGet]
        public IActionResult Usuarios()
        {
            List<Usuario> user = _contextDB.Usuario.ToList();
            return View(user);
        }
        [HttpGet]
        public IActionResult AgregarUsuarios()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AgregarUsuarios(IFormFile Imagen, string Correo, string Contra1, string Contra2, string Nombre, string Categoria)
        {
            List<Usuario> user = _contextDB.Usuario.ToList();
            foreach(var u in user)
            {
                if(u.Correo == Correo && u.Activo != 777)
                {
                    ViewBag.Mensaje = "Este correo ya esta registrado";
                    return View();
                }
            }

            if(Contra1 != Contra2)
            {
                ViewBag.Mensaje = "Las contraseñas no son iguales";
                return View();
            }

            string nombreImagen = "";

            if (Imagen != null)
            {
                nombreImagen = Convert.ToString(user.Count + 1) + "_" + Imagen.FileName;
                await this.helperUpload.UploadFilesAsync(Imagen, nombreImagen, Folders.Users);
                nombreImagen = "../Images/Users/" + nombreImagen;
            }
            else
            {
                nombreImagen = "h";
            }
            

            var insertarusuario = new Usuario[]
            {
                new Usuario {Nombre = Nombre, Contrasena = Contra1, Correo = Correo, TipoUsuario = Categoria, Activo = 0, DireccionImagen = nombreImagen},
            };

            foreach (var u in insertarusuario)
                _contextDB.Usuario.Add(u);
            _contextDB.SaveChanges();

            return RedirectToAction("Usuarios");
        }
        [HttpGet]
        public IActionResult ModUsuarios(int Id)
        {
            IdU = Id;
            var usuario = _contextDB.Usuario.FirstOrDefault(p => p.ID == Id);
            return View(usuario);
        }
        [HttpPost]
        public async Task<IActionResult> ModUsuarios(IFormFile Imagen, string Correo, string Contra1, string Contra2, string Nombre, string Categoria, int Id)
        {
            var usuario = _contextDB.Usuario.FirstOrDefault(p => p.ID == Id);

            if(usuario.Correo != Correo)
            {
                List<Usuario> user = _contextDB.Usuario.ToList();
                foreach (var u in user)
                {
                    if (u.Correo == Correo && u.Activo != 777)
                    {
                        ViewBag.Mensaje = "Este correo ya esta registrado";
                        return View();
                    }
                }
                usuario.Correo = Correo;
            }

            if(Contra1 != null || Contra2 != null)
            {
                if (Contra1 != Contra2)
                {
                    ViewBag.Mensaje = "Las contraseñas no son iguales";
                    return View();
                }
                usuario.Contrasena = Contra1;
            }

            usuario.Nombre = Nombre;
            usuario.TipoUsuario = Categoria;

            if(Imagen != null)
            {
                string nombreImagen = Id + "_" + Imagen.FileName;
                await this.helperUpload.UploadFilesAsync(Imagen, nombreImagen, Folders.Users);
                nombreImagen = "../Images/Users/" + nombreImagen;
                usuario.DireccionImagen = nombreImagen;
            }
            
            _contextDB.Entry(usuario).State = EntityState.Modified; ;
            _contextDB.SaveChanges();
            return RedirectToAction("Usuarios");
        }
        [HttpGet]
        public IActionResult EliUsuarios(int Id)
        {
            var Cat = _contextDB.Usuario.FirstOrDefault(c => c.ID == Id);
            IdPlat = Id;
            return View(Cat);
        }
        [HttpPost]
        public IActionResult EliUsuarios()
        {
            var u = _contextDB.Usuario.FirstOrDefault(o => o.ID == IdPlat);
            u.Activo = 777;
            _contextDB.Entry(u).State = EntityState.Modified; ;
            _contextDB.SaveChanges();
            return RedirectToAction("Usuarios");
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

            var viewmodel = new Tablas
            {
                Usuario = usuarios,
                Cliente = clientes
            };
            return View(viewmodel);
        }
        [HttpGet]
        public IActionResult Comentarios()
        {
            List<Cliente> clientes = _contextDB.Cliente.ToList();
            List<Usuario> usuario = _contextDB.Usuario.ToList();

            var viewmodel = new Tablas
            {
                Usuario = usuario,
                Cliente = clientes
            };
            return View(viewmodel);
        }

    }
}
