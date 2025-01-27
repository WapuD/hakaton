using hakaton_WEB.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Добавление клиента Refit
builder.Services
    .AddRefitClient<IApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7283/api"));

// Добавление кэша для хранения сессий
builder.Services.AddDistributedMemoryCache(); // Добавление кэша для сессий
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Установите таймаут сессии
    options.Cookie.HttpOnly = true; // Защитите куки от доступа через JavaScript
    options.Cookie.IsEssential = true; // Сделайте куки необходимыми
});

// Добавление Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// Конфигурация HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Включение поддержки сессий
app.UseSession(); // Включите поддержку сессий

app.UseAuthorization();

app.MapRazorPages();

app.Run();
