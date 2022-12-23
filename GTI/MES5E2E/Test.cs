// Generated by Selenium IDE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using NUnit.Framework;
[TestFixture]
public class Test {
  private IWebDriver driver;
  public IDictionary<string, object> vars {get; private set;}
  private IJavaScriptExecutor js;
  [SetUp]
  public void SetUp() {
    driver = new ChromeDriver();
    js = (IJavaScriptExecutor)driver;
    vars = new Dictionary<string, object>();
  }
  [TearDown]
  protected void TearDown() {
    driver.Quit();
  }
  [Test]
  public void A() {
    driver.Navigate().GoToUrl("http://localhost:59394/GenesisNewMes/ADM/Shift/ShiftMaster");
    driver.Manage().Window.Size = new System.Drawing.Size(1936, 1056);
    driver.FindElement(By.CssSelector(".el-button--success > span")).Click();
    driver.FindElement(By.CssSelector(".el-table__row:nth-child(1) .el-button")).Click();
    driver.SwitchTo().Frame(2);
    driver.FindElement(By.CssSelector(".el-switch__core")).Click();
    {
      WebDriverWait wait = new WebDriverWait(driver, System.TimeSpan.FromSeconds(30));
      wait.Until(driver => driver.FindElement(By.CssSelector(".el-button--success ")).Enabled);
    }
    driver.FindElement(By.CssSelector(".el-button--success ")).Click();
    {
      WebDriverWait wait = new WebDriverWait(driver, System.TimeSpan.FromSeconds(30));
      wait.Until(driver => driver.FindElement(By.CssSelector(".swal2-confirm")).Displayed);
    }
    driver.FindElement(By.CssSelector(".swal2-confirm")).Click();
    driver.FindElement(By.CssSelector(".el-switch__core")).Click();
  }
}
