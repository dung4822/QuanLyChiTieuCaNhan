# 💰 Ứng dụng Quản lý Chi Tiêu Cá Nhân






## 📑 Mục lục

- [Giới thiệu](#-giới-thiệu)
- [Tính năng](#-tính-năng)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Cài đặt](#-cài-đặt)
- [Cấu trúc dự án](#-cấu-trúc-dự-án)
- [API Documentation](#-api-documentation)
- [Đóng góp](#-đóng-góp)
- [Giấy phép](#-giấy-phép)

## 🌟 Giới thiệu

Ứng dụng Quản lý Chi Tiêu Cá Nhân là một Web API được xây dựng bằng .NET 8 giúp người dùng theo dõi và quản lý chi tiêu của họ một cách hiệu quả. Ứng dụng cung cấp các tính năng như theo dõi thu chi, phân loại chi tiêu, thiết lập ngân sách và báo cáo thống kê.

## ✨ Tính năng

- **👤 Quản lý người dùng**

  - Đăng ký và xác thực email
  - Đăng nhập với JWT
  - Quản lý thông tin cá nhân

- **📉 Quản lý giao dịch**

  - Thêm/sửa/xóa các khoản thu chi
  - Phân loại giao dịch
  - Ghi chú chi tiết

- **📊 Quản lý ngân sách**

  - Thiết lập ngân sách theo danh mục
  - Cảnh báo vượt ngân sách
  - Theo dõi chi tiêu thực tế

## 🛠 Công nghệ sử dụng

- **Backend**

  - ASP.NET Core 8.0
  - Entity Framework Core
  - SQL Server
  - AutoMapper
  - JWT Authentication

- **Tools & Libraries**

  - Swagger/OpenAPI
  - Identity Framework
  - Email Service (SMTP)

## 💻 Cài đặt

1. **Yêu cầu hệ thống**

   ```bash
   .NET 8.0 SDK
   SQL Server
   Visual Studio 2022 hoặc VS Code
   ```

2. **Clone dự án**

   ```bash
   git clone https://github.com/your-username/QuanLyChiTieuCaNhan.git

   cd QuanLyChiTieuCaNhan
   ```

3. **Cấu hình database**

   - Cập nhật connection string trong `appsettings.json`
   - Chạy migration:
     ```bash
     dotnet ef database update
     ```

4. **Chạy ứng dụng**

   ```bash
   dotnet run
   ```

## 🗋 Cấu trúc dự án

```plaintext
QuanLyChiTieuCaNhan/
├── Controllers/       # API Controllers
├── Models/            # Entity models
├── DTOs/              # Data Transfer Objects
├── Services/          # Business logic
├── Repository/        # Data access layer
├── Middleware/        # Custom middleware
├── Mapper/            # AutoMapper profiles
└── CustomExceptions/  # Custom exception handlers
```

## 📚 API Documentation

Swagger UI: [https://localhost:7125/swagger](https://localhost:7125/swagger)

