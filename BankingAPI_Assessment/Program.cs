using Application;
using BankingAPI;
using DotNetEnv;
using FluentValidation;
using Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
Env.Load();

// Configure Serilog logging
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Core ASP.NET Core services
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// Layer-specific service registrations
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// Validation services (must be in API layer due to package dependencies)
builder.Services.AddValidatorsFromAssembly(Application.Config.AppAssemblyReference.assembly, includeInternalTypes: true);

//automapper excluded due to time constraints

// AutoMapper configuration
// builder.Services.AddAutoMapper((serviceProvider, autoMapper) =>
// {
//     autoMapper.AllowNullCollections = true;
//     autoMapper.AddCollectionMappers();
// }, Application.Config.AppAssemblyReference.assembly);

// Exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// Configure request pipeline
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapControllers();

app.Run();