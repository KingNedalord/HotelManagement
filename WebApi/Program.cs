using HotelManagement.Core.Extensions;
using HotelManagement.WebCore.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Load configuration from WebCore ──────────────────────────────────────────
var webCorePath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "WebCore"));
if (Directory.Exists(webCorePath))
{
    builder.Configuration.AddJsonFile(Path.Combine(webCorePath, "appsettings.json"), optional: true, reloadOnChange: true);
    builder.Configuration.AddJsonFile(Path.Combine(webCorePath, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true, reloadOnChange: true);
}
builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true, reloadOnChange: true);

// ── Core Services (DbContext, Services, PasswordHasher) ───────────────────────
builder.Services.AddCoreServices(builder.Configuration);

// ── WebCore Services (JWT Authentication, Authorization, Exception Handling) ──
builder.Services.AddWebCoreServices(builder.Configuration);

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── OpenAPI / Swagger ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Hotel Management API", Version = "v1" });

    // Define the JWT Bearer security scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Swagger UI will add the 'Bearer ' prefix automatically."
    });

    // Require Bearer token on every operation
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ── Exception middleware ──────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler();   // always active; in dev it falls through to DeveloperExceptionPage
app.UseStatusCodePages();    // bare 404/405 → problem-details JSON

// ── Swagger UI ────────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Management API v1");
    options.RoutePrefix = string.Empty;   // serve Swagger UI at root "/"
    options.EnablePersistAuthorization(); // keep token in browser across refreshes
});

app.UseHttpsRedirection();

// ── Authentication & Authorization ────────────────────────────────────────────
app.UseAuthentication();
app.UseAuthorization();

// ── Routes ────────────────────────────────────────────────────────────────────
app.MapControllers();

app.Run();