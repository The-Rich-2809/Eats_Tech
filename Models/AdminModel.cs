using Microsoft.EntityFrameworkCore;

namespace Eats_Tech.Models
{
    public class AdminModel
    {
        public readonly Eats_TechDB _contextDB;

        public AdminModel(Eats_TechDB contextDB)
        {
            _contextDB = contextDB;
        }
        public string Correo { get; set; }
        public string Mesa { get; set; }
        public string Contrasena { get; set; }
        public string Contrasena2 { get; set; }
        public static string Mensaje { get; set; }

        public bool RegisterMesa()
        {
            string NombreMesa = "mesa" + Mesa + "@eatstech.com";
            List<Usuario> usuarios = _contextDB.Usuario.ToList();
            foreach (Usuario i in usuarios)
            {
                if (NombreMesa == i.Correo)
                {
                    if(i.Activo != 777)
                    {
                        Mensaje = "Esta mesa ya esta registrada";
                        return false;
                    }
                    else
                    {
                        if(Contrasena == Contrasena2)
                        {
                            var u = _contextDB.Usuario.FirstOrDefault(e => e.Correo == i.Correo);
                            u.Activo = 0;
                            u.Contrasena = Contrasena;
                            _contextDB.Entry(u).State = EntityState.Modified; ;
                            _contextDB.SaveChanges();
                            return true;
                        }
                        Mensaje = "Las contrasenan no coinsiden";
                        return false;
                    }
                }
            }
            if(Contrasena == Contrasena2)
            {
                var insertarMesa = new Usuario[]
                {
                    new Usuario(){Nombre = "Mesa " + Mesa, Activo = 0, Contrasena = Contrasena, Correo = NombreMesa, TipoUsuario = "Mesa", DireccionImagen = "h"}
                };
                foreach (Usuario i in insertarMesa)
                    _contextDB.Usuario.Add(i);
                _contextDB.SaveChanges();
                return true;
            }
            Mensaje = "Las contrasenan no coinsiden";
            return false;
        }
        public bool ModMesa()
        {
            List<Usuario> usuarios = _contextDB.Usuario.ToList();
            foreach(Usuario i in usuarios)
            {
                if(i.Correo == Correo && Contrasena == Contrasena2)
                {
                    var u = _contextDB.Usuario.FirstOrDefault(e => e.Correo == i.Correo);
                    u.Contrasena = Contrasena;
                    _contextDB.Entry(u).State = EntityState.Modified; ;
                    _contextDB.SaveChanges();
                    return true;
                }
            }
            Mensaje = "Las contrasenan no coinsiden";
            return false;
        }
        public void EliMesa()
        {
            var u = _contextDB.Usuario.FirstOrDefault(e => e.Correo == Correo);
            u.Activo = 777;
            _contextDB.Entry(u).State = EntityState.Modified; ;
            _contextDB.SaveChanges();
        }


    }
}
