using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TaskTracker.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
    });
builder.Services.AddProblemDetails();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Task Tracker API",
        Version = "v1",
        Description = "Manage projects and their tasks. Data is stored in a SQLite database by default, or in memory (see the README)."
    });

    // Show the /// comments from the controllers in Swagger.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile), includeControllerXmlComments: true);
});

// Choose where data is stored: "Sqlite" (a database file) or "InMemory" (a List that resets on restart).
var dataStore = builder.Configuration["DataStore"];
if (dataStore == "Sqlite")
{
    var dbPath = Path.Combine(builder.Environment.ContentRootPath, "tasktracker.db");
    builder.Services.AddDbContext<TaskTrackerDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));
    builder.Services.AddScoped<ITaskTrackerRepository, EfTaskTrackerRepository>();
}
else
{
    builder.Services.AddSingleton<ITaskTrackerRepository, InMemoryTaskTrackerRepository>();
}

var app = builder.Build();

// Create the database file and tables on first run, and apply any new migrations.
if (dataStore == "Sqlite")
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TaskTrackerDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();