# StarWarsTcg.Security

## Folder structure
SWTCG.Security/
│
├── src/
│   ├── SWTCG.Security.Api/
│   │   ├── Controllers/                # Contains API controllers that handle HTTP requests and responses.
│   │   ├── Models/                     # Contains data models and DTOs (Data Transfer Objects) used in the API.
│   │   ├── Services/                   # Contains service classes that encapsulate business logic.
│   │   ├── Middleware/                 # Contains custom middleware components for request processing.
│   │   ├── Extensions/                 # Contains extension methods for various functionalities.
│   │   ├── Configurations/             # Contains configuration classes and settings for the application.
│   │   └── SWTCG.Security.Api.csproj   # Project file for the API.
│   │
│   ├── SWTCG.Security.Data/
│   │   ├── Context/                    # Contains the DbContext class for Entity Framework Core.
│   │   ├── Repositories/               # Contains repository interfaces and implementations for data access.
│   │   ├── Migrations/                 # Contains database migration files for MySQL.
│   │   └── SWTCG.Security.Data.csproj   # Project file for the data access layer.
│   │
│   ├── SWTCG.Security.Services/
│   │   ├── Interfaces/                 # Contains interfaces for services to promote loose coupling.
│   │   ├── Implementations/            # Contains concrete implementations of the service interfaces.
│   │   └── SWTCG.Security.Services.csproj # Project file for the service layer.
│   │
│   └── SWTCG.Security.Common/
│       ├── Constants/                  # Contains constant values used throughout the application.
│       ├── Enums/                      # Contains enumerations used in the application.
│       ├── Exceptions/                  # Contains custom exception classes for error handling.
│       └── SWTCG.Security.Common.csproj # Project file for common utilities and shared code.
│
├── tests/
│   ├── SWTCG.Security.Api.Tests/       # Contains unit and integration tests for the API layer.
│   ├── SWTCG.Security.Data.Tests/      # Contains unit tests for the data access layer.
│   └── SWTCG.Security.Services.Tests/   # Contains unit tests for the service layer.
│
├── docs/                                # Contains documentation related to the project.
│
├── .gitignore                           # Specifies files and directories to ignore in version control.
├── README.md                            # Project overview and setup instructions.
└── SWTCG.Security.sln                   # Solution file for the entire project.
