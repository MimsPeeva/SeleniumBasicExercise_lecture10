using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;
using System;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace TestProject1
{
    [TestFixture]
    public class TestCalculator
    {
        private ChromeDriver driver;
        private WebDriverWait wait;

        [OneTimeSetUp]
        public void SetUp()
        {
            // Automatically download matching ChromeDriver
            new DriverManager().SetUpDriver(new ChromeConfig());

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--remote-debugging-port=9222");

            driver = new ChromeDriver(options);

            // Explicit wait for element presence
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            driver.Navigate().GoToUrl("https://calculatorhtml.onrender.com/");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        private IWebElement WaitAndFindElement(By by)
        {
            return wait.Until(d => d.FindElement(by));
        }

        public void PerformCalculation(string firstNumber, string operation,
                                        string secondNumber, string expectedResult)
        {
            var textBoxFirstNum = WaitAndFindElement(By.Id("number1"));
            var textBoxSecondNum = WaitAndFindElement(By.Id("number2"));
            var dropDownOperation = WaitAndFindElement(By.Id("operation"));
            var calcBtn = WaitAndFindElement(By.Id("calcButton"));
            var resetBtn = WaitAndFindElement(By.Id("resetButton"));
            var divResult = WaitAndFindElement(By.Id("result"));

            // Reset form
            resetBtn.Click();

            if (!string.IsNullOrEmpty(firstNumber))
                textBoxFirstNum.SendKeys(firstNumber);

            if (!string.IsNullOrEmpty(secondNumber))
                textBoxSecondNum.SendKeys(secondNumber);

            if (!string.IsNullOrEmpty(operation))
                new SelectElement(dropDownOperation).SelectByText(operation);

            // Calculate
            calcBtn.Click();

            // Assert result
            Assert.That(divResult.Text, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("5", "+ (sum)", "10", "Result: 15")]
        [TestCase("3.5", "- (subtract)", "1.2", "Result: 2.3")]
        [TestCase("2e2", "* (multiply)", "1.5", "Result: 300")]
        [TestCase("5", "/ (divide)", "0", "Result: Infinity")]
        [TestCase("invalid", "+ (sum)", "10", "Result: invalid input")]
        public void TestNumberCalculator(string firstNumber, string operation,
                                         string secondNumber, string expectedResult)
        {
            PerformCalculation(firstNumber, operation, secondNumber, expectedResult);
        }
    }
}
