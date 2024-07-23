using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using API;

var builder = WebApplication.CreateBuilder(args);

// Set up configuration
builder.Configuration.AddJsonFile("appsettings.json"); // Đọc cấu hình từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DbContext");

// Register database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString); // Đăng ký ApplicationDbContext với chuỗi kết nối từ appsettings.json
});

// Đọc cấu hình từ appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");

var secretKey = jwtSettings["Key"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];
var key = Encoding.ASCII.GetBytes(secretKey); // Mã hóa secret key thành mảng byte

// Đăng ký JwtTokenService với DI container
builder.Services.AddSingleton(new JwtTokenService(secretKey, issuer, audience));

builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // Không cần xác thực issuer
        ValidateAudience = false, // Không cần xác thực audience
        ValidateLifetime = true, // Xác thực thời gian sống của token
        ValidateIssuerSigningKey = true, // Xác thực khóa ký token
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key), // Khóa ký token
        ClockSkew = TimeSpan.Zero // Không cho phép chênh lệch thời gian
    };
});

// Register HttpClient
builder.Services.AddHttpClient(); // Đăng ký HttpClient cho DI container

// Register services and repositories
// builder.Services.AddScoped<RolesInterface, RolesServices>(); // Đăng ký RolesServices cho DI container với interface RolesInterface


// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Thêm Swagger để tạo tài liệu API

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("http://localhost:5173") // Cho phép yêu cầu từ localhost:5173 (ứng dụng React)
            .AllowAnyHeader() // Cho phép mọi header
            .AllowAnyMethod() // Cho phép mọi phương thức (GET, POST, PUT, DELETE, v.v.)
            .AllowCredentials()); // Cho phép gửi thông tin xác thực (cookies, headers)
});

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Sử dụng Swagger trong môi trường phát triển
    app.UseSwaggerUI(); // Sử dụng giao diện Swagger UI
    app.UseDeveloperExceptionPage(); // Sử dụng trang lỗi dành cho nhà phát triển
}
else
{
    app.UseHsts(); // Sử dụng HSTS trong môi trường sản xuất
}
app.UseSwagger(); // Sử dụng Swagger
app.UseSwaggerUI(); // Sử dụng giao diện Swagger UI
app.UseHttpsRedirection(); // Chuyển hướng các yêu cầu HTTP sang HTTPS
app.UseStaticFiles(); // Phục vụ các tệp tĩnh
app.UseRouting(); // Kích hoạt định tuyến

app.UseCors("AllowReactApp"); // Kích hoạt chính sách CORS

app.UseAuthentication(); // Kích hoạt middleware xác thực
app.UseAuthorization(); // Kích hoạt middleware phân quyền

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Định tuyến đến các controller
});

app.Run(); // Chạy ứng dụng
