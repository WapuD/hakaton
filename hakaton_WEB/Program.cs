using hakaton_WEB.Data;
using Refit;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddRefitClient<IApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7283/api"));
// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapGet("/", async context =>
{
    context.Response.Redirect("/TestsPage");
});

app.Run();
