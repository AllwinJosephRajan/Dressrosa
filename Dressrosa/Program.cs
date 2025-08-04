using DbUp;
using DbUp.ScriptProviders;
using Dressrosa.Converter;
using Dressrosa.Data;
using Dressrosa.Dto;
using Dressrosa.Model;
using Dressrosa.Service;
using Dressrosa.Services;
using Dressrosa.Infrastructures;
using Dressrosa.Core.Model;
using MySql.Data.MySqlClient;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);

// Load Configuration
var configuration = builder.Configuration;

// Configure Services (Dependency Injection)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(60);
});

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Bind Configuration Sections
var appSettingsSection = configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);

// Dependency Injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDBUnitOfWork, DBUnitOfWork>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddSingleton<Dressrosa.Infrastructures.IConfigurationManager, Dressrosa.Infrastructures.ConfigurationManager>();
builder.Services.AddScoped<IConverter<UserDto, User>, UserConverter>();
builder.Services.AddScoped<IConverter<TokenRequestDto, TokenRequest>, TokenRequestConverter>();
builder.Services.AddTransient<PasswordHasher>();

// Configure Database Connection
builder.Services.AddScoped<DbConnection>(provider =>
{
    var config = provider.GetRequiredService<Dressrosa.Infrastructures.IConfigurationManager>();
    var connectionString = config.GetDBConnectionStringAsync().Result;
    return new MySqlConnection(connectionString);
});

// Execute Migrations
var connectionString = configuration.GetConnectionString("Database");
var scriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts");
try
{
    var upgrader = DeployChanges.To
        .MySqlDatabase(connectionString)
        .WithScriptsFromFileSystem(scriptsPath, new FileSystemScriptOptions
        {
            IncludeSubDirectories = true
        })
        .LogToConsole()
        .Build();

    var result = upgrader.PerformUpgrade();
    if (!result.Successful)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(result.Error);
        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Migrations successful!");
        Console.ResetColor();
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Migration error: {ex.Message}");
    Console.ResetColor();
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
