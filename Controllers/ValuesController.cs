using Eats_Tech.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eats_Tech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly Eats_TechDB _contextDB;

        public ValuesController(Eats_TechDB perashopDB)
        {
            _contextDB = perashopDB;
        }

        [HttpGet("CambioCantidad")]
        public IActionResult CambioCantidad(int idprodInter, int valor, int carritoId)
        {
            var carrito = _contextDB.Orden.FirstOrDefault(i => i.Id == carritoId && i.IdMenu == idprodInter);
            var inter = _contextDB.Menu.FirstOrDefault(i => i.Id == idprodInter);

            carrito.Cantidad = valor;
            carrito.Costo = carrito.Cantidad * carrito.Costo;

            _contextDB.Orden.Update(carrito);
            _contextDB.SaveChanges();

            var nuevototal = carrito.Cantidad * inter.Costo;

            var response = new
            {
                total = nuevototal,
            };

            return Ok(response);
        }
    }
}
