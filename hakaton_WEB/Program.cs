using hakaton_WEB.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// ���������� ������� Refit
builder.Services
    .AddRefitClient<IApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7283/api"));

// ���������� ���� ��� �������� ������
builder.Services.AddDistributedMemoryCache(); // ���������� ���� ��� ������
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // ���������� ������� ������
    options.Cookie.HttpOnly = true; // �������� ���� �� ������� ����� JavaScript
    options.Cookie.IsEssential = true; // �������� ���� ������������
});

// ���������� Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// ������������ HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ��������� ��������� ������
app.UseSession(); // �������� ��������� ������

app.UseAuthorization();

app.MapRazorPages();

app.Run();
