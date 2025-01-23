using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using hakaton_API.Data;
using hakaton_API.Controllers.Interface;
using hakaton_API.Controllers.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DBContext' not found.")));

builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<ICompetencyService, CompetencyService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
