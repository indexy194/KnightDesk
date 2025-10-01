# 🏰 KnightDesk - Game Account Management Tool

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![WPF](https://img.shields.io/badge/WPF-4.7.2-purple.svg)](https://docs.microsoft.com/en-us/dotnet/framework/wpf/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-blue.svg)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-blue.svg)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

> **KnightDesk** là một công cụ quản lý tài khoản game toàn diện, cho phép quản lý nhiều tài khoản game cùng lúc thông qua giao diện desktop thân thiện và API web mạnh mẽ.

## 📋 Mục lục

- [✨ Tính năng chính](#-tính-năng-chính)
- [🏗️ Kiến trúc hệ thống](#️-kiến-trúc-hệ-thống)
- [🛠️ Công nghệ sử dụng](#️-công-nghệ-sử-dụng)
- [🚀 Cài đặt và chạy](#-cài-đặt-và-chạy)
- [📖 Hướng dẫn sử dụng](#-hướng-dẫn-sử-dụng)
- [🔧 API Documentation](#-api-documentation)
- [🎮 Game Integration](#-game-integration)
- [☁️ Deployment](#️-deployment)
- [🤝 Đóng góp](#-đóng-góp)
- [📄 License](#-license)

## ✨ Tính năng chính

### 🎯 Quản lý tài khoản game
- **Multi-account support** - Quản lý nhiều tài khoản game cùng lúc
- **Account grouping** - Nhóm tài khoản theo user và server
- **Favorite system** - Đánh dấu tài khoản yêu thích
- **Search & filter** - Tìm kiếm và lọc tài khoản nhanh chóng
- **Character management** - Quản lý nhân vật trong game

### 🖥️ Desktop Application (WPF)
- **Modern UI** - Giao diện đẹp mắt với gradient và animations
- **Real-time monitoring** - Theo dõi trạng thái game real-time
- **Auto-login** - Tự động đăng nhập game
- **Game control** - Điều khiển game từ xa
- **Settings management** - Cấu hình linh hoạt

### 🌐 Web API
- **RESTful API** - API chuẩn REST cho tất cả operations
- **Swagger documentation** - Tài liệu API tự động
- **CORS support** - Hỗ trợ cross-origin requests
- **Health monitoring** - Theo dõi sức khỏe hệ thống
- **Error handling** - Xử lý lỗi toàn diện

### 🎮 Game Integration
- **TCP Communication** - Giao tiếp real-time với game clients
- **Command system** - Hệ thống lệnh tự động hóa
- **Process management** - Quản lý lifecycle của game clients
- **Multi-client support** - Hỗ trợ nhiều game client đồng thời

## 🏗️ Kiến trúc hệ thống

```
┌─────────────────────────────────────────────────────────────┐
│                    KnightDesk System                        │
├─────────────────────────────────────────────────────────────┤
│  WPF Desktop App (Presentation Layer)                      │
│  ├── MainWindow (Navigation)                               │
│  ├── AccountPage (Account Management)                      │
│  ├── ManagerPage (Game Control)                            │
│  ├── SettingsPage (Configuration)                          │
│  └── LogScreenPage (Monitoring)                            │
├─────────────────────────────────────────────────────────────┤
│  Web API (API Layer)                                       │
│  ├── AccountsController                                    │
│  ├── UsersController                                       │
│  └── ServerInfoController                                  │
├─────────────────────────────────────────────────────────────┤
│  Core Business Logic (Domain Layer)                        │
│  ├── Entities (Account, User, ServerInfo)                 │
│  ├── Services (Business Logic)                             │
│  └── DTOs (Data Transfer Objects)                          │
├─────────────────────────────────────────────────────────────┤
│  Infrastructure Layer                                      │
│  ├── Entity Framework Core (Data Access)                  │
│  ├── PostgreSQL Database                                   │
│  └── TCP Server (Game Communication)                       │
└─────────────────────────────────────────────────────────────┘
```

## 🛠️ Công nghệ sử dụng

### Backend
- **.NET 8** - Framework chính
- **ASP.NET Core 8.0** - Web API framework
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL** - Database chính
- **AutoMapper 12.0.1** - Object mapping
- **Swagger/OpenAPI** - API documentation

### Frontend
- **WPF (.NET Framework 4.7.2)** - Desktop UI
- **XAML** - UI markup
- **MVVM Pattern** - Architecture pattern
- **FontAwesome.WPF** - Icons
- **Newtonsoft.Json** - JSON handling

### Infrastructure
- **Docker** - Containerization
- **TCP/IP** - Game communication
- **CORS** - Cross-origin support
- **Render.com** - Cloud hosting

## 🚀 Cài đặt và chạy

### Yêu cầu hệ thống
- Windows 10/11
- .NET 8.0 SDK
- .NET Framework 4.7.2
- PostgreSQL 15+
- Docker (optional)

### Cài đặt từ source code

1. **Clone repository**
```bash
git clone https://github.com/yourusername/KnightDesk.git
cd KnightDesk
```

2. **Cài đặt dependencies**
```bash
# Restore NuGet packages
dotnet restore

# Build solution
dotnet build
```

3. **Cấu hình database**
```bash
# Update connection string in appsettings.json
# Default: Server=localhost;Port=5432;Database=knightdesk;Username=postgres;Password=yourpassword;
```

4. **Chạy database migrations**
```bash
cd KnightDesk.Api
dotnet ef database update
```

5. **Chạy ứng dụng**
```bash
# Terminal 1: Start API
cd KnightDesk.Api
dotnet run

# Terminal 2: Start WPF App
cd KnightDesk.Presentation.WPF
dotnet run
```

### Chạy với Docker

```bash
# Build và chạy với Docker Compose
docker-compose up --build
```

## 📖 Hướng dẫn sử dụng

### 1. Khởi động ứng dụng
- Chạy `KnightDesk.Presentation.WPF.exe`
- Đăng nhập với tài khoản (mặc định: admin/admin)

### 2. Quản lý tài khoản
- **Thêm tài khoản**: Click "Add Account" → Nhập thông tin
- **Chỉnh sửa**: Click vào tài khoản → "Edit"
- **Xóa tài khoản**: Click "Delete" → Xác nhận
- **Đánh dấu yêu thích**: Click icon ⭐

### 3. Quản lý game
- **Khởi động game**: Click "Start Game" trên tài khoản
- **Dừng game**: Click "Stop Game"
- **Theo dõi trạng thái**: Xem real-time status
- **Gửi lệnh**: Sử dụng command panel

### 4. Cấu hình
- **Game path**: Cài đặt đường dẫn game client
- **Server settings**: Cấu hình server thông tin
- **API settings**: Cấu hình kết nối API

## 🔧 API Documentation

### Base URL
```
Development: http://localhost:5000
```

### Swagger UI
```
http://localhost:5000/swagger
```

### Endpoints chính

#### Accounts
```http
GET    /api/accounts              # Lấy danh sách tài khoản
GET    /api/accounts/{id}         # Lấy tài khoản theo ID
POST   /api/accounts/add-account  # Thêm tài khoản mới
PUT    /api/accounts/update-account # Cập nhật tài khoản
DELETE /api/accounts/{id}         # Xóa tài khoản
GET    /api/accounts/search?q={term} # Tìm kiếm tài khoản
PUT    /api/accounts/{id}/toggle-favorite # Đánh dấu yêu thích
```

#### Users
```http
GET    /api/users                 # Lấy danh sách users
GET    /api/users/{id}            # Lấy user theo ID
POST   /api/users/add-user        # Thêm user mới
PUT    /api/users/update-user     # Cập nhật user
DELETE /api/users/{id}            # Xóa user
```

#### Server Info
```http
GET    /api/serverinfo            # Lấy danh sách servers
GET    /api/serverinfo/{id}       # Lấy server theo ID
POST   /api/serverinfo/add-server # Thêm server mới
PUT    /api/serverinfo/update-server # Cập nhật server
DELETE /api/serverinfo/{id}       # Xóa server
```

### Response Format
```json
{
  "code": 200,
  "message": "Success",
  "data": { ... },
  "errors": []
}
```

## 🎮 Game Integration

### TCP Communication Protocol

KnightDesk sử dụng TCP để giao tiếp với game clients:

#### Registration
```
Client → Server: REGISTER|{accountId}
Server → Client: REGISTERED_OK
```

#### Commands
```
Server → Client: PING
Client → Server: PONG

Server → Client: LOGIN|username|password|server|character
Client → Server: CHARACTER_NAME|CharacterName

Server → Client: SHUTDOWN
Client → Server: SHUTDOWN_OK
```

### Game Client Integration

Để tích hợp game client với KnightDesk:

1. **Khởi động với parameters**:
```bash
GameClient.exe -account {AccountId} -tcpport 8888
```

2. **Implement TCP client**:
```csharp
// Connect to KnightDesk server
var client = new TcpClient();
client.Connect("127.0.0.1", 8888);

// Register with account ID
var registerMessage = $"REGISTER|{accountId}";
// Send registration and wait for confirmation
```

3. **Handle commands**:
```csharp
// Listen for commands from KnightDesk
while (connected)
{
    var command = ReadCommand();
    var response = ProcessCommand(command);
    SendResponse(response);
}
```

Chi tiết xem: [TCP Integration Guide](KnightDesk.Presentation.WPF/TCP_INTEGRATION_GUIDE.md)

## ☁️ Deployment

### Render.com Deployment

1. **Tạo PostgreSQL database** trên Render.com
2. **Deploy Web Service** với Docker
3. **Cấu hình environment variables**:
   - `DATABASE_URL` (tự động từ database)
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `ASPNETCORE_URLS=http://+:10000`

Chi tiết xem: [Deployment Guide](KnightDesk.Api/DEPLOYMENT_GUIDE.md)

### Docker Deployment

```dockerfile
# Build API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "KnightDesk.Api/KnightDesk.Api.csproj"
RUN dotnet build "KnightDesk.Api/KnightDesk.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KnightDesk.Api/KnightDesk.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KnightDesk.Api.dll"]
```

## 📁 Cấu trúc project

```
KnightDesk/
├── KnightDesk.Api/                 # Web API project
│   ├── Controllers/                # API controllers
│   ├── Program.cs                  # API startup
│   └── appsettings.json           # Configuration
├── KnightDesk.Core/                # Business logic layer
│   ├── Domain/                     # Entities và interfaces
│   ├── Application/                # Services và DTOs
│   └── Mappers/                    # AutoMapper profiles
├── KnightDesk.Infrastructure/      # Data access layer
│   ├── Data/                       # DbContext
│   ├── Repositories/               # Repository implementations
│   └── Configurations/             # EF configurations
├── KnightDesk.Presentation.WPF/    # Desktop application
│   ├── Views/                      # XAML views
│   ├── ViewModels/                 # MVVM view models
│   ├── Services/                   # Business services
│   ├── Models/                     # Data models
│   └── Constants/                  # Application constants
├── Dockerfile                      # Docker configuration
├── render.yaml                     # Render.com config
└── README.md                       # This file
```

## 🤝 Đóng góp

Chúng tôi hoan nghênh mọi đóng góp! Vui lòng:

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

### Development Guidelines

- Tuân thủ C# coding conventions
- Viết unit tests cho new features
- Update documentation khi cần
- Sử dụng meaningful commit messages

## 📄 License

Dự án này được phân phối dưới MIT License. Xem file [LICENSE](LICENSE) để biết thêm chi tiết.

## 📞 Hỗ trợ

- **Issues**: [GitHub Issues](https://github.com/yourusername/KnightDesk/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yourusername/KnightDesk/discussions)
- **Email**: support@knightdesk.com

## 🙏 Acknowledgments

- [FontAwesome](https://fontawesome.com/) - Icons
- [Material Design](https://material.io/) - UI inspiration
- [Entity Framework](https://docs.microsoft.com/en-us/ef/) - ORM
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) - Web framework

---

<div align="center">

**Made with ❤️ by KnightDesk Team**

[⭐ Star this repo](https://github.com/yourusername/KnightDesk) • [🐛 Report Bug](https://github.com/yourusername/KnightDesk/issues) • [💡 Request Feature](https://github.com/yourusername/KnightDesk/issues)

</div>