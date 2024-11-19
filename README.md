# Connexien

**Connexien** is a .NET Framework MVC application that powers the [Connexien platform](https://app.connexien.com/). It provides a seamless solution for organizing holiday gift exchanges, combining security, efficiency, and advanced cloud-based features.

## Features

- **Payment Integration**: Secure and reliable payment processing with Stripe.  
- **.NET Framework 4.8**: Ensures a stable and high-performance backend.  
- **Azure Cloud Storage**: Provides scalable and secure data storage solutions.  
- **Entity Framework**: Simplifies database management and interactions.  
- **AntiXSS**: Protects against cross-site scripting attacks for enhanced security.  
- **Antlr**: Implements a powerful parser for processing structured text.  
- **ASP.NET MVC**: Enables the Model-View-Controller architecture for efficient web development.  
- **ASP.NET Razor**: Delivers dynamic and server-side rendering of HTML.

## Demo

Explore Connexien live at: [Connexien](https://app.connexien.com/)  

## Installation

Follow these steps to set up the application:

1. **Clone the repository**:  
   ```bash
   git clone https://github.com/ajcdev1227/connexien.git
   cd connexien
   ```

2. **Set up the database:**
   - Use SQL Server to create the required database.
   - Apply Entity Framework migrations to configure the schema.
3. **Configure the application:**
    - Update the web.config file with your database connection string and Stripe API keys.
    - Set up Azure storage keys for cloud storage integration.
4. **Build the project:**
    - Open the solution in Visual Studio 2022 and build the project.
5. **Run the application:**
    - Start the project using IIS Express or publish it to your preferred web server.

## Usage
1. Log in to the application using your credentials.
2. Access features such as:
   - Payment Management: Securely process transactions using Stripe.
   - Scheduling: Organize and automate holiday gift exchanges.
   - Email Notifications: Manage email communications directly from the platform.

## Technologies Used
- .NET Framework 4.8: Provides a stable foundation for the application.
- Entity Framework: Simplifies database access and management.
- Azure Cloud Storage: Ensures data is stored securely and reliably.
- AntiXSS Library: Protects against XSS vulnerabilities.
- Antlr: Enables advanced parsing capabilities for text processing.
- ASP.NET MVC: Implements the Model-View-Controller architecture.
- ASP.NET Razor: Delivers server-side rendering for dynamic HTML content.

## Security
Connexien is designed with robust security measures, including:
- Cross-Site Scripting (XSS) prevention with AntiXSS.
- Secure storage and transmission of sensitive data.
- Compliance with best practices for web application security.