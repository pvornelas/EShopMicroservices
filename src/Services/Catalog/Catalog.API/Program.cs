using BuildingBlocks.Exceptions.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to container
builder.Services.AddCarter();

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddMarten(opts =>
{
    opts.Connection(connectionString: builder.Configuration.GetConnectionString("Database")!);
    opts.DisableNpgsqlLogging = true;
}).UseLightweightSessions();

if (builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>();

// Cross-Cutting Services
builder.Services.AddBuildingBlocksExceptionHandling(builder.Environment);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapCarter();

app.UseBuildingBlocksExceptionHandling();

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();