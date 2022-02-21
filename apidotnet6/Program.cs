using apidotnet6;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIDotNet6", Description = "Teste com Minimal APIs", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIDotNet6 v1");
});


#region GET

// Obter produto pela uri
app.MapGet("/product/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    var product = context.Products
        .Include(p => p.Category)
        .Include(p => p.Tags)
        .Where(p => p.Id == id)
        .FirstOrDefault();

    if (product != null)
        return Results.Ok(product);

    return Results.NotFound();
});

#endregion

#region POST

app.MapPost("/product", (ProductRequest productReq, ApplicationDbContext context) =>
{
    var category = context.Categorys.Where(x => x.Id == productReq.CategoryId).FirstOrDefault();

    if (category != null)
    {
        var product = new Product
        {
            Code = productReq.Code,
            Name = productReq.Name,
            Description = productReq.Description,
            CategoryId = productReq.CategoryId
        };

        if (productReq.Tags != null)
        {
            product.Tags = new List<Tag>();

            foreach (var tag in productReq.Tags)
            {
                product.Tags.Add(new Tag { Name = tag });
            }

            context.Products.Add(product);
            context.SaveChanges();
            return Results.Created($"/product{product.Id}", product.Id);
        }



    }

    return Results.NotFound();

});


#endregion

#region PUT

app.MapPut("/product/{id}", (int id, ProductRequest productReq, ApplicationDbContext context) =>
{
    var product = context.Products
     .Include(p => p.Tags)
     .Where(p => p.Id == id)
     .FirstOrDefault();

    var category = context.Categorys.Where(x => x.Id == productReq.CategoryId).FirstOrDefault();

    if (category == null) return Results.NotFound();    

    product.Code = productReq.Code;
    product.Name = productReq.Name;
    product.Description = productReq.Description;
    product.Category = category;
    product.Tags = new List<Tag>(); // Remove as tags existentes para depois adicionar as novas

    if (productReq.Tags != null)
    {
        product.Tags = new List<Tag>();

        foreach (var tag in productReq.Tags)
        {
            product.Tags.Add(new Tag { Name = tag });
        }

        context.SaveChanges();
        return Results.Ok();
    }
    else
        return Results.NotFound();
   
});

#endregion

#region DELETE 

app.MapDelete("/product/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{

    var product = context.Products.Where(p => p.Id == id).FirstOrDefault();
    context.Products.Remove(product);
    context.SaveChanges();

    return Results.NoContent();
});

#endregion

app.Run();