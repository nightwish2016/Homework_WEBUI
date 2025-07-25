# AOS UI Automation

This project contains automated UI tests for the Advantage Online Shopping (AOS) website using Selenium WebDriver with C# and NUnit framework.

## Project Overview

This automation framework is designed to test the shopping cart functionality of the AOS website, including:
- Adding products to cart
- Verifying product details (name, price, color, quantity)
- Validating cart totals
- Cart management operations

## Prerequisites

Before running this project, ensure you have the following installed:

- **.NET 5.0 or higher** (recommended .NET 6.0 or .NET 8.0)
-  **Visual Studio Code**
- **Chrome Browser** (latest version)
- **ChromeDriver** (automatically managed by Selenium WebDriver)

## Project Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd AOS_UI_Automation
```

### 2. Install Dependencies

#### Using Command Line (dotnet CLI)

Navigate to the project directory and run:

```bash
# Restore NuGet packages
dotnet restore

# Build the project
dotnet build

# Run tests
dotnet test
```



#### Using Visual Studio Code

1. Open the project folder in VS Code
2. Install the C# extension if not already installed
3. Open terminal and run:
   ```bash
   dotnet restore
   dotnet build
   ```

### 3. Dependencies

The project uses the following NuGet packages (automatically managed):

- **NUnit** (3.13.3) - Testing framework
- **NUnit3TestAdapter** (4.3.0) - Test adapter for NUnit
- **Selenium.WebDriver** (4.15.0) - WebDriver for browser automation
- **Selenium.Support** (4.15.0) - Selenium support libraries
- **DotNetSeleniumExtras.WaitHelpers** (3.11.0) - Wait helpers for Selenium

## Running Test Cases

### Command Line Execution

#### Run All Tests
```bash
dotnet test
```



## Test Cases Overview

### 1. AddProductToCartAndVerify
- **Purpose**: Add products to cart and verify details
- **Parameters**: Product name, quantity, color, price, category
- **Test Cases**:
  - HP ZBook 17 G2 Mobile Workstation (Laptop)
  - HP Z8000 Bluetooth Mouse
  - HP Elite x2 1011 G1 Tablet

### 2. VerifyCartTotalQuantity
- **Purpose**: Verify total number of items in cart
- **Validation**: Compares expected vs actual cart quantity

### 3. VerifyCartTotalAmount
- **Purpose**: Verify total cart amount
- **Validation**: Compares expected vs actual cart total

## Test Data

The project includes hardcoded test data for:
- Product names and categories
- Quantities and prices
- Color specifications

## Configuration

### Browser Settings
- **Browser**: Chrome (automatically managed)
- **Window**: Maximized
- **Timeout**: 30 seconds for element waits

### Test Environment
- **Base URL**: https://www.advantageonlineshopping.com/
- **Test Data**: Built-in product data array

## Test Reports

### HTML Reports
Test results are automatically generated in HTML format:
- Location: `TestReport/TestReport.html`
- Contains detailed test execution information

### TRX Files
NUnit generates TRX files in the `TestResults/` directory:
- Contains test execution details
- Can be opened in Visual Studio Test Results window

  ```bash
  dotnet test --logger "trx;LogFilePath=TestResults.trx"
  trxlog2html -i TestResults\xxx.trx -o TestReport\TestReport.html
  ```
report screen:
https://github.com/nightwish2016/Homework_WEBUI/blob/main/AOS_UI_Automation/TestReport/TestReport.html
https://github.com/nightwish2016/Homework_WEBUI/blob/main/report.png

  

