using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;


using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Security.Claims;

using System.Collections.Generic;

using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;

using Apiraiser.Services;
using Apiraiser.Enums;
using Apiraiser.Models;
using Apiraiser.Interfaces;
using Apiraiser.Constants;

namespace Apiraiser
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IMemoryCache MemoryCache { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews()
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = Schemas.System, Version = "v3" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                    }
                });
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services
            .AddCors(
                o => o.AddPolicy("ApiraiserPolicy",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                    }
                )
            );

            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
            {
                // Set the limit to 256 MB
                options.MultipartBodyLengthLimit = 268435456;
            });

            string issuer = Environment.GetEnvironmentVariable(Config.JwtTokenIssuer);
            if (issuer == null)
            {
                issuer = Config.DefaultJwtTokenIssuer;
            }

            string key = Environment.GetEnvironmentVariable(Config.JwtTokenSecretKey);
            if (key == null)
            {
                key = Config.DefaultJwtTokenSecretKey;
            }

            services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                }
            );
            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            services.AddMemoryCache(cacheOptions =>
            {
            });

            // To add Policies which can be used used in Controllers like this:
            // [Authorize(Policy = "CanCreateTablesData")]
            // services.AddAuthorization(options =>
            // {
            //     options.AddPolicy("CanCreateTablesData", policy =>
            //         policy.RequireAssertion(context =>
            //             context.User.HasClaim(c =>
            //                 (
            //                     c.Type == ClaimTypes.Authentication && c.Value.Contains("CanCreateTablesData")
            //                 )
            //             )
            //         )
            //     );
            //     options.AddPolicy("CanCreateOwnData", policy =>
            //         policy.RequireAssertion(context =>
            //             context.User.HasClaim(c =>
            //                 (
            //                     c.Type == ClaimTypes.Authentication && c.Value.Contains("CanCreateOwnData")
            //                 )
            //             )
            //         )
            //     );
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMemoryCache cache)
        {
            this.MemoryCache = cache;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Apiraiser v1");
                c.RoutePrefix = "Documentation";
            });

            // app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            // app.UseCors(option => option
            //                 .AllowAnyOrigin()
            //                 .AllowAnyMethod()
            //                 .AllowAnyHeader()
            //             );

            app.UseCors("ApiraiserPolicy");

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.MapWhen(x => !x.Request.Path.Value.ToLower().StartsWith("/api") && !x.Request.Path.Value.ToLower().StartsWith("/documentation"), reactApp =>
            {
                reactApp.UseSpa(spa =>
                {
                    // spa.Options.SourcePath = "ClientApp";
                    spa.Options.SourcePath = Path.Join(env.ContentRootPath, "ClientApp");
                    // spa.Options.SourcePath = Path.Join(env.ContentRootPath, "ClientBuild");
                    if (env.IsDevelopment())
                    {
                        spa.UseReactDevelopmentServer(npmScript: "start");
                    }
                });
            });



            // app.Map("/admin", admin =>
            // {
            //     admin.UseSpa(spa =>
            //     {
            //         spa.Options.SourcePath = "ClientApp";

            //         if (env.IsDevelopment())
            //         {
            //             spa.UseReactDevelopmentServer(npmScript: "start --app=admin --base-href=/admin/ --serve-path=/admin/");
            //         }
            //     });
            // });


            // Register Services
            RegisterServices();

            // Initialize
            Initialize();
        }

        private Type[] GetInstantiableTypes(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes()
            .Where(x => x.FullName.Contains("Service") && !x.FullName.Contains("Base") && !x.FullName.Contains("ServiceManager"))
            .ToArray();
        }

        private void RegisterServices()
        {
            Type[] types = GetInstantiableTypes(Assembly.GetExecutingAssembly(), "Apiraiser.Services");
            foreach (Type type in types)
            {
                if (type.FullName.Contains("Apiraiser.Services.") && !type.FullName.Contains("+<"))
                {
                    Console.WriteLine("Registering: " + type.FullName);
                    if (type.FullName == "Apiraiser.Services.MemoryCacheService")
                    {
                        Activator.CreateInstance(type, Configuration, MemoryCache);
                    }
                    else
                    {
                        Activator.CreateInstance(type, Configuration);
                    }
                }
            }
        }

        private void Initialize()
        {
            // Logger
            ServiceManager
                .Instance
                .GetService<LogService>()
                .SetLoggingLevelFromEnvironment();

            ServiceManager
                .Instance
                .GetService<LogService>()
                .Print(string.Format("Starting {0}", Application.Name), LoggingLevel.Info);

            //Database
            DatabaseTypes databaseType = (DatabaseTypes)Enum.Parse(
                typeof(DatabaseTypes),
                (string)ServiceManager
                    .Instance
                    .GetService<EnvironmentService>()
                    .GetEnvironmentVariable(Config.DatabaseType, "Postgresql")
            );

            ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .SetQueryBuilder(databaseType);

            ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .SetQueryExecuter(databaseType);

            ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .SetQueryHelper(databaseType);

            ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .SetDatabaseDriver(databaseType);

            ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .SetDatabaseErrorHandler(databaseType);

            ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetDatabaseConnection()
                .SetDatabaseConnectionUsingEnvironment();
        }
    }
}
