using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pandoc;
using bookApi;
var builder = WebApplication.CreateBuilder(args);

// ��������� ������������
builder.Services.AddControllers();

// ����������� ��
builder.Services.AddDbContext<BookContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ������������� ��� ������������
app.MapControllers();





// ����������� API endpoint
app.MapGet("/", () => "Hello World!");

app.Run();