# 🎟️ Tazkarti (تذكرتي) - Tier-1 Ticket Reservation System

## 📖 Overview

**Tazkarti** is a high-performance, enterprise-grade ticket reservation platform built with **ASP.NET Core 10**. Designed with modern **N-Tier Architecture** and user experience at its core, it provides a seamless interface for users to discover events, book tickets securely, and manage their reservations in a fully localized, multilingual environment (English & Arabic).

The system emphasizes **design excellence**, **bi-directional layout support (LTR/RTL)**, and **robustness**, featuring advanced client-side features like high-fidelity PDF ticket generation with correct Arabic text reshaping.

---

## ✨ Key Features

- 💳 **Secure Payments**: Integrated with **Stripe Checkout** for industry-standard secured credit card transactions.
- 🌍 **Full Localization (i18n)**: One-click switching between English and Arabic with full **RTL (Right-to-Left)** support.
- 🖨️ **High-Fidelity PDF Tickets**: Client-side PDF generation using `jsPDF` and `html2canvas`, featuring:
  - **Arabic Text Reshaping**: Fixed layout issues for Arabic characters in PDFs.
  - **Dynamic QR Codes**: Ready for gate verification.
  - **Custom Aesthetics**: Sawtooth dividers and premium design.
- 🌓 **Dynamic Theme Engine**: Premium UI with support for both **Light and Dark modes**.
- 🏗️ **Professional Architecture**: Implements **N-Tier Architecture** with Repository and Unit of Work patterns for maximum scalability.
- 📱 **Responsive Design**: Mobile-first approach ensuring a premium experience on any device.

---

## 🛠️ Technology Stack

### 🚀 Backend & Core
- **Framework**: [.NET 10 (ASP.NET Core MVC)](https://dotnet.microsoft.com/)
- **Database**: [SQL Server](https://www.microsoft.com/en-us/sql-server/)
- **ORM**: [Entity Framework Core 10.0 (Code First)](https://learn.microsoft.com/en-us/ef/core/)
- **Identity**: [Microsoft ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity) (Auth & Role Management)
- **Mapping**: [AutoMapper](https://automapper.org/) (for Clean DTO Transitions)
- **Logging**: [Serilog](https://serilog.net/) (Structured Logging)

### 🎨 Frontend & Design
- **Styling**: [CSS3 (Vanilla)](https://developer.mozilla.org/en-US/docs/Web/CSS) + [Bootstrap 5](https://getbootstrap.com/)
- **Logic**: ES6+ JavaScript
- **PDF Generation**: [jsPDF](https://github.com/parallax/jsPDF) & [html2canvas](https://html2canvas.hertzen.com/)
- **Localization**: [reshaer-js](https://github.com/Zayan/reshaper-js) (for PDF Arabic reshaping)

### 💳 Payment Gateway
- **Provider**: [Stripe](https://stripe.com/)
- **Integration**: **Stripe.net** (Server-side) + Stripe Checkout (Client-side)
- **Features**: Secure sessions, webhooks-ready, and credit card processing.

### 🧪 Quality Assurance & Testing
- **Framework**: [xUnit](https://xunit.net/)
- **Mocking**: [Moq](https://github.com/devlooped/moq) (Mocking Repository layers and Services)
- **Coverage**: [Coverlet](https://github.com/coverlet-coverage/coverlet)
- **Scope**: Unit Testing for `BLL`, Controller Testing, and Business Logic validation.

---

## 🏗️ Project Architecture (N-Tier)

Tazkarti is built with a clear separation of concerns to ensure maintainability and testability:

- **`Tazkarti` (Presentation Layer)**: ASP.NET Core MVC handling Controllers, ViewModels, and Client-side assets.
- **`BLL` (Business Logic Layer)**: Contains Repository interfaces, concrete implementations, and the **Unit of Work** pattern.
- **`DAL` (Data Access Layer)**: Manages `ApplicationDbContext`, Migrations, and Core Domain Entities.
- **`Tazkarti.Tests`**: Dedicated test project for ensuring system reliability.

---

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (Express/Developer)
- Visual Studio 2022

### Installation & Setup

1.  **Clone the repository**:
    ```bash
    git clone https://github.com/hazemkhalifa1/Tazkarti.git
    cd Tazkarti
    ```

2.  **Configure Connection & API Keys**:
    Update `Tazkarti/appsettings.json` with your credentials:
    ```json
    {
      "ConnectionStrings": { "MyConnection": "Server=...;Database=Tazkarti;..." },
      "StripeKeys": { "Secretkey": "sk_test_...", "Publishablekey": "pk_test_..." }
    }
    ```

3.  **Run Database Migrations**:
    ```powershell
    Update-Database -Project DAL -StartupProject Tazkarti
    ```

4.  **Run the Project**:
    ```bash
    dotnet run --project Tazkarti
    ```

---

## 🧪 Running Tests

To run the full suite of unit and integration tests:
```bash
dotnet test
```

---

## 📸 Showcasing Tazkarti

| Home Page (Arabic RTL) | High-Fidelity PDF Ticket | Dark Mode Event Info |
| :---: | :---: | :---: |
| <img width="1349" alt="Home Page RTL" src="https://github.com/user-attachments/assets/c107b530-61dd-4e27-a4ba-032e8fd5eedd" /> | <img width="1121" alt="PDF Ticket" src="https://github.com/user-attachments/assets/5e40b06c-bda9-4fdf-bfc2-13054eda9be8" /> | <img width="1346" alt="Dark Mode" src="https://github.com/user-attachments/assets/98aaf1e3-2fa8-4964-8cae-e3e97328bbd5" /> |

> [!TIP]
> **Pro Tip**: Use the built-in language switcher at the top right to explore the full RTL experience.

---

## 📝 License

Distributed under the MIT License. See `LICENSE` for more information.

---

<p align="center">
  Developed by <a href="https://github.com/hazemkhalifa1">Hazem Khalifa</a> with ❤️
</p>
