using LeadsManagement.Infrastructure.Data.Contexts;
using LeadsManagement.Application.Features.Leads.Commands;
using LeadsManagement.Domain.Services;
using LeadsManagement.Infrastructure.Services;
using LeadsManagement.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);


// 1. Basic services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. MediatR
builder.Services.AddMediatR(typeof(CreateLeadCommand).Assembly);

// 3. Database - ONLY ONCE
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    })
    .LogTo(Console.WriteLine, LogLevel.Information));

// 4. Dependencies
builder.Services.AddScoped<LeadRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();

// 5. CORS
builder.Services.AddCors(policy =>
{
    policy.AddPolicy("DevelopmentCorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("DevelopmentCorsPolicy");
app.MapControllers();

app.Run();
