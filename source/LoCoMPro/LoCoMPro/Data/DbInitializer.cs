using System.Diagnostics;
using LoCoMPro.Models;
using Microsoft.IdentityModel.Tokens;

namespace LoCoMPro.Data
{
    public class DbInitializer
    {
        public static void Initialize(LoCoMProContext context)
        {
            List<Provincia> provincias = new List<Provincia>();
            List<Canton> cantones = new List<Canton>();
            List<Store> stores = new ();

            InitializeProvincias(context, ref provincias);
            InitializeCantones(context, ref provincias, ref cantones);
            InitializeStores(context, ref cantones, ref stores);
            //InitializeUsers(context);
           // InitializeCantones(context);

        }

        public static void InitializeProvincias(LoCoMProContext context, ref List<Provincia> provincias)
        {

            if (context.Provincias.Any())
            {
                return;   // DB has been seeded
            }
            
            provincias.Add(new Provincia() { Name = "San Jose"});
            provincias.Add(new Provincia() { Name = "Heredia"});

            context.Provincias.AddRange(provincias);
            context.SaveChanges();
        }

        public static void InitializeCantones(LoCoMProContext context
            , ref List<Provincia> provincias, ref List<Canton> cantones)
        {

            if (context.Cantones.Any() || provincias.IsNullOrEmpty())
            {
                return;   // DB has been seeded
            }

            cantones.Add(new Canton() { CantonName = "Montes de Oca", Provincia = provincias[0], ProvinciaName = "San Jose"});
            cantones.Add(new Canton() { CantonName = "Guadalupe", Provincia = provincias[0], ProvinciaName = "San Jose"});
            cantones.Add(new Canton() { CantonName = "Santo Domingo", Provincia = provincias[0], ProvinciaName = "Heredia"});

            context.Provincias.AddRange(provincias);
            context.SaveChanges();
        }

        public static void InitializeStores(LoCoMProContext context, ref List<Canton> cantones, ref List<Store> stores)
        {
            if (!context.Stores.Any() || cantones.IsNullOrEmpty())
            {
                stores.Add(new Store() { Name = "Super San Agustin", Location = cantones[0], CantonName = "Montes de Oca", ProvinciaName = "San Jose" });
                stores.Add(new Store() { Name = "Pali", Location = cantones[1], CantonName = "Guadalupe", ProvinciaName = "San Jose" });
                stores.Add(new Store() { Name = "MasXMenos", Location = cantones[2], CantonName = "Santo Domingo", ProvinciaName = "Heredia" });
            }
            context.Stores.AddRange(stores);
            context.SaveChanges();
        }

        //public static void InitializeUsers(LoCoMProContext context)
        //{

        //    if (context.Users.Any())
        //    {
        //        return;   // DB has been seeded
        //    }
        //    var users = new User[]
        //    {
        //        //new User
        //        new User { UserName = "JuanPerez", Email = "juan.perez@example.com", Password = "contraseña123", CantonName = "Canton1", ProvinciaName = "Alajuela" },
        //        new User { UserName = "MariaGomez", Email = "maria.gomez@example.com", Password = "contraseña456", CantonName = "Canton2", ProvinciaName = "Limon" },
        //        new User { UserName = "LuisGonzalez", Email = "luis.gonzalez@example.com", Password = "contraseña789", CantonName = "Canton3", ProvinciaName = "Cartago" },
        //        new User { UserName = "AnaRodriguez", Email = "ana.rodriguez@example.com", Password = "contraseñaXYZ", CantonName = "Canton4", ProvinciaName = "Puntarenas" },
        //        new User { UserName = "CarlosMartinez", Email = "carlos.martinez@example.com", Password = "contraseñaABC", CantonName = "Canton5", ProvinciaName = "Heredia" }
        //    };

        //    context.Users.AddRange(users);
        //    context.SaveChanges();
        //}




    }

}