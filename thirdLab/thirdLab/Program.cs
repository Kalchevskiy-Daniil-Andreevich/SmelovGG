using Microsoft.EntityFrameworkCore;
using thirdLab.Models;

var builder = WebApplication.CreateBuilder(args);

// ��������� ��������
builder.Services.AddControllersWithViews() // ��������� ��������� MVC � ���������������
    .AddXmlSerializerFormatters(); // ��������� ��������� XML

// ���������� ��������� ���� ������
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=students.db"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(); // ��������� ������������������, ��� ��� Swagger �� �����

var app = builder.Build();

// ����������� ������ ��� Swagger ��� ������������� � ����������
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseAuthorization();

// ��������� �������������
app.UseRouting();

// ��������� �������� �����
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"); // ������� ��� �������� ���������� ����������
    endpoints.MapControllers(); // ������� ��� API
});

app.Run();
