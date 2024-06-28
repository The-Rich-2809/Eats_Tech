using Eats_Tech.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using RestSharp;

namespace Eats_Tech.Controllers
{
    public class RecepcionistaController : Controller
    {
        private readonly Eats_TechDB _contextDB;
        public RecepcionistaController(Eats_TechDB contextDB)
        {
            _contextDB = contextDB;
        }
        public static int IdMes { get; set; }
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
            Cookies();
            return View();
        }
        [HttpGet]
        public IActionResult MesasDisponibles()
        {
            Cookies();
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
        public IActionResult Registro(int IdM){
            Cookies();
            IdMes = IdM;
            return View();
        }
        [HttpPost]
        public IActionResult Registro(string Nombre, string Correo, string Telefono)
        {
            Cookies();
            var insetarCliente = new Cliente[]
            {
                new Cliente {Nombre = Nombre, Correo = Correo, IdMesa = IdMes, Status = "Empezando", PrecioFinal = 0, Hora = DateTime.Now, Comentarios = "", Telefono = Telefono}
            };

            foreach(var item in insetarCliente)
                _contextDB.Add(item);
            _contextDB.SaveChanges();
            return RedirectToAction("Index", "Recepcionista");
        }
    }
}
