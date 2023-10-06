using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Policy;
using LoCoMPro.Models;
using Microsoft.EntityFrameworkCore;
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
            List<Category> categories = new();
            List<Product> products = new();
            List<Store> stores = new();
            List<User> users = new();
            List<Register> registers = new();

            //  Initialize all the database tables
            InitializeLocation(context, ref provincias, ref cantones);
            //InitializeProvincias(context, ref provincias);
            //InitializeCantones(context, ref provincias, ref cantones);
            InitializeCategories(context, ref categories);
            InitializeProducts(context, ref products, ref categories);
            InitializeStores(context, ref cantones, ref stores, ref products);
            InitializeUsers(context, ref users, ref cantones);
            InitializeRegisters(context, ref registers, ref users, ref products, ref stores);

        }

        public static void InitializeLocation(LoCoMProContext context, ref List<Provincia> provinces, ref List<Canton> cantons)
        {
            var csvPath = "~/../../../../data/DTA-TABLA POR PROVINCIA-CANTÓN-DISTRITO 2022V3.csv";
            var data = File.ReadAllLines(csvPath, System.Text.Encoding.Latin1)
                .Skip(1)
                .Select(line => line.Split(','))
                .Select(columns => new
                {
                    ProvinceName = columns[2],
                    CantonName = columns[4],
                })
                .Where(pair => !pair.ProvinceName.IsNullOrEmpty() && !pair.ProvinceName.Equals("PROVINCIA"));
                provinces = data.GroupBy(j => j.ProvinceName)
                .Select(group => new Provincia
                {
                    Name = group.Key.ToString(),
                })
                .ToList();


            var grouppedData = data.GroupBy(pair => new { pair.ProvinceName, pair.CantonName });

            foreach ( var group in grouppedData)
            {
                var province = provinces.Find(s => s.Name == group.Key.ProvinceName);
              
                var canton = new Canton
                {
                    CantonName = group.Key.CantonName,
                    Provincia = province!,
                };
                cantons.Add(canton);
            }
            context.Provincias.AddRange(provinces);
            context.Cantones.AddRange(cantons);
            context.SaveChanges();
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

        /* Initialize the categories data in the database */
        public static void InitializeCategories(LoCoMProContext context
            , ref List<Category> categories)
        {
            // Add the categories
            categories.Add(new Category() { CategoryName = "Comida"});
            categories.Add(new Category() { CategoryName = "Tecnología"});
            categories.Add(new Category() { CategoryName = "Ropa"});

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        /* Initialize the products data in the database */
        public static void InitializeProducts(LoCoMProContext context
            , ref List<Product> products, ref List<Category> categories)
        {

            // Add the products
            products.Add(new Product() { Name = "Leche Dos Pinos 1 litros", Brand = "Dos Pinos" 
                , Categories = new List<Category>() { categories[0] } });
            products.Add(new Product() { Name = "Camisa deportiva negra Nike", Brand = "Nike"
                , Categories = new List<Category>() { categories[2] } });
            products.Add(new Product() { Name = "Celular IPhone 15 Pro color beige", Brand = "IPhone", Model = "15 Pro"
                , Categories = new List<Category>() { categories[1] } });

            context.Products.AddRange(products);
            context.SaveChanges();
        }

        /* Initialize the stores data in the database */
        public static void InitializeStores(LoCoMProContext context
            , ref List<Canton> cantones, ref List<Store> stores
            , ref List<Product> product)
        {
            // Add the stores
            stores.Add(new Store() { Name = "Super San Agustin", Location = cantones[0]
                , Products = new List<Product>() { product[0], product[2] }});
            stores.Add(new Store() { Name = "Pali", Location = cantones[1]
                , Products = new List<Product>() { product[0], product[1] , product[2] }});
            stores.Add(new Store() { Name = "MasXMenos", Location = cantones[2]
                , Products = new List<Product>() { product[2] }});

            context.Stores.AddRange(stores);
            context.SaveChanges();
        }

        /* Initialize the users data in the database */
        public static void InitializeUsers(LoCoMProContext context
            , ref List<User> users, ref List<Canton> cantones)
        {
            // Add the users
            users.Add(new User {Id = "2f024bae-0bb6-4db0-8c3f-40bafbcd0273", UserName = "Omar123",
                NormalizedUserName = "OMAR123", Email = "omar123@gmail.com", NormalizedEmail = "OMAR123@GMAIL.COM"
                , PasswordHash = "AQAAAAIAAYagAAAAEAOQ/Jm+AUOvpKZDfgFCdqEhXjHl5vKkJ4wKvVpWGPv58mgk2A9hkeFuLvnKRFTLRw==",
                Location = cantones[0]});
            users.Add(new User { Id = "3af5899a-3957-415a-9675-be20966ba6d7", UserName = "Prueba123", 
                NormalizedUserName = "PRUEBA123", Email = "prueba123@gmail.com", NormalizedEmail = "PRUEBA123@GMAIL.COM"
                , PasswordHash = "AQAAAAIAAYagAAAAEJfi9TUT6VewLFHdzos2qZ29eaoRr4s0YjS60YhkekCR0Mzbe5LMp3sYgj+elkblVA==", 
                Location = cantones[0]});
            users.Add(new User { Id = "9df2b729-89f7-448a-988d-616892794621", UserName = "geanca567", 
                NormalizedUserName = "GEANCA567", Email = "geanca567@hotmail.com", NormalizedEmail = "GEANCA567@HOTMAIL.COM"
                , PasswordHash = "AQAAAAIAAYagAAAAEPYFGrUrFDogPQ7/NslHe12CNANe6GM45VDkOotgLNRR16lUmVUh2C/xeflZ5Inivg==", 
                Location = cantones[0]});
            users.Add(new User { Id = "af37fd1c-51bd-4200-9412-1a710949a6ad", UserName = "Dwayne123", 
                NormalizedUserName = "DWAYNE123", Email = "dwayne123@gmail.com", NormalizedEmail = "DWAYNE123@GMAIL.COM"
                , PasswordHash = "AQAAAAIAAYagAAAAEOT77XEI5fND1riqa9P5VKsRTPfAY5i3piTJ0HTzAPDA7+33lAvWvbUmerrQdXwxag==", 
                Location = cantones[0]});
            users.Add(new User { Id = "eabef9c3-0e08-4372-a88c-6f63075c4a0b", UserName = "Julio444", 
                NormalizedUserName = "JULIO444", Email = "Julio444@ucr.ac.cr", NormalizedEmail = "JULIO444@UCR.AC.CR"
                , PasswordHash = "AQAAAAIAAYagAAAAEBSumaRyX1siCcQ3b8TXll5Km5TWXJry6oq6euj1Bj1JaKgVWIat1jxDrhwvSwZJwA==", 
                Location = cantones[0]});
            users.Add(new User { Id = "f4232f1d-5696-4537-b0a1-02fd36e27dd7", UserName = "Alonso111", 
                NormalizedUserName = "ALONSO111", Email = "Julio444@ucr.ac.cr", NormalizedEmail = "ALONSO111@GMAIL.COM"
                , PasswordHash = "AQAAAAIAAYagAAAAEDcvQfXxtswPnX0ixY3T5EipIoGSk85MVeGdNAVs0tGrkfgseQnfRidNltc5BFSFvw==", 
                Location = cantones[0]});
            context.Users.AddRange(users);
            context.SaveChanges();
        }

        /* Initialize the registers data in the database */
        public static void InitializeRegisters(LoCoMProContext context
            , ref List<Register> registers, ref List<User> users
            , ref List<Product> products, ref List<Store> stores)
        {
            string comment = "Lorem ipsum dolor sit amet, consectetur adipiscing elit" +
                ", sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Elementum pulvinar etiam non quam. ";

            /* Generates the registers by using the index of the products, users, stores and the dates */
            for (int productIndex = 0; productIndex < products.Count; productIndex++)
            {
                for (int usersIndex = 0; usersIndex < users.Count; usersIndex++)
                {
                    for (int storeIndex = 0; storeIndex < stores.Count; storeIndex++)
                    {
                        for (int dateIndex = 3; dateIndex < 5; dateIndex++)
                        {
                            registers.Add(new Register() { Product = products[productIndex], Contributor = users[usersIndex], Store = stores[storeIndex]
                            , Price = (1500 + (1000 * (int) Math.Pow(10, productIndex)) + (100 * usersIndex * storeIndex * dateIndex))
                            , SubmitionDate = new DateTime(2023, dateIndex, dateIndex + storeIndex + usersIndex, 12, 0, 0, DateTimeKind.Utc), Comment = comment });
                        }
                    }
                }
            }

            context.Registers.AddRange(registers);
            context.SaveChanges();
        }

    }

}