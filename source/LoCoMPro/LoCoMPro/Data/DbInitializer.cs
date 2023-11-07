using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Policy;
using LoCoMPro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;

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

            List<Provincia> provincias = new();
            List<Canton> cantones = new();
            List<Category> categories = new();
            List<Product> products = new();
            List<Store> stores = new();
            List<User> users = new();
            List<Register> registers = new();
            List<Review> reviews = new();
            List<Report> reports = new();

            //  Initialize all the database tables
            InitializeLocation(context, ref provincias, ref cantones);
            InitializeCategories(context, ref categories);
            InitializeProducts(context, ref products, ref categories);
            InitializeStores(context, ref cantones, ref stores, ref products);
            InitializeUsers(context, ref users, ref cantones);
            InitializeRegisters(context, ref registers, ref users, ref products, ref stores);
            InitializeReviews(context, ref reviews, ref registers, ref users);
            InitializeReports(context, ref reports, ref registers, ref users);

        }

        /// <summary>
        /// Initializes provinces and cantons for application.
        /// </summary>
        /// <param name="context">Context that needs initialized.</param>
        /// <param name="provinces">List of provinces that will be filled.</param>
        /// <param name="cantons">List of cantons that will be filled.</param>
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
                , PasswordHash = "AQAAAAIAAYagAAAAEAOQ/Jm+AUOvpKZDfgFCdqEhXjHl5vKkJ4wKvVpWGPv58mgk2A9hkeFuLvnKRFTLRw=="
                , EmailConfirmed = true
                , SecurityStamp = "b5485742-ed3c-4443-aa34-0159fcc778e2"
                , ConcurrencyStamp = "d8e5ae03-d369-43aa-9b98-80af46e71c12"
                , Location = cantones[0]});
            users.Add(new User { Id = "3af5899a-3957-415a-9675-be20966ba6d7", UserName = "Prueba123", 
                NormalizedUserName = "PRUEBA123", Email = "prueba123@gmail.com", NormalizedEmail = "PRUEBA123@GMAIL.COM"
                , PasswordHash = "AQAAAAIAAYagAAAAEJfi9TUT6VewLFHdzos2qZ29eaoRr4s0YjS60YhkekCR0Mzbe5LMp3sYgj+elkblVA=="
                , EmailConfirmed = true
                , SecurityStamp = "993d9233-8ba7-4c7c-930b-cb3c4d2cd674"
                , ConcurrencyStamp = "bbb4d61b-faf1-4d88-94e1-9aa2ae747da3"
                , Location = cantones[0]});
            users.Add(new User { Id = "9df2b729-89f7-448a-988d-616892794621", UserName = "geanca567", 
                NormalizedUserName = "GEANCA567", Email = "geanca567@hotmail.com", NormalizedEmail = "GEANCA567@HOTMAIL.COM"
                , PasswordHash = "AQAAAAIAAYagAAAAEPYFGrUrFDogPQ7/NslHe12CNANe6GM45VDkOotgLNRR16lUmVUh2C/xeflZ5Inivg=="
                , EmailConfirmed = true
                , SecurityStamp = "30f3bb50-5574-4010-a7a6-358b3981151e"
                , ConcurrencyStamp = "07083dad-871a-47c2-9312-aca84d8410fe"
                , Location = cantones[0]});
            users.Add(new User { Id = "af37fd1c-51bd-4200-9412-1a710949a6ad", UserName = "Dwayne123", 
                NormalizedUserName = "DWAYNE123", Email = "dwayne123@gmail.com", NormalizedEmail = "DWAYNE123@GMAIL.COM"
                , PasswordHash = "AQAAAAIAAYagAAAAEOT77XEI5fND1riqa9P5VKsRTPfAY5i3piTJ0HTzAPDA7+33lAvWvbUmerrQdXwxag=="
                , EmailConfirmed = true
                , SecurityStamp = "0a4575fb-40e3-434a-81ae-09a1a99bc628"
                , ConcurrencyStamp = "2c58260b-e162-4231-bc86-504ccdbfa8b5"
                , Location = cantones[0]});
            users.Add(new User { Id = "eabef9c3-0e08-4372-a88c-6f63075c4a0b", UserName = "Julio444", 
                NormalizedUserName = "JULIO444", Email = "julio444@ucr.ac.cr", NormalizedEmail = "JULIO444@UCR.AC.CR"
                , PasswordHash = "AQAAAAIAAYagAAAAEBSumaRyX1siCcQ3b8TXll5Km5TWXJry6oq6euj1Bj1JaKgVWIat1jxDrhwvSwZJwA=="
                , EmailConfirmed = true
                , SecurityStamp = "92355d52-a3d5-41a0-ba79-7e0658846a59"
                , ConcurrencyStamp = "06661238-1061-4053-beb1-7e9acdb68f9f"
                , Location = cantones[0]});
            users.Add(new User { Id = "f4232f1d-5696-4537-b0a1-02fd36e27dd7", UserName = "Alonso111", 
                NormalizedUserName = "ALONSO111", Email = "alonso111@gmail.com", NormalizedEmail = "ALONSO111@GMAIL.COM"
                , PasswordHash = "AQAAAAIAAYagAAAAEDcvQfXxtswPnX0ixY3T5EipIoGSk85MVeGdNAVs0tGrkfgseQnfRidNltc5BFSFvw=="
                , EmailConfirmed = true
                , SecurityStamp = "e08c3cb7-6e6e-47ad-ac47-390c40f30518"
                , ConcurrencyStamp = "3459b6a1-c848-4fd3-ad8e-dba24664fa3b"
                , Location = cantones[0]});
            users.Add(new User
            { Id = "498fc1e4-db15-4411-94e3-6511dff4a758", Role = "Moderator", UserName = "Moderador",
                NormalizedUserName = "MODERADOR", Email = "locoModerador@gmail.com",
                NormalizedEmail = "LOCOMODERADOR@GMAIL.COM",
                PasswordHash = "AQAAAAIAAYagAAAAEPeUgIjsKiJnOABson+8/zMh8gnzGVre5xjEtLO9GgV2jPSigVwKrDZ89fWRQYDbgQ=="
                , EmailConfirmed = true
                , SecurityStamp = "ff950c9e-55f0-4dd9-9926-d48dba8ae4cb"
                , ConcurrencyStamp = "0eb293cc-8be8-4a72-a51e-d352f6a6ef28"
                , Location = cantones[0]});
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
            List<string> comments = CreateComments();

            List<int> basePrice = new List<int>{ 2500, 2000, 20000, 300000, 1100000 };

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
                        , Comment = comments[GenerateRandom(0, comments.Count)] });
                    }
                }
            }

            context.Registers.AddRange(registers);
            context.SaveChanges();
        }

        /// <summary>
        /// Initialize the reviews data in the database.
        /// </summary>
        /// <param name="context">Context to initialize.</param>
        /// <param name="reviews">List of reviews to initialize.</param>
        /// <param name="registers">List of registers to initialize.</param>
        /// <param name="users">Users to associate with each register.</param>
        public static void InitializeReviews(LoCoMProContext context, ref List<Review> reviews
            , ref List<Register> registers, ref List<User> users)
        {
            for(int usersIndex = 0; usersIndex < users.Count; usersIndex += 2)
            {
                for(int registerIndex = 0; registerIndex < registers.Count; registerIndex++)
                {
                    float reviewValue = GenerateRandom(0, 6);
                    reviewValue = reviewValue == 0 ? 0.5f: reviewValue;

                    reviews.Add(new Review() { ReviewedRegister = registers[registerIndex]
                        , Reviewer = users[usersIndex]
                        , ReviewDate = new DateTime(2024, 2 + usersIndex, 15 + usersIndex, 12, 0, 0, DateTimeKind.Utc)
                        , CantonName = registers[registerIndex].CantonName!
                        , ProvinceName = registers[registerIndex].ProvinciaName!
                        , ReviewValue = reviewValue});
                }
            }

            context.Reviews.AddRange(reviews);
            context.SaveChanges();
        }


        /// <summary>
        /// Initialize the reports data in the database.
        /// </summary>
        /// <param name="context">Context to initialize.</param>
        /// <param name="reports">List of reports to initialize.</param>
        /// <param name="registers">List of registers to initialize.</param>
        /// <param name="users">Users to associate with each register.</param>
        public static void InitializeReports(LoCoMProContext context, ref List<Report> reports
            , ref List<Register> registers, ref List<User> users)
        {
            for(int registerIndex = 0 ; registerIndex < (registers.Count / (users.Count * 3)); registerIndex++) {
                reports.Add(new Report() { ReportedRegister = registers[registerIndex]
                        , Reporter = users[GenerateRandom(0, users.Count)]
                        , ReportDate = new DateTime(2024, 2, 15, 12, 0, 0, DateTimeKind.Utc)
                        , CantonName = registers[registerIndex].CantonName!
                        , ProvinceName = registers[registerIndex].ProvinciaName!, ReportState = 1});
            }

            context.Reports.AddRange(reports);
            context.SaveChanges();
        }

        /// <summary>
        /// Creates a list of comments
        /// </summary>
        public static List<string> CreateComments()
        {
            var comments = new List<string>();

            comments.Add("¡Este producto es increíble! Cumple con todas mis expectativas y más. La calidad es excepcional, y su precio es muy razonable. ¡Lo recomiendo sin dudarlo!");
            comments.Add("No puedo creer lo útil que ha resultado este producto en mi vida. Ha hecho las tareas diarias mucho más fáciles. ¡Definitivamente vale la pena cada centavo!");
            comments.Add("Me encanta este producto. Su diseño es elegante y moderno, y su rendimiento es excepcional. ¡Una compra que no me arrepiento de hacer!");
            comments.Add("Es difícil encontrar productos tan confiables como este. Me ha ahorrado tiempo y energía. ¡Definitivamente lo compraría de nuevo!");
            comments.Add("Este producto superó mis expectativas. La entrega fue rápida y el artículo llegó en perfectas condiciones. ¡Estoy muy contento con mi compra!");

            comments.Add("El producto es bastante estándar en términos de calidad y rendimiento. No me impresionó, pero tampoco me decepcionó.");
            comments.Add("Es un producto funcional que hace lo que se espera de él. No es excepcional, pero cumple su propósito.");
            comments.Add("La relación calidad-precio es justa. No es el mejor producto que he tenido, pero tampoco es el peor.");
            comments.Add("El producto llegó a tiempo y en buen estado. No tengo quejas importantes, pero tampoco tengo elogios especiales.");
            comments.Add("En general, el producto es promedio. No es algo de lo que me emocionaría mucho, pero tampoco es un desastre. Cumple con lo que se espera.");

            comments.Add("No estoy satisfecho con este producto en absoluto. La calidad es mala y no cumplió con lo que prometía. Una completa pérdida de dinero.");
            comments.Add("Me decepcionó profundamente. Parecía prometedor, pero después de usarlo, descubrí que era frágil y poco confiable. No lo recomendaría.");
            comments.Add("Este producto no duró mucho tiempo antes de empezar a tener problemas. La garantía no cubre estos defectos, lo que lo hace aún más decepcionante.");
            comments.Add("La relación calidad-precio de este producto es pésima. Pagué mucho por algo que no vale la pena. No lo compraría de nuevo.");
            comments.Add("El servicio al cliente de la marca es inexistente. Cuando intenté resolver un problema con el producto, no obtuve respuesta. ¡Una experiencia muy frustrante!");

            return comments;
        }

        /// <summary>
        /// Returns a random number
        /// </summary>
        /// <param name="lower">Lower value</param>
        /// <param name="higher">Higher value</param>
        public static int GenerateRandom(int lower, int higher)
        {
            var random = new Random();
            return random.Next(lower, higher);
        }

    }

}