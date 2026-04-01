# 🎟️ Tazkarti (تذكرتي) - Professional Ticket Reservation System

![Tazkarti Banner](file:///C:/Users/Hazem%20Khalifa/.gemini/antigravity/brain/0b9ab02a-1788-47ea-8319-c124d0366668/tazkarti_banner_1775064222348.png)

## 📖 Overview

**Tazkarti** is a high-performance, feature-rich ticket reservation platform built with **ASP.NET Core 10**. Designed with modern architecture and user experience in mind, it provides a seamless interface for users to browse events, book tickets, and manage their reservations in a multilingual environment (English & Arabic).

The system emphasizes **design excellence**, **localization**, and **robustness**, featuring a clean N-Tier architecture and advanced client-side features like high-fidelity PDF ticket generation.

---

## ✨ Key Features

- 🌍 **Full Localization (i18n)**: Seamless switching between English and Arabic with full RTL (Right-to-Left) support.
- 🖨️ **High-Fidelity PDF Tickets**: Download tickets as beautifully designed PDFs using `jsPDF` and `html2canvas`, featuring:
  - Dynamic QR codes (Ready for expansion).
  - Custom sawtooth design dividers.
  - Correct Arabic text reshaping and rendering in PDF.
- 🌓 **Adaptive UI**: Premium design with support for both **Light and Dark modes**.
- 🔐 **Secure Authentication**: Built on ASP.NET Core Identity for secure user registration, login, and role management.
- 🏗️ **Professional Architecture**: Implements **N-Tier Architecture** with Repository and Unit of Work patterns.
- 📱 **Fully Responsive**: Mobile-first design ensuring a great experience on any device.
- 📊 **Dashboards**: Dedicated interfaces for users and administrators.

---

## 🛠️ Technology Stack

| Layer | Technologies |
| :--- | :--- |
| **Backend** | C#, .NET 10, ASP.NET Core MVC |
| **Database** | SQL Server, Entity Framework Core 10.0 (Code First) |
| **Architecture** | N-Tier, Repository Pattern, Unit of Work, Dependency Injection |
| **Frontend** | HTML5, CSS3 (Vanilla), JavaScript, Bootstrap 5 |
| **Libraries** | AutoMapper, Serilog, jsPDF, html2canvas, Arabic Reshaper |
| **Identity** | Microsoft ASP.NET Core Identity |

---

## 🏗️ Project Structure

The solution follows a clean separation of concerns:

- **`Tazkarti` (UI Layer)**: ASP.NET Core MVC project handling controllers, views, view models, and client-side logic.
- **`BLL` (Business Logic Layer)**: Contains repository interfaces, concrete implementations, and the Unit of Work.
- **`DAL` (Data Access Layer)**: Manages database context, migrations, and core entities.
- **`Tazkarti.Tests`**: Unit and integration tests for ensuring system reliability.

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or Developer edition)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (latest preview or version supporting .NET 10)

### Installation

1.  **Clone the repository**:
    ```bash
    git clone https://github.com/your-username/Tazkarti.git
    cd Tazkarti
    ```

2.  **Update Database Connection**:
    Open `Tazkarti/appsettings.json` and update the `MyConnection` string.

3.  **Apply Migrations**:
    Open Package Manager Console and run:
    ```powershell
    Update-Database -Project DAL -StartupProject Tazkarti
    ```

4.  **Run the Application**:
    ```bash
    dotnet run --project Tazkarti
    ```

---

## 📸 Screenshots

| Home Page | Ticket Generation |
| :---: | :---: |
| *[Dynamic Landing Page with Event Cards]* | *[High-Fidelity PDF Export]* |

---

## 📝 License

Distributed under the MIT License. See `LICENSE` for more information.

---

<p align="center">
  Generated with ❤️ for a premium user experience.
</p>
