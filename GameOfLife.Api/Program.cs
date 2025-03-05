using Couchbase;
using GameOfLife.Application.DTOs;
using GameOfLife.Application.Repositories;
using GameOfLife.Application.Services;
using GameOfLife.Infrastructure.Repositories;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<CouchbaseSettings>(builder.Configuration.GetSection("Couchbase"));

builder.Services.AddSingleton<ICluster>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<CouchbaseSettings>>().Value;
    return Cluster.ConnectAsync(settings.ConnectionString, settings.UserName, settings.Password)
        .GetAwaiter().GetResult();
});

builder.Services.AddScoped<IBoardRepository, BoardRepository>();
builder.Services.AddScoped<IGameOfLifeService, GameOfLifeService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }