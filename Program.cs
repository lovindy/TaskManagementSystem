using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.Data;
using TaskManagementSystem.Repositories;
using TaskManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Register DB Context
builder.Services.AddScoped<IDbContext>(sp =>
    new DbContext(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBoardRepository, BoardRepository>();
builder.Services.AddScoped<IBoardMemberRepository, BoardMemberRepository>();
builder.Services.AddScoped<IListRepository, ListRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// Enable CORS Middleware
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
