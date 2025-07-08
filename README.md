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
- **JWT Token Authentication** : Built-in Identity in .NET 9.0
- **Role-Based Access Control (RBAC)**: Supports Teacher and Administrator roles with different data access levels
- **Authorization Middleware**: Uses JWT `sub` claim or client IP to uniquely identify callers.
- **Input Validation**: Comprehensive validation using Data Annotations and FluentValidation
- **SQL Injection Prevention**: Entity Framework Core parameterized queries prevent SQL injection attacks
- **Data Sanitization**: All input data is sanitized before processing
- **Rate Limiting Middleware**: 
  - Limits each user/IP to 100 requests per minute.
  - Tracks request windows using an in-memory dictionary.
  - Returns standard headers:  
    `X-RateLimit-Limit`, `X-RateLimit-Remaining`, `X-RateLimit-Reset`
  - Responds with **HTTP 429** and helpful message when the limit is exceeded.
- **Global Exception Handling**:
  - All unhandled exceptions are logged and returned in a consistent error format.
  - Avoids stack traces being exposed to the client.

## ⚡ Performance Optimization

- **Efficient Filtering and Pagination**: All list endpoints are paginated and filterable by grade, subject, or date.
- **Query Optimization**: Key fields like `Grade`, `StudentId`, `Subject` are indexed in the database schema.
- **Async/Await**: Used throughout for non-blocking DB and IO operations.
- **Rate Limiting**: Prevents abuse and overload by rejecting excessive requests gracefully.
- **Async Handling**: Middleware and all service layers use async I/O for scalable request handling.
- **Global Error Handler**: Prevents unnecessary crashes and maintains consistent response latency.

### Database Performance
- **Efficient Queries**: LINQ queries optimized with proper includes and projections
- **Indexing Strategy**: Database indexes on frequently queried columns (StudentId, Grade, Subject)
- **Pagination**: Efficient pagination using Skip/Take with total count optimization

### Async/Await Patterns
- All I/O operations use async/await for better scalability
- Database operations are non-blocking


## 📐 Architecture Decisions & Patterns Used

- **Layered Architecture**: Clean separation between Controllers, Services, Repositories, DTOs, and Models.
- **Repository Pattern**: Used for database access abstraction, making it testable and replaceable.
- **Service Layer**: Contains business logic independent of controller and data access concerns.
- **DTO Pattern**: To decouple internal domain models from API contracts, enhancing security and flexibility.
- **AutoMapper**: Simplifies transformation between Models and DTOs.
- **Validation Layer**: FluentValidation used to separate and centralize input validation logic.

---

## 🏢 Integration Considerations for Enterprise Environments

- **OpenAPI (Swagger) Spec**: Exposes full API contract for integration with frontend or external systems.
- **Role-Based Access**: Easily extendable to connect with identity providers (e.g., Azure AD, Okta).
- **Modular Design**: Each layer is testable and loosely coupled, allowing it to be containerized or extracted.
- **Environment Configs**: Supports different environments (dev/stage/prod) through `appsettings.{env}.json`.
- **Token-Based Auth**: Compatible with OAuth2 flows and bearer token schemes used in enterprise APIs.
- **Health Checks & Logging**: Extendable using ASP.NET Core Diagnostics + Serilog or Application Insights.

---

## 🌐 Scalability & Deployment Notes

- **Stateless Services**: Designed for horizontal scaling across containers or VMs.
- **Database Migrations**: EF Core Migrations + seed data enable consistent deployment across environments.

---

## 🤖 AI Tool Usage & Prompt Engineering Methodology

### Tool Used:
- **ChatGPT**

### Prompt Strategy:
- Started with general design prompts and then refined to:  
  - _“Add pagination and filtering to students endpoint”_
  - _“Generate CSV export of student data in .NET Core”_
  - _“Secure endpoint using mock JWT token and RBAC policy”_
- Generating seed data logic
- Prompted for examples of:
  - Proper DTO setup

### Critical Thinking & Review:
- All code generated by AI was reviewed, refined, or replaced to meet business logic or performance needs.
- Rejected suggestions when they lacked clarity on RBAC or state management.
- Used domain expertise to align AI output with enterprise-scale expectations.

### Learning Outcome:
- AI accelerated setup and initial scaffolding
- Helped reduce boilerplate
- Encouraged deeper understanding of scalable .NET API design

## 🙏 Acknowledgments

- Built for EdTech integration challenge
- Designed with enterprise scalability in mind
