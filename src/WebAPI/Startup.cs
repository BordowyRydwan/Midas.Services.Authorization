using System.Net;
using System.Text;
using Application.Dto;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Application.Validators;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Midas.Services;
using NLog;
using NLog.Web;
using WebAPI.Extensions;
using UserRegisterDto = Application.Dto.UserRegisterDto;
using UserUpdateDto = Application.Dto.UserUpdateDto;
using UserUpdateEmailDto = Application.Dto.UserUpdateEmailDto;

namespace WebAPI;

public class Startup
{
    private readonly WebApplicationBuilder _builder;
    private readonly Logger _logger;
    
    public Startup(string[] args)
    {
        _builder = WebApplication.CreateBuilder(args);
        _logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        _logger.Debug("The Authorization API was started");
    }

    public Startup SetBuilderOptions()
    {
        _builder.Services.AddControllers();
        _builder.Services.AddEndpointsApiExplorer();

        return this;
    }

    public Startup SetOpenCors()
    {
        _builder.Services.AddCors(options =>
        {
            options.AddPolicy("Open", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        _logger.Debug("The CORS open policy was successfully added");

        return this;
    }

    public Startup SetDbContext()
    {
        var authConnString = _builder.Configuration.GetConnectionString("AuthorizationConnection");

        _builder.Services.AddDbContext<AuthorizationDbContext>(options =>
        {
            options.UseSqlServer(authConnString).EnableSensitiveDataLogging();
        });
        
        _logger.Debug("SQL connection was successfully added");
        
        return this;
    }

    public Startup SetMapperConfig()
    {
        var mapperConfig = AutoMapperConfig.Initialize();

        _builder.Services.AddSingleton(mapperConfig);
        _logger.Debug("The mapping config was successfully added");

        return this;
    }

    public Startup AddInternalServices()
    {
        _builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
        _logger.Debug("Internal services were successfully added");

        return this;
    }

    public Startup AddInternalRepositories()
    {
        _builder.Services.AddScoped<IUserRepository, UserRepository>();
        _logger.Debug("Internal repositories were successfully added");

        return this;
    }

    public Startup AddLoggerConfig()
    {
        _builder.Logging.ClearProviders();
        _builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        _builder.Host.UseNLog();

        _logger.Debug("Logger options were successfully added");

        return this;
    }

    public Startup AddValidators()
    {
        _builder.Services.AddScoped<IValidator<UserRegisterDto>, UserRegisterDtoValidator>();
        _builder.Services.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();
        _builder.Services.AddScoped<IValidator<UserUpdateEmailDto>, UserUpdateEmailDtoValidator>();
        _builder.Services.AddScoped<IValidator<UserUpdatePasswordDto>, UserUpdatePasswordDtoValidator>();

        _logger.Debug("Validators were successfully added");
        
        return this;
    }

    public Startup AddPasswordHashers()
    {
        _builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        
        _logger.Debug("Password hashers were successfully added");
        return this;
    }

    public Startup SetupAuthentication()
    {
        _builder.Services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = "Bearer";
            option.DefaultScheme = "Bearer";
            option.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = _builder.Configuration["Jwt:Issuer"],
                ValidAudience = _builder.Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_builder.Configuration["Jwt:Key"]))
            };
        });

        _logger.Debug("Authorization has been successfully set");
        return this;
    }

    public Startup SetSwaggerConfig()
    {
        _builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Authorization API", Version = "0.1" });
            c.AddSecurityDefinition("AuthorizationBearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme.
          <br/> Enter your token in the text input below.
          <br/> You don't have to add prefix 'Bearer'.
          <br/> Example: 'this_is_my_token'",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "AuthorizationBearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer"
                    },
                    new List<string>()
                }
            });
        });

        return this;
    }
    
    public Startup SetExternalServiceClients()
    {
        var authServiceAddress = _builder.Configuration["ServiceAddresses:User"];
        var httpClientDelegate = (Action<HttpClient>)(client => client.BaseAddress = new Uri(authServiceAddress));
          

        _builder.Services.AddHttpClient<IUserClient, UserClient>(httpClientDelegate)
            .ConfigurePrimaryHttpMessageHandler(() => {
                return new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) => true
                };  
            })
            .AddHeaderPropagation();
        
        return this;
    }

    public void Run()
    {
        var app = _builder.Build();

        app.UseCors("Open");
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHeaderPropagation();
        app.MigrateDatabase();
        app.UseAuthentication();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
        
        _logger.Debug("Application has been successfully ran");
    }
}