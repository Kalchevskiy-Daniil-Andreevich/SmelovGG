using Microsoft.EntityFrameworkCore;
using thirdLab.Models;

var builder = WebApplication.CreateBuilder(args);

// Настройка сервисов
builder.Services.AddControllersWithViews() // Добавляем поддержку MVC с представлениями
    .AddXmlSerializerFormatters(); // Добавляем поддержку XML

// Добавление контекста базы данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=students.db"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(); // Оставлено закомментированным, так как Swagger не нужен

var app = builder.Build();

// Используйте только для Swagger при необходимости в разработке
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseAuthorization();

// Настройка маршрутизации
app.UseRouting();

// Настройка конечных точек
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"); // Маршрут для страницы управления студентами
    endpoints.MapControllers(); // Маршрут для API
});

app.Run();
