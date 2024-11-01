# 🚀 Search Engine Project
A high-performance search engine implementation using TF-IDF (Term Frequency-Inverse Document Frequency) scoring, built with .NET 8.

## 🛠️ Technologies Used
- .NET 8
- PostgreSQL
- Entity Framework Core
- ASP.NET Core Web API
- Memory Caching
- Project Gutenberg Integration

## 📋 Prerequisites
- .NET 8 SDK
- PostgreSQL 15 or higher
- Visual Studio 2022 or VS Code

## 📁 Project Structure
- **SearchEngine.API**: Web API endpoints and configuration
- **SearchEngine.Core**: Business logic and interfaces
- **SearchEngine.Infrastructure**: External services implementation
- **SearchEngine.Persistence**: Database context and repositories
- **SearchEngine.Shared**: DTOs and shared models
- **SearchEngine.Indexer**: Simple console project to batch load Gutenberg books

## ⚙️ Setup Instructions

1. Clone the repository:
   ```bash
   git clone [repository-url]
   cd SearchEngine
   ```

2. Update the connection string in `SearchEngine.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=searchengine;Username=your_username;Password=your_password;"
     },
     "GutenbergApi": {
       "BaseUrl": "https://www.gutenberg.org/cache/epub/"
     }
   }
   ```

3. Apply database migrations:
   ```bash
   cd SearchEngine.API
   dotnet ef database update
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

## 📡 API Endpoints

- **Index Gutenberg Book**
  ```
  POST /api/index/gutenberg/{bookId}
  ```

- **Search Documents**
  ```
  GET /api/search?query={searchQuery}&page={pageNumber}&pageSize={pageSize}
  ```

## 🌟 Features
- TF-IDF based relevance scoring
- Asynchronous document indexing
- Parallel search operations
- Memory caching for performance
- Project Gutenberg integration
- Background logging
- Pagination support

## ⚡ Performance Optimizations
- Parallel query execution
- Efficient database indexing
- Memory caching for IDF values
- Connection pooling
- Batch processing
- Asynchronous logging (Not yet implemented)

## 🤝 Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

