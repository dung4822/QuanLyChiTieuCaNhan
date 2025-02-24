# 💰 Personal Finance Management API

## 📑 Mục lục

- [Tổng quan](#-tổng-quan)
- [Tính năng chính](#-tính-năng-chính) 
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Kiến trúc hệ thống](#-kiến-trúc-hệ-thống)
- [Cài đặt và chạy](#-cài-đặt-và-chạy)
- [API Documentation](#-api-documentation)
- [Bảo mật](#-bảo-mật)
- [Tác giả](#-tác-giả)

## 🌟 Tổng quan

Personal Finance Management API là một RESTful Web API được xây dựng bằng ASP.NET Core 8 giúp người dùng theo dõi và quản lý tài chính cá nhân một cách hiệu quả. Dự án này được thiết kế với kiến trúc clean, có khả năng mở rộng và tập trung vào hiệu suất cũng như bảo mật.

## ✨ Tính năng chính

### 👤 Quản lý người dùng
- Đăng ký tài khoản với xác thực email
- Xác thực JWT với Refresh Token
- Rate limiting cho API đăng nhập
- Quản lý thông tin cá nhân

### 💳 Quản lý giao dịch
- CRUD các khoản thu chi
- Phân loại giao dịch theo danh mục
- Ghi chú chi tiết cho mỗi giao dịch
- Lọc và tìm kiếm giao dịch

### 📊 Quản lý ngân sách
- Thiết lập ngân sách theo danh mục
- Theo dõi và cảnh báo vượt ngân sách
- Báo cáo chi tiêu theo thời gian
- Thống kê 

## 🛠 Công nghệ sử dụng

### Backend
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQL Server
- AutoMapper
- Identity Framework

### Bảo mật & Xác thực
- JWT Authentication
- Refresh Token
- Rate Limiting
- Email Verification

### Development Tools
- Swagger/OpenAPI
- Docker
- Git
- Visual Studio 2022

## 🏗 Kiến trúc hệ thống

Dự án được xây dựng theo mô hình Repository Pattern với các layer:

```plaintext
src/
├── Controllers/     # API Endpoints
├── Services/        # Business Logic
├── Repository/      # Data Access
├── Models/          # Domain Models
├── DTOs/            # Data Transfer Objects
├── Middleware/      # Custom Middleware
└── Configurations/  # App Settings
```

## 💻 Cài đặt và chạy

### Yêu cầu hệ thống
- .NET 8.0 SDK
- SQL Server 2019+
- Visual Studio 2022 hoặc VS Code

### Các bước cài đặt

1. Clone dự án:
```bash
git clone https://github.com/dung4822/QuanLyChiTieuCaNhan.git
cd QuanLyChiTieuCaNhan
```

2. Cấu hình database trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your_connection_string"
  }
}
```

3. Chạy migration:
```bash
dotnet ef database update
```

4. Khởi chạy ứng dụng:
```bash
dotnet run
```

### Docker
```bash
docker build -t personal-finance-api .
docker run -p 8080:80 personal-finance-api
```

## 📚 API Documentation

API được tài liệu hóa đầy đủ với Swagger UI:
- Development: https://localhost:7125/swagger
- Production: https://api.example.com/swagger

## 🔐 Bảo mật

- JWT Authentication với refresh token
- Rate limiting cho API authentication
- Mã hóa mật khẩu với Identity Framework
- Xác thực email người dùng
- HTTPS và SSL
- Cross-Origin Resource Sharing (CORS)

## 👨‍💻 Tác giả

**Nguyễn Đức Dũng**
- GitHub: [@yourgithub](https://github.com/dung4822)

---
© 2024 Personal Finance Management API. All rights reserved.

