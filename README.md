# Expense Tracker API

A production-ready RESTful API built with **C#**, **Azure Functions**, and **Azure Cosmos DB** for managing personal expenses with real-time analytics.

## 🚀 Features

- **CRUD Operations**: Create, read, update, and delete expense records
- **Category-Based Analytics**: Real-time spending summaries grouped by category
- **Serverless Architecture**: Built on Azure Functions for automatic scaling
- **NoSQL Database**: Leverages Azure Cosmos DB for high performance and global distribution
- **Monitoring & Logging**: Integrated Application Insights for telemetry
- **RESTful Design**: Follows REST principles with proper HTTP methods and status codes

## 🏗️ Architecture
```
Client → Azure Functions (HTTP Triggers) → Cosmos DB
                ↓
        Application Insights (Monitoring)
```

**Tech Stack:**
- **Runtime**: .NET 8 (Isolated Worker Model)
- **Compute**: Azure Functions (Consumption Plan)
- **Database**: Azure Cosmos DB (NoSQL)
- **Monitoring**: Azure Application Insights
- **Storage**: Azure Blob Storage

## 📋 Prerequisites

- .NET 8 SDK
- Azure Subscription
- Azure Functions Core Tools
- Visual Studio Code (or Visual Studio 2022)

## 🔧 Local Development Setup

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/ExpenseTrackerAPI.git
cd ExpenseTrackerAPI
```

2. **Install dependencies**
```bash
dotnet restore
```

3. **Configure local settings**
Create a `local.settings.json` file:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "CosmosDBConnectionString": "YOUR_COSMOS_CONNECTION_STRING",
    "CosmosDBDatabaseName": "ExpenseTrackerDB",
    "CosmosDBContainerName": "Expenses"
  }
}
```

4. **Run locally**
```bash
func start
```

## 🌐 API Endpoints

### Base URL
- **Local**: `http://localhost:7071/api`
- **Production**: `https://expensetracker-hz-g9caaqc9b2cgfydg.centralus-01.azurewebsites.net/api`

### Endpoints

#### Create Expense
```http
POST /expenses
Content-Type: application/json

{
  "userId": "user123",
  "amount": 50.99,
  "category": "Food",
  "description": "Lunch at restaurant",
  "date": "2026-01-29T12:00:00Z"
}
```

**Response:** `201 Created`
```json
{
  "id": "3f2504e0-4f89-41d3-9a0c-0305e82c3301",
  "userId": "user123",
  "amount": 50.99,
  "category": "Food",
  "description": "Lunch at restaurant",
  "date": "2026-01-29T12:00:00Z",
  "createdAt": "2026-01-29T15:30:00Z",
  "receiptUrl": null
}
```

#### Get All Expenses
```http
GET /expenses?userId=user123
```

**Response:** `200 OK`
```json
[
  {
    "id": "3f2504e0-4f89-41d3-9a0c-0305e82c3301",
    "userId": "user123",
    "amount": 50.99,
    "category": "Food",
    "description": "Lunch at restaurant",
    "date": "2026-01-29T12:00:00Z",
    "createdAt": "2026-01-29T15:30:00Z",
    "receiptUrl": null
  }
]
```

#### Get Expense by ID
```http
GET /expenses/{id}?userId=user123
```

#### Update Expense
```http
PUT /expenses/{id}?userId=user123
Content-Type: application/json

{
  "userId": "user123",
  "amount": 60.00,
  "category": "Food",
  "description": "Updated description",
  "date": "2026-01-29T12:00:00Z"
}
```

#### Delete Expense
```http
DELETE /expenses/{id}?userId=user123
```

**Response:** `204 No Content`

#### Get Spending Summary
```http
GET /summary?userId=user123
```

**Response:** `200 OK`
```json
{
  "totalSpent": 247.24,
  "expenseCount": 5,
  "categoryBreakdown": [
    {
      "category": "Entertainment",
      "totalAmount": 120.00,
      "count": 1,
      "averageAmount": 120.00
    },
    {
      "category": "Food",
      "totalAmount": 86.74,
      "count": 2,
      "averageAmount": 43.37
    },
    {
      "category": "Transport",
      "totalAmount": 40.50,
      "count": 2,
      "averageAmount": 20.25
    }
  ]
}
```

## 🎯 Key Technical Implementations

### Dependency Injection
Uses .NET's built-in DI container for loose coupling and testability:
```csharp
services.AddSingleton<IExpenseRepository, CosmosDbRepository>();
```

### Repository Pattern
Abstracts data access logic for maintainability:
```csharp
public interface IExpenseRepository
{
    Task<Expense> CreateAsync(Expense expense);
    Task<IEnumerable<Expense>> GetAllAsync(string userId);
    // ... more methods
}
```

### Async/Await
Non-blocking I/O operations for scalability:
```csharp
public async Task<HttpResponseData> CreateExpense([HttpTrigger] HttpRequestData req)
{
    var expense = await _repository.CreateAsync(expense);
    // ...
}
```

### Partition Key Strategy
Optimized Cosmos DB queries using userId as partition key for data locality and query performance.

## 🚀 Deployment

### Deploy to Azure

1. **Login to Azure**
```bash
az login
```

2. **Deploy Function App**
```bash
func azure functionapp publish expensetracker-hz
```

3. **Configure App Settings**
```bash
az functionapp config appsettings set --name expensetracker-hz \
  --resource-group ExpenseTrackerRG \
  --settings CosmosDBConnectionString="YOUR_CONNECTION_STRING"
```

## 📊 Monitoring

Access Application Insights for:
- Request/response times
- Failure rates
- Custom metrics
- Live metrics stream

## 🔐 Security Features

- **Function-level authorization**: Requires API keys for all endpoints
- **Partition isolation**: Users can only access their own data
- **Parameterized queries**: SQL injection prevention
- **HTTPS only**: Enforced in production

## 💰 Cost Optimization

- **Consumption Plan**: Pay only for execution time
- **Cosmos DB Free Tier**: 1000 RU/s included
- **Estimated Monthly Cost**: $0 for light usage (within free tiers)

## 📈 Future Enhancements

- [ ] Receipt image upload to Azure Blob Storage
- [ ] Azure AD B2C authentication
- [ ] Email notifications for spending limits
- [ ] Export data to CSV/Excel
- [ ] Multi-currency support
- [ ] Budget tracking and alerts

## 🧪 Testing

Run unit tests:
```bash
dotnet test
```

## 📝 License

This project is licensed under the MIT License.

## 👤 Author

**Your Name**
- LinkedIn: [Your LinkedIn](https://linkedin.com/in/haoqzhu)
- GitHub: [Your GitHub](https://github.com/haoqzhu)
- Portfolio: [Your Portfolio](https://yourwebsite.com)

## 🙏 Acknowledgments

Built as a demonstration of modern cloud-native development practices using Microsoft Azure services.