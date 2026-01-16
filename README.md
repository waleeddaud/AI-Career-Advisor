# AI Career Advisor

An intelligent web application that provides personalized career guidance and development plans using Google's Gemini AI. The system helps users map out their career paths based on their education, skills, interests, and goals.

## Features

### Core Functionality
- **AI-Powered Career Planning**: Generates personalized career development plans using Google Gemini 2.5 Flash
- **User Profile Management**: Comprehensive career details collection including education, skills, interests, and goals
- **Real-time Chat**: SignalR-based chat functionality for interactive career guidance
- **Admin Dashboard**: User management and system oversight capabilities
- **Role-Based Access Control**: Separate permissions for Admin and User roles with claim-based authorization

### Technical Features
- **Authentication & Authorization**: ASP.NET Core Identity with role-based and claim-based policies
- **Database Management**: SQL Server with Entity Framework Core migrations
- **Logging**: Structured logging with Serilog (file-based logging)
- **API Integration**: Google GenAI integration with retry logic for handling API rate limits
- **Unit Testing**: Comprehensive test coverage using xUnit and Moq

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 (MVC)
- **Database**: SQL Server
- **ORM**: Entity Framework Core 9.0
- **Data Access**: Dapper (for optimized queries)
- **Authentication**: ASP.NET Core Identity

### AI & Real-time Communication
- **AI Service**: Google Gemini API (gemini-2.5-flash)
- **Real-time**: SignalR for WebSocket communication
- **Markdown Processing**: Markdig for content rendering

### Logging & Monitoring
- **Logging Framework**: Serilog with file sink
- **Log Storage**: File-based logs in `logs/` directory

### Testing
- **Test Framework**: xUnit
- **Mocking**: Moq

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full installation)
- Google Gemini API key
- Visual Studio 2022 or VS Code

## Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd AI-Career-Advsior
```

### 2. Configure Database Connection
Update `appsettings.json` with your SQL Server connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CareerAdvisorDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 3. Configure Google Gemini API
Add your Gemini API key to `appsettings.json`:
```json
{
  "GeminiSettings": {
    "apikey1": "your-primary-api-key",
    "apikey2": "your-backup-api-key"
  }
}
```

### 4. Apply Database Migrations
```bash
cd CareerAdvisorApp
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`

## Project Structure

```
CareerAdvisorApp/
├── Controllers/
│   ├── AdminController.cs       # Admin panel and user management
│   ├── CareerController.cs      # Career planning and AI integration
│   ├── DashboardController.cs   # User dashboard
│   └── HomeController.cs        # Landing and public pages
├── Models/
│   ├── CareerDetails.cs         # Career profile data model
│   ├── CareerPlan.cs            # Generated career plan model
│   ├── Services/
│   │   ├── CareerService.cs     # AI career planning service
│   │   └── AdminService.cs      # Admin operations service
│   └── Interfaces/              # Service contracts
├── Hubs/
│   └── ChatHub.cs               # SignalR hub for real-time chat
├── Data/
│   ├── ApplicationDbContext.cs  # EF Core context
│   └── IdentitySeed.cs          # Initial roles and users
├── Views/                       # Razor views
├── wwwroot/                     # Static files
└── Migrations/                  # EF Core migrations

CareerAdvisorApp.Tests/
└── CareerServiceTests.cs        # Unit tests for career service
```

## User Roles & Permissions

### Admin Role
- **Claims**: `Permission: FullAccess`
- **Capabilities**:
  - View all users and their roles
  - Manage user permissions
  - Access admin dashboard
  - View system statistics

### User Role
- **Claims**: `Permission: BasicAccess`
- **Capabilities**:
  - Create and manage career profile
  - Generate AI-powered career plans
  - Access personalized dashboard
  - Use real-time chat features

## Running Tests

```bash
cd CareerAdvisorApp.Tests
dotnet test
```

## Database Models

### CareerDetails
Stores user career information:
- Education Level
- Skills
- Interests
- Career Goals
- Experience
- Industry
- Work Style
- Salary Expectations
- Timeline

### CareerPlan
Stores generated career development plans with AI recommendations

## API Integrations

### Google Gemini API
- **Model**: gemini-2.5-flash
- **Purpose**: Generate personalized career development plans
- **Features**: 
  - Automatic retry logic for rate limiting
  - Fallback API key support
  - Streaming response handling

## Logging

Logs are stored in the `logs/` directory with the following formats:
- `log{YYYYMMDD}.txt` - General application logs
- `error{YYYYMMDD}.txt` - Error-specific logs

## Known Issues & Limitations

- SignalR integration is functional but could be enhanced with additional features
- Token usage tracking is not yet implemented

## Future Enhancements

### Planned Features
- **Chat Memory**: Add conversation history and context awareness to the AI chatbot
- **Token Logger**: Track API token usage per user to prevent system abuse
- **Admin Analytics**: Display token usage statistics in admin dashboard
- **Enhanced View Components**: Add reusable UI components
- **Extended Test Coverage**: More comprehensive unit and integration tests
- **User Analytics**: Career plan effectiveness tracking
- **Notification System**: Email/SMS alerts for career milestones

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is proprietary. All rights reserved.

## Authors

Development Team - Initial work and ongoing maintenance

## Acknowledgments

- Google Gemini AI for powering the career recommendations
- ASP.NET Core team for the excellent web framework
- SignalR for real-time communication capabilities 