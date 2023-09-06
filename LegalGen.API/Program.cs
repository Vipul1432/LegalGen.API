using LegalGen.Data.Context;
using LegalGen.Data.Repository;
using LegalGen.Data.Services;
using LegalGen.Domain.Helper;
using LegalGen.Domain.Helpers;
using LegalGen.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Configure Serilog to read logging settings from the application's configuration file.
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Define and configure Swagger documentation settings for API.
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityApi", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please Enter a valid Token!",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

Microsoft.Extensions.Configuration.ConfigurationManager Configuration = builder.Configuration;


// Database connection string configuration
var connectionStrings = builder.Configuration.GetConnectionString("LegalGenAiEntities");
builder.Services.AddDbContextPool<LegalGenDbContext>(options => options.UseSqlServer(
connectionStrings, b => b.MigrationsAssembly("LegalGen.Data")));

// Registering scoped services for repository interfaces.
// This allows for the use of dependency injection to provide instances of these repositories
// to various parts of the application, ensuring data access is scoped to the current request.
builder.Services.AddScoped<IResearchBookRepository, ResearchBookRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddScoped<IResearchBookService, ResearchBookService>();

// For Identity 
builder.Services.AddIdentity<LegalGenUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<LegalGenDbContext>().AddDefaultTokenProviders();


// Configures authentication services with JWT Bearer authentication.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

//Adds JWT Bearer authentication options to the authentication services.
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = Configuration["JWT:ValidAudience"],
        ValidIssuer = Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
    };
});

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

app.MapControllers();

app.Run();
// Close and flush the Serilog logger when the application exits
Log.CloseAndFlush();
