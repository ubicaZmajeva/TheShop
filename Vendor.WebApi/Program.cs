using Microsoft.AspNetCore.Mvc;
using Vendor.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISupplierService, SupplierService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/Supplier/ArticleInInventory/{id:int}",
    (int id, [FromServices] ISupplierService supplierService) => supplierService.ArticleInInventory(id));

// Throwing exception from api endpoint is not really good idea, however, assumption is that this implementation
// realistically mocks real-world one. We to not have any other option, but to leave it like this and handle
// response properly on "our side of code"
app.MapGet("/Supplier/GetArticle/{id:int}",
    (int id, [FromServices] ISupplierService supplierService) =>
        supplierService.ArticleInInventory(id)
            ? supplierService.GetArticle(id)
            : throw new Exception("Article does not exist."));

app.Run();

    