using LibHub.BorrowService.Data;
using LibHub.BorrowService.Data.Repositories;
using LibHub.BorrowService.Infrastructure.Clients;
using LibHub.BorrowService.Services;
using Microsoft.EntityFrameworkCore;
using Consul;
using LibHub.BorrowService.Infrastructure.Discovery;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BorrowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IBorrowService, BorrowService>();

// Configure service discovery
builder.Services.AddServiceDiscovery();

builder.Services.AddHttpClient<IBookServiceClient, BookServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://book-service");
}).AddServiceDiscovery();

// Add Consul Client configuration
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    var address = builder.Configuration["Consul:Address"];
    if (address != null)
    {
        consulConfig.Address = new Uri(address);
    }
}));

// Add our custom Consul registration service
builder.Services.AddHostedService<ConsulHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok());

app.Run();
