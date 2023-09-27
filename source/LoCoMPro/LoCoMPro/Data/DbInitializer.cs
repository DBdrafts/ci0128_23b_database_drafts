using System.Diagnostics;
using LoCoMPro.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;

namespace LoCoMPro.Data
{
    public class DbInitializer
    {
        /* Initialize the seed data of the database */
        public static void Initialize(LoCoMProContext context)
        {
            if (context.Provincias.Any())
            {
                return;   // DB has been seeded
            }

            List<Provincia> provincias = new();
            List<Canton> cantones = new();
            List<Store> stores = new();
            List<Product> products = new();
            List<User> users = new();
            List<Register> registers = new();

            //  Initialize all the database tables
            InitializeProvincias(context, ref provincias);
            InitializeCantones(context, ref provincias, ref cantones);
            InitializeStores(context, ref cantones, ref stores);
            InitializeProducts(context, ref products);
            InitializeUsers(context, ref users, ref cantones);
            InitializeRegisters(context, ref registers, ref users, ref products, ref stores);

        }

        /* Initialize the provincias data in the database */
        public static void InitializeProvincias(LoCoMProContext context, ref List<Provincia> provincias)
        {
            // Add the provincias
            provincias.Add(new Provincia() { Name = "San Jose"});
            provincias.Add(new Provincia() { Name = "Heredia"});

            context.Provincias.AddRange(provincias);
            context.SaveChanges();
        }

        /* Initialize the cantones data in the database */
        public static void InitializeCantones(LoCoMProContext context
            , ref List<Provincia> provincias, ref List<Canton> cantones)
        {
            // Add the cantones
            cantones.Add(new Canton() { CantonName = "Montes de Oca", Provincia = provincias[0] });
            cantones.Add(new Canton() { CantonName = "Guadalupe", Provincia = provincias[0] });
            cantones.Add(new Canton() { CantonName = "Santo Domingo", Provincia = provincias[1] });

            context.Cantones.AddRange(cantones);
            context.SaveChanges();
        }

        /* Initialize the stores data in the database */
        public static void InitializeStores(LoCoMProContext context, ref List<Canton> cantones
            , ref List<Store> stores)
        {
            // Add the stores
            stores.Add(new Store() { Name = "Super San Agustin", Location = cantones[0], CantonName = "Montes de Oca" });
            stores.Add(new Store() { Name = "Pali", Location = cantones[1], CantonName = "Guadalupe" });
            stores.Add(new Store() { Name = "MasXMenos", Location = cantones[2], CantonName = "Santo Domingo" });

            context.Stores.AddRange(stores);
            context.SaveChanges();
        }

        /* Initialize the products data in the database */
        public static void InitializeProducts(LoCoMProContext context
            , ref List<Product> products)
        {
            // Add the products
            products.Add(new Product() { Name = "Leche Dos Pinos 1 litros", Brand = "Dos Pinos"});
            products.Add(new Product() { Name = "Celular IPhone 15 Pro color beige", Brand = "IPhone", Model = "15 Pro"});
            products.Add(new Product() { Name = "Camisa deportiva negra Nike", Brand = "Nike"});

            context.Products.AddRange(products);
            context.SaveChanges();
        }

        /* Initialize the users data in the database */
        public static void InitializeUsers(LoCoMProContext context
            , ref List<User> users, ref List<Canton> cantones)
        {
            // Add the users
            users.Add(new User() { UserName = "Jose Miguel Garcia Lopez", Email = "email1@gmail.com", Password = "Password.1", Location = cantones[0]});
            users.Add(new User() { UserName = "Ana Maria Cerdas Lizano", Email = "email2@gmail.com", Password = "Password.2", Location = cantones[1]});
            users.Add(new User() { UserName = "Keith Wilson Buzkova", Email = "email3gmail.com", Password = "Password.3", Location = cantones[2]});
            users.Add(new User() { UserName = "Yordi Lopez Rodríguez", Email = "email4gmail.com", Password = "Password.4", Location = cantones[0]});
            users.Add(new User() { UserName = "Tatiana Espinoza Villalobos", Email = "email5@gmail.com", Password = "Password.5", Location = cantones[1]});

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        /* Initialize the registers data in the database */
        public static void InitializeRegisters(LoCoMProContext context
            , ref List<Register> registers, ref List<User> users
            , ref List<Product> products, ref List<Store> stores)
        {
            // Add the users
            registers.Add(new Register() { Product = products[0], Contributor = users[0], Store = stores[0], Price = 2000, SubmitionDate = new DateTime(year: 2023, month: 1, day: 10) });
            registers.Add(new Register() { Product = products[0], Contributor = users[1], Store = stores[0], Price = 2100, SubmitionDate = new DateTime(year: 2023, month: 1, day: 11) });
            registers.Add(new Register() { Product = products[0], Contributor = users[2], Store = stores[0], Price = 1900, SubmitionDate = new DateTime(year: 2023, month: 1, day: 8) });
            registers.Add(new Register() { Product = products[0], Contributor = users[3], Store = stores[0], Price = 2000, SubmitionDate = new DateTime(year: 2023, month: 1, day: 1) });
            registers.Add(new Register() { Product = products[0], Contributor = users[1], Store = stores[0], Price = 2300, SubmitionDate = new DateTime(year: 2022, month: 12, day: 31) });
            registers.Add(new Register() { Product = products[0], Contributor = users[0], Store = stores[0], Price = 2250, SubmitionDate = new DateTime(year: 2023, month: 1, day: 15) });

            registers.Add(new Register() { Product = products[0], Contributor = users[4], Store = stores[1], Price = 1000000, SubmitionDate = new DateTime(year: 2023, month: 4, day: 4) });

            registers.Add(new Register() { Product = products[1], Contributor = users[2], Store = stores[1], Price = 900000, SubmitionDate = new DateTime(year: 2023, month: 3, day: 28) });
            registers.Add(new Register() { Product = products[1], Contributor = users[0], Store = stores[1], Price = 904000, SubmitionDate = new DateTime(year: 2023, month: 3, day: 14) });
            registers.Add(new Register() { Product = products[1], Contributor = users[4], Store = stores[1], Price = 910000, SubmitionDate = new DateTime(year: 2023, month: 3, day: 1) });
            registers.Add(new Register() { Product = products[1], Contributor = users[2], Store = stores[1], Price = 810000, SubmitionDate = new DateTime(year: 2023, month: 4, day: 19) });

            registers.Add(new Register() { Product = products[2], Contributor = users[1], Store = stores[2], Price = 21000, SubmitionDate = new DateTime(year: 2023, month: 8, day: 30) });
            registers.Add(new Register() { Product = products[2], Contributor = users[3], Store = stores[2], Price = 19000, SubmitionDate = new DateTime(year: 2023, month: 8, day: 24) });

            registers.Add(new Register() { Product = products[2], Contributor = users[3], Store = stores[1], Price = 26000, SubmitionDate = new DateTime(year: 2023, month: 6, day: 24) });

            context.Registers.AddRange(registers);
            context.SaveChanges();
        }

    }

}