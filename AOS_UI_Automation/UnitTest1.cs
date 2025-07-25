
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOS_UI_Automation
{
    public class AOSTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [OneTimeSetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            driver.Navigate().GoToUrl("https://www.advantageonlineshopping.com/");
           
            // Clear shopping cart
            ClearCart();
        }

        // Product data class
        public class ProductData
        {
            public string Category { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public double Price { get; set; }
            public string Color { get; set; }
        }

        // Read test data from CSV file
        private List<ProductData> ReadTestDataFromCSV(string csvFilePath)
        {
            var testData = new List<ProductData>();
            
            try
            {
                var lines = File.ReadAllLines(csvFilePath);
                
                // Skip header row
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;
                    
                    var values = line.Split(',');
                    if (values.Length >= 5)
                    {
                        testData.Add(new ProductData
                        {
                            Category = values[0].Trim(),
                            ProductName = values[1].Trim(),
                            Quantity = int.Parse(values[2].Trim()),
                            Price = double.Parse(values[3].Trim()),
                            Color = values[4].Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to read CSV file: {ex.Message}");
            }
            
            return testData;
        }

        // Generic parameterized test case: Add product to cart and verify
        [Test]
        [TestCase("HP ZBook 17 G2 Mobile Workstation", 1, "GRAY", 1799.00, "laptopsImg", 
            TestName = "Add laptop to cart and verify")]
        [TestCase("HP Z8000 Bluetooth Mouse", 2, "BLACK", 50.99, "miceImg", 
            TestName = "Add mouse to cart and verify")]
        [TestCase("HP Elite x2 1011 G1 Tablet", 1, "BLACK", 1279.00, "tabletsImg", 
            TestName = "Add tablet to cart and verify")]
        [Description("Generic parameterized test: Add specified product to cart, verify product name, price, color, and quantity are correct")]
        public void AddProductToCartAndVerify(string productName, int quantity, string color, double price, string categoryImgId)
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[@translate='HOME']"))).Click();
            // Add product
            AddProduct(productName, quantity, color, categoryImgId);
            
            // Verify product in cart
            VerifyProductInCart(productName, quantity, color, price);
        }
              

 // Test data source
        private static readonly ProductData[] TestData = new ProductData[]
        {
            new ProductData { ProductName = "HP ZBook 17 G2 Mobile Workstation", Quantity = 1, Color = "GRAY", Price = 1799.00 },
            new ProductData { ProductName = "HP Z8000 Bluetooth Mouse", Quantity = 2, Color = "BLACK", Price = 50.99 },
            new ProductData { ProductName = "HP Elite x2 1011 G1 Tablet", Quantity = 1, Color = "BLACK", Price = 1279.00}
        };

        // Test case 4: Verify cart total quantity
        [Test]
        [Description("Verify cart total product quantity is correct")]
        public void VerifyCartTotalQuantity()
        {
            // Calculate expected total quantity from test data
            int expectedQuantity = TestData.Sum(item => item.Quantity);
            
            // Verify total quantity
            VerifyTotalQuantity(expectedQuantity);
        }

        // Test case 5: Verify cart total amount
        [Test]
        [Description("Verify cart total amount is correct")]
        public void VerifyCartTotalAmount()
        {
            // Calculate expected total amount from test data
            double expectedAmount = TestData.Sum(item => item.Price * item.Quantity);
            
            // Verify total amount
            VerifyCartTotal(expectedAmount);
        }

        private void AddProduct(string productName, int quantity, string color = null, string categoryImgId = null)
        {
            //wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[@translate='HOME']"))).Click();
            // If categoryImgId is not provided, use default mapping
            if (string.IsNullOrEmpty(categoryImgId))
            {
                var productCategoryMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "HP ZBOOK 17 G2 MOBILE WORKSTATION", "laptopsImg" },
                    { "HP Z8000 BLUETOOTH MOUSE", "miceImg" },
                    { "HP ELITE X2 1011 G1 TABLET", "tabletsImg" }
                };

                if (!productCategoryMap.TryGetValue(productName, out categoryImgId))
                    throw new Exception($"Category mapping not found for product {productName}");
            }

            // Click homepage category image
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id(categoryImgId))).Click();

            // Click product
            wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath($"//a[text()='{productName}']"))).Click();

            // Select color (if specified)
            if (!string.IsNullOrEmpty(color))
            {
                var colorBtn = wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath($"//span[@title='{color}']")));
                colorBtn.Click();
            }

            // Set quantity
            var qtyInput = wait.Until(ExpectedConditions.ElementIsVisible(By.Name("quantity")));
            int currentQty = int.Parse(qtyInput.GetAttribute("value"));
            if (currentQty < quantity)
            {
                var plusBtn = driver.FindElement(By.XPath("//div[@class='plus' and  @increment-value-attr='+']"));
                for (int i = currentQty; i < quantity; i++)
                {
                    plusBtn.Click();
                    Thread.Sleep(200);
                }
            }

            // Add to cart
            driver.FindElement(By.Name("save_to_cart")).Click();
            Thread.Sleep(1000);

            // After adding product, click Home button to return to homepage
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[@translate='HOME']"))).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("our_products"))); // Wait for homepage to load

        }

        private void VerifyProductInCart(string productName, int expectedQty, string expectedColor, double expectedPrice)
        {
            // Enter cart
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("menuCart"))).Click();
            // Thread.Sleep(5000);
            string str = driver.PageSource;
            Console.WriteLine(str);
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//tr[@id='product']")));
            wait.Until(driver =>
            {
                try
                    {
                        var productRows = driver.FindElements(By.XPath("//tr[@id='product']"));
                        Console.WriteLine($"Found {productRows.Count} product rows");
                        return productRows.Count > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error waiting for product rows: {ex.Message}");
                        return false;
                    }
            });

            Thread.Sleep(2000);
            var cartRows = driver.FindElements(By.XPath("//tr[@id='product']"));
            bool found = false;

            foreach (var row in cartRows)
            {
                var name = row.FindElement(By.XPath(".//h3")).Text.Trim();
                if (string.IsNullOrEmpty(name))
                    continue;

                // Use contains check to prevent product name truncation
                if (name != "")
                {
                    if (!name.ToUpper().StartsWith(productName.Substring(0, Math.Min(10, productName.Length)).ToUpper()))
                        continue;
                }
                // Quantity
                var qtyLabel = row.FindElement(By.XPath(".//label[contains(text(),'QTY')]")).Text.Trim();
                int actualQty = int.Parse(qtyLabel.Replace("QTY:", "").Trim());

                // Color
                var colorSpan = row.FindElement(By.XPath(".//label[contains(text(),'Color')]/span")).Text.Trim().ToUpper();

                // Amount
                var priceText = row.FindElement(By.XPath(".//p[contains(@class,'price')]")).Text.Replace("$", "").Trim();
                double actualPrice = double.Parse(priceText);

                // Assertions
                Assert.AreEqual(expectedQty, actualQty, $"Product {productName} quantity mismatch");
                Assert.AreEqual(expectedColor.ToUpper(), colorSpan, $"Product {productName} color mismatch");
                Assert.AreEqual(expectedPrice*expectedQty, actualPrice, 0.01, $"Product {productName} amount mismatch");

                found = true;
                break;
            }

            Assert.IsTrue(found, $"Product {productName} not found in cart");
        }

        private void VerifyTotalQuantity(int expectedTotal)
        {
            var cartCountElement = driver.FindElement(By.XPath("//span[@ng-show='cart.productsInCart.length > 0']"));
            string actualTotal = new string(cartCountElement.Text.Where(char.IsDigit).ToArray());            
            Assert.AreEqual(expectedTotal, double.Parse(actualTotal), $"Cart total product quantity mismatch, expected: {expectedTotal}, actual: {actualTotal}");
        }

        private void VerifyCartTotal(double expectedTotal)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[contains(@class,'cart-total')]")));
            var totalElement = wait.Until(driver => 
                driver.FindElement(By.XPath("//span[contains(@class,'cart-total')]"))
            );
            string totalText = totalElement.Text.Replace("$", "").Replace(",", "").Trim();
            double actualTotal = double.Parse(totalText);
            Assert.AreEqual(expectedTotal, actualTotal, 0.01, $"Cart total amount mismatch, expected: {expectedTotal}, actual: {actualTotal}");
        }

        private void ClearCart()
        {
           // Click cart button
            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("menuCart"))).Click();
            // Wait for cart popup to appear (regardless of whether there are products)
            wait.Until(driver =>
            {
                // Has products when shoppingCartContainer exists, empty prompt when no products
                return driver.FindElements(By.Id("shoppingCartContainer")).Count > 0 ||
                       driver.PageSource.Contains("Your shopping cart is empty");
            });
            Thread.Sleep(500); // Wait for animation

            // Check if cart has products (by checking if delete button exists)
            bool hasProduct = false;
            try
            {
                driver.FindElement(By.XPath("//a[@class='remove']"));
                hasProduct = true;
            }
            catch (NoSuchElementException)
            {
                hasProduct = false;
            }

            if (hasProduct)
            {
                // Loop to delete all products
                while (true)
                {
                    try
                    {
                        var removeBtn = driver.FindElement(By.XPath("//a[@class='remove']"));
                        removeBtn.Click();
                        Thread.Sleep(500);
                    }
                    catch (NoSuchElementException)
                    {
                        break;
                    }
                }
            }
           
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}

