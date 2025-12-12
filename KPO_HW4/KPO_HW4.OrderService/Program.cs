using EntityFramework.Exceptions.PostgreSQL;
using KPO_HW4.OrderService.Data;

var builder = WebApplication.CreateBuilder(args);
builder.AddNpgsqlDbContext<OrdersDbContext>("payments-db",
    settings => { },
    optionsBuilder =>
    {
        optionsBuilder.UseExceptionProcessor();
    });


builder.AddServiceDefaults();
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();