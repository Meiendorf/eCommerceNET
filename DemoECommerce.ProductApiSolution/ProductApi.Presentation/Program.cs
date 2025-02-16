using ProductApi.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
    
builder.Services.AddProductApiInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseProductApiInfrastructure();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "ProductApi"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();