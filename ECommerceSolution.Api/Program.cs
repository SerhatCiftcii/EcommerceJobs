using ECommerceSolution.Api.Authorization;
using Application.Interfaces.Services;
using ECommerceSolution.Core.Application.Interfaces;
using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Application.Interfaces.Services;
using ECommerceSolution.Infrastructure.Jobs;
using ECommerceSolution.Infrastructure.Persistence;
using ECommerceSolution.Infrastructure.Persistence.Repositories;
using ECommerceSolution.Infrastructure.Services;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NUnit.Framework;
using Swashbuckle.AspNetCore.Filters;
using System.Text;



var builder = WebApplication.CreateBuilder(args);


// VERİTABANI VE SERVİS KATMANLARI KAYITLARI

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
//YeniKayıt Admin için 
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
builder.Services.AddScoped<IDailyReportRepository, DailyReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddTransient<ReportingJobs>();

//  JWT KİMLİK DOĞRULAMA YAPILANDIRMASI

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key appsettings.json'da eksik veya boş.");
}

var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddHangfireServer();
//  SWAGGER + JWT ENTEGRASYONU

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Swagger JWT desteği
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Bearer token ile yetkilendirme. Örnek: 'Bearer {token}'",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();

    // API bilgileri
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "E-Commerce Solution API",
        Version = "v1",
        Description = "ECommerceSolution API (JWT + Onion Architecture)",
        Contact = new OpenApiContact
        {
            Name = "Serhat",
            Email = "serhat@example.com"
        }
    });
});

// Tüm domainlere izin veren CORS (geliştirme için)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// UYGULAMA MIDDLEWARE

var app = builder.Build();

// --- DB Seeding (Başlangıç Verisi Ekleme) ---
using (var scope = app.Services.CreateScope())
{
    try
    {
        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        await seeder.SeedAdminUserAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[SEED ERROR]: Admin oluşturulurken hata oluştu: {ex.Message}");
        
    }
}
// --------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce Solution API v1");
        c.RoutePrefix = string.Empty; // root (/) adresinde Swagger açılır
    });
}

app.UseHttpsRedirection();

app.UseAuthentication(); // JWT doğrulama
app.UseAuthorization();  // Yetki kontrolü

app.UseCors("AllowAll");
app.MapControllers();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});
// 1. Günlük Rapor (Her gün sabah 05:00'te)
RecurringJob.AddOrUpdate<ReportingJobs>(
    "DailySalesReport",
    x => x.CalculateDailyReport(),
    Cron.Daily(5), // Her gün 05:00
    new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time") }
);

// 2. Aylık Rapor (Her Ayın 1. Günü saat 04:00'te)
RecurringJob.AddOrUpdate<ReportingJobs>(
    "MonthlySalesReport",
    x => x.CalculateMonthlySalesReport(),
    Cron.Monthly(1, 4), // Ayın 1. günü, 04:00
    new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time") }
);

// 3. Stok Senkronizasyonu (Her Gün Gece Yarısı 03:00'te)
RecurringJob.AddOrUpdate<ReportingJobs>(
    "StockSync",
    x => x.SyncStockQuantities(),
    Cron.Daily(3),
    new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time") }
);
app.Run();
