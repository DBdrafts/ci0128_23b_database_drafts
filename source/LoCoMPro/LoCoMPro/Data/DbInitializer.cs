        /*
using System.Diagnostics;
using LoCoMPro.Models;
namespace LoCoMPro.Data
{
    public class DbInitializer
    {
        public static void Initialize(LoCoMProContext context)
        {
            InitializeUsers(context);
            InitializeProvincias(context);
            InitializeCantones(context);
            InitializeStores(context);
            InitializeProducts(context);
            InitializeProducts(context);

        }


        public static void InitializeUsers(LoCoMProContext context)
        {

            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }
            var users = new User[]
            {
                //new User
                new User { UserName = "JuanPerez", Email = "juan.perez@example.com", Password = "contraseña123", CantonName = "Canton1", ProvinciaName = "Alajuela" },
                new User { UserName = "MariaGomez", Email = "maria.gomez@example.com", Password = "contraseña456", CantonName = "Canton2", ProvinciaName = "Limon" },
                new User { UserName = "LuisGonzalez", Email = "luis.gonzalez@example.com", Password = "contraseña789", CantonName = "Canton3", ProvinciaName = "Cartago" },
                new User { UserName = "AnaRodriguez", Email = "ana.rodriguez@example.com", Password = "contraseñaXYZ", CantonName = "Canton4", ProvinciaName = "Puntarenas" },
                new User { UserName = "CarlosMartinez", Email = "carlos.martinez@example.com", Password = "contraseñaABC", CantonName = "Canton5", ProvinciaName = "Heredia" }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }

    }

}

        */
