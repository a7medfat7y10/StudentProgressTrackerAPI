# Student Progress Tracker API

A robust REST API for an EdTech student progress tracking system built with .NET Core. This backend simulates the data integration challenges schools face when consolidating information from multiple learning platforms into a unified system.

## 🚀 Quick Start

### Prerequisites
- .NET 9.0
- SQL Server Express
- Visual Studio 2022
- Postman (optional, for API testing)

### Installation & Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/a7medfat7y10/StudentProgressTrackerAPI.git
   cd StudentProgressTrackerAPI
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string**
   - Open `appsettings.json`
   - Update the `ConnectionStrings:DefaultConnection` to match your SQL Server setup
     
4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the API**
   - API Base URL: `https://localhost:7094`
   - Swagger Documentation: `https://localhost:5001/7094`

## 🏗️ Architecture Overview

### Design Patterns Used

**Repository Pattern**: Implemented to abstract data access logic and provide a clean separation between business logic and data persistence.

**Dependency Injection**: Leveraged ASP.NET Core's built-in DI container for loose coupling and better testability.

**Service Layer Pattern**: Business logic is encapsulated in service classes, keeping controllers thin and focused on HTTP concerns.

### Project Structure
```
StudentProgressTrackerAPI/
├── Controllers/           # API endpoint controllers
├── Models/               # Domain models and DTOs
├── Services/             # Business logic services
├── Data/                 # Entity Framework context and configurations
├── Migrations/           # Database migration files
├── DTOs/           # Extension methods and Dtos
├── Middleware/           # Custom middleware components
```

## 🔐 Security Implementation

### Authentication & Authorization
- **JWT Token Authentication **: Built-in Identity in .NET 9.0
- **Role-Based Access Control (RBAC)**: Supports Teacher and Administrator roles with different data access levels

### Data Protection
- **Input Validation**: Comprehensive validation using Data Annotations and FluentValidation
- **SQL Injection Prevention**: Entity Framework Core parameterized queries prevent SQL injection attacks
- **Data Sanitization**: All input data is sanitized before processing
- **Secure Error Handling**: Error responses don't expose sensitive system information

## ⚡ Performance Optimization

### Database Performance
- **Efficient Queries**: LINQ queries optimized with proper includes and projections
- **Indexing Strategy**: Database indexes on frequently queried columns (StudentId, Grade, Subject)
- **Pagination**: Efficient pagination using Skip/Take with total count optimization

### Async/Await Patterns
- All I/O operations use async/await for better scalability
- Database operations are non-blocking

## 🙏 Acknowledgments

- Built for EdTech integration challenge
- Designed with enterprise scalability in mind
