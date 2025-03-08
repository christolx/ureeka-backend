using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ureeka_backend.Data;
using ureeka_backend.Models;
using ureeka_backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen((opt) =>
{
    opt.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer",
    });
    
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, new string[] { }
        }
    });
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.Services.AddDbContext<AppDbContext>((db) =>
{
    db.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddStackExchangeRedisCache(option =>
{
    option.Configuration = builder.Configuration.GetConnectionString("Redis");
    option.InstanceName = "fooCache";
});

builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var accountGroup = app.MapGroup("account");
accountGroup.MapCustomIdentityApi<User>();
accountGroup.MapPost("/logout", async (SignInManager<User> signInManager) =>
    {
        await signInManager.SignOutAsync();
        return Results.Ok(new { message = "Logged out successfully" });
    })
    .RequireAuthorization();
app.MapControllers();

app.Run();