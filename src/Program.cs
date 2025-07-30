using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pandoc;
using bookApi;
var builder = WebApplication.CreateBuilder(args);

// Поддержка контроллеров
builder.Services.AddControllers();

// Подключение бд
builder.Services.AddDbContext<BookContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Маршрутизация для контроллеров
app.MapControllers();





// Минимальный API endpoint
app.MapGet("/", () => "Hello World!");

app.Run();