using NetTopologySuite.Geometries;
using LoCoMPro.Models;
using Microsoft.IdentityModel.Tokens;

namespace LoCoMPro.Data
{
    /// <summary>
    /// Seeds the database with needed and testing data.
    /// </summary>
    public class DbInitializer
    {
        /// <summary>
        /// Initialize the seed data of the database.
        /// </summary>
        /// <param name="context">Context that is beeing intialized.</param>
        public static void Initialize(LoCoMProContext context)
        {
            if (context.Provincias.Any())
            {
                return;   // DB has been seeded
            }
            var currentDir = Directory.GetCurrentDirectory();
            context.ExecuteSqlScriptFile(currentDir + "/Data/SearchRegister.sql");
            context.ExecuteSqlScriptFile(currentDir + "/Data/CalculateDistance.sql");
            context.ExecuteSqlScriptFile(currentDir + "/Data/GetSearchResults.sql");

            List<Provincia> provincias = new();
            List<Canton> cantones = new();
            List<Category> categories = new();
            List<Product> products = new();
            List<Store> stores = new();
            List<User> users = new();
            List<Register> registers = new();

            //  Initialize all the database tables
            InitializeLocation(context, ref provincias, ref cantones);
            InitializeCategories(context, ref categories);
            InitializeProducts(context, ref products, ref categories);
            InitializeStores(context, ref cantones, ref stores, ref products);
            InitializeUsers(context, ref users, ref cantones);
            InitializeRegisters(context, ref registers, ref users, ref products, ref stores);

        }

        /// <summary>
        /// Initializes provinces and cantons for application.
        /// </summary>
        /// <param name="context">Context that needs initialized.</param>
        /// <param name="provinces">List of provinces that will be filled.</param>
        /// <param name="cantons">List of cantons that will be filled.</param>
        public static void InitializeLocation(LoCoMProContext context, ref List<Provincia> provinces, ref List<Canton> cantons)
        {
            var csvPath = "~/../../../../data/PROVINCIA-CANTÓN-UBICACIÓN.csv";
            var data = File.ReadAllLines(csvPath, System.Text.Encoding.UTF8)
                .Skip(1)
                .Select(line => line.Split(','))
                .Select(columns => new
                {
                    ProvinceName = columns[0],
                    CantonName = columns[1],
                    Latitude = columns[2],  // Assuming column index 2 is for 'LATITUD'
                    Longitude = columns[3], // Assuming column index 3 is for 'LONGITUD'
                })
                .Where(pair => !pair.ProvinceName.IsNullOrEmpty() && !pair.ProvinceName.Equals("PROVINCIA"));
            provinces = data.GroupBy(j => j.ProvinceName)
                .Select(group => new Provincia
                {
                    Name = group.Key.ToString(),
                })
                .ToList();


            var grouppedData = data.GroupBy(pair => new { pair.ProvinceName, pair.CantonName, pair.Latitude, pair.Longitude });

            foreach (var group in grouppedData)
            {
                var province = provinces.Find(s => s.Name == group.Key.ProvinceName);

                if (float.TryParse(group.Key.Latitude, out float latitude) && float.TryParse(group.Key.Longitude, out float longitude))
                {
                    var coordinates = new Coordinate (latitude, longitude);
                    var canton = new Canton
                    {
                        CantonName = group.Key.CantonName,
                        Provincia = province!,
                        Geolocation = new Point (coordinates.X, coordinates.Y) { SRID = 4326 },
                    };
                    cantons.Add(canton);
                }
            }
            context.Provincias.AddRange(provinces);
            context.Cantones.AddRange(cantons);
            context.SaveChanges();
        }

        /// <summary>
        /// Initialize the categories data in the database.
        /// </summary>
        /// <param name="context">Context to initialize.</param>
        /// <param name="categories">List of categories that will be initialized.</param>
        public static void InitializeCategories(LoCoMProContext context
            , ref List<Category> categories)
        {
            // Add the categories
            categories.Add(new Category() { CategoryName = "Comida"});
            categories.Add(new Category() { CategoryName = "Ropa"});
            categories.Add(new Category() { CategoryName = "Tecnología"});

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        /// <summary>
        /// Initialize the products data in the database.
        /// </summary>
        /// <param name="context">Context to initialize.</param>
        /// <param name="products">List of products that will be initialized.</param>
        /// <param name="categories">Initialized categories to asociate with products.</param>
        public static void InitializeProducts(LoCoMProContext context
            , ref List<Product> products, ref List<Category> categories)
        {

            // Add the products
            products.Add(new Product() { Name = "Leche semidescremada 1 litro", Brand = "Dos Pinos" 
                , Categories = new List<Category>() { categories[0] } });
            products.Add(new Product() { Name = "Leche descremada 1 litro", Brand = "Coronado" 
                , Categories = new List<Category>() { categories[0] } });
            products.Add(new Product() { Name = "Camisa deportiva negra", Brand = "Nike"
                , Categories = new List<Category>() { categories[1] } });
            products.Add(new Product() { Name = "Apple Watch rosa", Brand = "Apple", Model = "Serie 9"
                , Categories = new List<Category>() { categories[1], categories[2] } });
            products.Add(new Product() { Name = "Celular IPhone color beige", Brand = "Apple", Model = "15 Pro"
                , Categories = new List<Category>() { categories[2] } });

            context.Products.AddRange(products);
            context.SaveChanges();
        }

        /// <summary>
        /// Initialize the stores data in the database.
        /// </summary>
        /// <param name="context">Context to initialize.</param>
        /// <param name="cantones">Cantons to use to put stores into.</param>
        /// <param name="stores">List of stores to initialize.</param>
        /// <param name="product">Products that are sold in the store.</param>
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

        /// <summary>
        /// Initialize the users data in the database.
        /// </summary>
        /// <param name="context">Context to initialize.</param>
        /// <param name="users">List of users to initialize.</param>
        /// <param name="cantones">Cantons to use as users default location.</param>
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
                NormalizedUserName = "JULIO444", Email = "julio444@ucr.ac.cr", NormalizedEmail = "JULIO444@UCR.AC.CR"
                , PasswordHash = "AQAAAAIAAYagAAAAEBSumaRyX1siCcQ3b8TXll5Km5TWXJry6oq6euj1Bj1JaKgVWIat1jxDrhwvSwZJwA==", 
                Location = cantones[0]});
            users.Add(new User { Id = "f4232f1d-5696-4537-b0a1-02fd36e27dd7", UserName = "Alonso111", 
                NormalizedUserName = "ALONSO111", Email = "alonso111@gmail.com", NormalizedEmail = "ALONSO111@GMAIL.COM"
                , PasswordHash = "AQAAAAIAAYagAAAAEDcvQfXxtswPnX0ixY3T5EipIoGSk85MVeGdNAVs0tGrkfgseQnfRidNltc5BFSFvw==", 
                Location = cantones[0]});
            context.Users.AddRange(users);
            context.SaveChanges();
        }

        /// <summary>
        /// Initialize the registers data in the database.
        /// </summary>
        /// <param name="context">Context to initialize.</param>
        /// <param name="registers">List of registers to initialize.</param>
        /// <param name="users">Users to asociate with each register.</param>
        /// <param name="products">Products wich the registers refer to.</param>
        /// <param name="stores">Stores wich the registers refer to.</param>
        public static void InitializeRegisters(LoCoMProContext context
            , ref List<Register> registers, ref List<User> users
            , ref List<Product> products, ref List<Store> stores)
        {
            string comment = "Lorem ipsum dolor sit amet, consectetur adipiscing elit" +
                ", sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Elementum pulvinar etiam non quam. ";

            List<int> basePrice = new() { 2500, 2000, 20000, 300000, 1100000 };

            /* Generates the registers by using the index of the products, users, stores and the dates */
            for (int productIndex = 0; productIndex < products.Count; productIndex++)
            {
                for (int usersIndex = 0; usersIndex < users.Count; usersIndex++)
                {
                    for (int storeIndex = 0; storeIndex < stores.Count; storeIndex++)
                    {
                        registers.Add(new Register() { Product = products[productIndex], Contributor = users[usersIndex], Store = stores[storeIndex]
                        , Price = (basePrice[productIndex] + ((basePrice[productIndex] / 25) * usersIndex * storeIndex))
                        , SubmitionDate = new DateTime(2023, 1 + usersIndex + storeIndex, 10 + productIndex + usersIndex + storeIndex, 12, 0, 0, DateTimeKind.Utc)
                        , Comment = comment });
                    }
                }
            }

            context.Registers.AddRange(registers);
            context.SaveChanges();
        }

    }

}