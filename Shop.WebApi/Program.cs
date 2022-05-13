using System.Diagnostics.CodeAnalysis;
using MediatR;
using Shop.WebApi.Models;
using Shop.WebApi.Services.ArticleProviders;
using Shop.WebApi.Services.ArticleProviders.Core;
using Shop.WebApi.Services.Cache;
using Shop.WebApi.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IRepository<ArticleEntity>, Repository<ArticleEntity>>();
builder.Services.AddSingleton<ICachedSupplier>(a => new CachedSupplier(new Repository<Article>()));

builder.Services.AddTransient<IArticleProvider, Warehouse>();
builder.Services.AddHttpClient<IArticleProvider, DealerConnector>(m => m.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Dealer1Url")));
builder.Services.AddHttpClient<IArticleProvider, DealerConnector>(m => m.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Dealer2Url")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

[ExcludeFromCodeCoverage(Justification = "Nothing to test here")]
public partial class Program { }