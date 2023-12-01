using LoCoMPro.Data;
using LoCoMPro.Models;
using LoCoMPro.Pages;
using NUnit.Framework;
using OpenQA.Selenium.DevTools.V117.IndexedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace FilterStoreFromReportTest
{
    internal class RazorPageTests : BaseTest
    {
        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 3
        [Test]
        public void FilterStoreByProductAmountEmptyStructure()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ListReportPageModel)CreatePageModel("report_list_page");

            // Act

            // Filters the stores based in the amount of product the store has
            pageModel.FilterStoreByProductAmount();

            // Assert

            Assert.IsTrue(pageModel.StoreProducts.Count == 0);

        }

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 3
        [Test]
        public void FilterStoreByProductAmountEmptyList()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ListReportPageModel)CreatePageModel("report_list_page");

            // Set the test locations
            Provincia provinciaTest = new Provincia() { Name = "CantonTest" };
            Canton cantonTest = new Canton() { CantonName = "CantonTest", Provincia = provinciaTest };

            // Prepare the list of stores
            for (int storeIndex = 0; storeIndex < 3; storeIndex++)
            {
                pageModel.StoreProducts.Add(new Store() { Name = storeIndex.ToString(), Location = cantonTest }, new List<Register>());
            }

            // Create a list with 0 products
            pageModel.WantedProducts = new List<Product>();

            // Act

            // Filters the stores based in the amount of product the store has
            pageModel.FilterStoreByProductAmount();

            // Assert

            Assert.IsTrue(pageModel.StoreProducts.Count == 0);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 3
        [Test]
        public void FilterStoreByProductAmountTooLittleProducts()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ListReportPageModel)CreatePageModel("report_list_page");

            // Create product to test
            List<Product> products = new List<Product>();

            for (int productIndex = 0; (productIndex < 7); productIndex++)
            {
                products.Add(new Product() { Name = ("ProductTest" + productIndex.ToString()) });
            }

            // Create registers for the test
            List<Register> registers = InitRegisters();

            // Set the test locations
            Provincia provinciaTest = new Provincia() { Name = "CantonTest" };
            Canton cantonTest = new Canton() { CantonName = "CantonTest", Provincia = provinciaTest };

            // Prepare the list of stores
            for (int storeIndex = 0; storeIndex < 3; storeIndex++)
            {
                pageModel.StoreProducts.Add(new Store() { Name = storeIndex.ToString(), Location = cantonTest }
                    , new List<Register>() { registers[storeIndex] });
            }

            // Set the wanted product list of products
            pageModel.WantedProducts = products;

            // Act

            // Filters the stores based in the amount of product the store has
            pageModel.FilterStoreByProductAmount();

            // Assert

            Assert.IsTrue(pageModel.StoreProducts.Count == 0);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 3
        [Test]
        public void FilterStoreByProductAmountAllAmounts()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ListReportPageModel)CreatePageModel("report_list_page");

            // Create product to test
            List<Product> products = new List<Product>();

            for (int productIndex = 0; (productIndex < 7); productIndex++)
            {
                products.Add(new Product() { Name = ("ProductTest" + productIndex.ToString()) });
            }

            // Create registers for the test
            List<Register> registers = InitRegisters();

            // Set the test locations
            Provincia provinciaTest = new Provincia() { Name = "CantonTest" };
            Canton cantonTest = new Canton() { CantonName = "CantonTest", Provincia = provinciaTest };

            // Prepare the list of stores
            for (int storeIndex = 0; storeIndex < 3; storeIndex++)
            {
                Store storeToAdd = new Store() { Name = storeIndex.ToString(), Location = cantonTest };

                pageModel.StoreProducts.Add(storeToAdd, new List<Register>() { });

                for (int registerIndex = 0; registerIndex < storeIndex; registerIndex++)
                {
                    pageModel.StoreProducts[storeToAdd].Add(registers[registerIndex]);
                }
            }

            // Set the wanted product list of products
            pageModel.WantedProducts = products;

            // Act

            // Filters the stores based in the amount of product the store has
            pageModel.FilterStoreByProductAmount();

            // Assert

            Assert.IsTrue(pageModel.StoreProducts.Count == 1);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 3
        [Test]
        public void FilterStoreByProductEnoughAmount()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ListReportPageModel)CreatePageModel("report_list_page");

            // Create product to test
            List<Product> products = new List<Product>();

            for (int productIndex = 0; (productIndex < 7); productIndex++)
            {
                products.Add(new Product() { Name = ("ProductTest" + productIndex.ToString()) });
            }

            // Create registers for the test
            List<Register> registers = InitRegisters();

            // Set the test locations
            Provincia provinciaTest = new Provincia() { Name = "CantonTest" };
            Canton cantonTest = new Canton() { CantonName = "CantonTest", Provincia = provinciaTest };

            // Prepare the list of stores
            for (int storeIndex = 0; storeIndex < 3; storeIndex++)
            {
                Store storeToAdd = new Store() { Name = storeIndex.ToString(), Location = cantonTest };

                pageModel.StoreProducts.Add(storeToAdd, new List<Register>() { });

                for (int registerIndex = 0; registerIndex < 3; registerIndex++)
                {
                    pageModel.StoreProducts[storeToAdd].Add(registers[registerIndex]);
                }
            }

            // Set the wanted product list of products
            pageModel.WantedProducts = products;

            // Act

            // Filters the stores based in the amount of product the store has
            pageModel.FilterStoreByProductAmount();

            // Assert

            Assert.IsTrue(pageModel.StoreProducts.Count == 3);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 3
        [Test]
        public void FilterStoreByProductTooMuchProducts()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ListReportPageModel)CreatePageModel("report_list_page");

            // Create product to test
            List<Product> products = new List<Product>();

            for (int productIndex = 0; (productIndex < 7); productIndex++)
            {
                products.Add(new Product() { Name = ("ProductTest" + productIndex.ToString()) });
            }

            // Create registers for the test
            List<Register> registers = InitRegisters();

            // Set the test locations
            Provincia provinciaTest = new Provincia() { Name = "CantonTest" };
            Canton cantonTest = new Canton() { CantonName = "CantonTest", Provincia = provinciaTest };

            // Prepare the list of stores 
            for (int storeIndex = 0; storeIndex < 3; storeIndex++)
            {
                Store storeToAdd = new Store() { Name = storeIndex.ToString(), Location = cantonTest };

                pageModel.StoreProducts.Add(storeToAdd, new List<Register>() { });

                for (int registerIndex = 0; registerIndex < 10; registerIndex++)
                {
                    pageModel.StoreProducts[storeToAdd].Add(registers[registerIndex]);
                }
            }

            // Set the wanted product list of products
            pageModel.WantedProducts = products;

            // Act

            // Filters the stores based in the amount of product the store has
            pageModel.FilterStoreByProductAmount();

            // Assert

            Assert.IsTrue(pageModel.StoreProducts.Count == 0);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 3
        [Test]
        public void FilterStoreByDistanceNoDistance()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ListReportPageModel)CreatePageModel("report_list_page");

            // Set the test locations
            Provincia provinciaTest = new Provincia() { Name = "CantonTest" };
            Canton cantonTest = new Canton() { CantonName = "CantonTest", Provincia = provinciaTest };

            // Prepare the list of stores with the distances
            for (int storeIndex = 0; storeIndex < 3; storeIndex++)
            {
                Store storeToAdd = new Store() { Name = storeIndex.ToString(), Location = cantonTest };

                pageModel.StoreProducts.Add(storeToAdd, new List<Register>() { });

                pageModel.StoreDistances.Add(storeToAdd, 0);
            }

            // Act

            // Filters the stores based in the amount of product the store has
            pageModel.FilterStoreByDistance();

            // Assert

            Assert.IsTrue(pageModel.StoreProducts.Count == 3);
            Assert.IsTrue(pageModel.StoreDistances.Count == 3);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 3
        [Test]
        public void FilterStoreByDistanceAllDistance()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ListReportPageModel)CreatePageModel("report_list_page");

            // Set the test locations
            Provincia provinciaTest = new Provincia() { Name = "CantonTest" };
            Canton cantonTest = new Canton() { CantonName = "CantonTest", Provincia = provinciaTest };

            // Prepare the list of stores with the distances
            double distance = 0;

            for (int storeIndex = 0; storeIndex < 3; storeIndex++)
            {
                Store storeToAdd = new Store() { Name = storeIndex.ToString(), Location = cantonTest };

                pageModel.StoreProducts.Add(storeToAdd, new List<Register>() { });

                pageModel.StoreDistances.Add(storeToAdd, distance);

                distance += 0.44;
            }

            // Act

            // Filters the stores based in the amount of product the store has
            pageModel.FilterStoreByDistance();

            // Assert

            Assert.IsTrue(pageModel.StoreProducts.Count == 2);
            Assert.IsTrue(pageModel.StoreDistances.Count == 2);
        }

        // Test by Julio Alejandro Rodríguez Salguera C16717 | Sprint 3
        [Test]
        public void FilterStoreByDistanceTooFar()
        {
            // Arrange

            // Create an instance of the page model (pageModel) using the CreatePageModel()
            var pageModel = (ListReportPageModel)CreatePageModel("report_list_page");
            
            // Set the test locations 
            Provincia provinciaTest = new Provincia() { Name = "CantonTest" };
            Canton cantonTest = new Canton() { CantonName = "CantonTest", Provincia = provinciaTest };

            // Prepare the list of stores with the distances
            for (int storeIndex = 0; storeIndex < 3; storeIndex++)
            {
                Store storeToAdd = new Store() { Name = storeIndex.ToString(), Location = cantonTest };

                pageModel.StoreProducts.Add(storeToAdd, new List<Register>() { });

                pageModel.StoreDistances.Add(storeToAdd, 0.46);
            }

            // Act

            // Filters the stores based in the amount of product the store has
            pageModel.FilterStoreByDistance();

            // Assert

            Assert.IsTrue(pageModel.StoreProducts.Count == 0);
            Assert.IsTrue(pageModel.StoreDistances.Count == 0);
        }

    }
}
