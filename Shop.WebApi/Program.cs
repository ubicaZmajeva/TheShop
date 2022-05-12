using MediatR;
using Shop.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IRepository, Db>();
builder.Services.AddSingleton<ICachedSupplier, CachedSupplier>();
builder.Services.AddTransient<IArticleProvider, Warehouse>();
builder.Services.AddHttpClient<IArticleProvider, DealerConnector>(
    cfg => cfg.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Dealer1Url")));
builder.Services.AddHttpClient<IArticleProvider, DealerConnector>(
    cfg => cfg.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Dealer2Url")));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(typeof(Program).Assembly);


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