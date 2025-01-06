using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuanLyChiTieuCaNhan.Access;
using QuanLyChiTieuCaNhan.Mapper;
using QuanLyChiTieuCaNhan.Middleware;
using QuanLyChiTieuCaNhan.Models;
using QuanLyChiTieuCaNhan.Repository;
using QuanLyChiTieuCaNhan.Repository.BaseRepository;
using QuanLyChiTieuCaNhan.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Thay bằng địa chỉ React.js
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Nếu cần sử dụng cookie hoặc thông tin xác thực
    });
});

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;             // Yêu cầu có số
    options.Password.RequiredLength = 8;             // Độ dài tối thiểu
    options.Password.RequireNonAlphanumeric = false;  // Yêu cầu ký tự đặc biệt
    options.Password.RequireUppercase = false;        // Yêu cầu chữ hoa
    options.Password.RequireLowercase = true;        // Yêu cầu chữ thường
                                                     // Cấu hình khóa tài khoản tạm thời
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); // Khóa trong 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3;                      // Sau 5 lần đăng nhập sai
    options.Lockout.AllowedForNewUsers = true;                        // Áp dụng cho tài khoản mới

    // Cấu hình xác nhận email
    options.SignIn.RequireConfirmedEmail = true;      // Yêu cầu email xác nhận
    // Cấu hình username
    options.User.RequireUniqueEmail = true;          // Email phải là duy nhất
})
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
        ValidAudience = builder.Configuration["JwtConfig:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:SecretKey"])
        )
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            Console.WriteLine($"Token Received: {context.Token}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAutoMapper(typeof(MappingProfile));
//vì cái này là generate nên ta phải đăng ký kiểu vậy
builder.Services.AddScoped(typeof(IBaseCRUDRepositoy<>), typeof(BaseCRUDRepositoy<>));

builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IExpenseTransactionRepository, ExpenseTransactionRepository>();
builder.Services.AddScoped<IExpenseTransactionService, ExpenseTransactionService>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IBudgetService, BudgetService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();
