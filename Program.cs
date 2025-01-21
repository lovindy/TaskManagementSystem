using TaskManagementSystem.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IBoardMemberRepository, BoardMemberRepository>();
builder.Services.AddScoped<IListRepository, ListRepository>();

// Register DB Context
builder.Services.AddScoped<IDbContext>(sp =>
    new DbContext(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();