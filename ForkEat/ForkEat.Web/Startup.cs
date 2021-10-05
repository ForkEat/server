using System;
using System.Text;
using ForkEat.Core.Domain;
using ForkEat.Core.Repositories;
using ForkEat.Core.Services;
using ForkEat.Web.Adapters.Files;
using ForkEat.Web.Database;
using ForkEat.Web.Database.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace ForkEat.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(GetPostgresConnectionString()));


            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordValidator, PasswordValidator>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IFilesRepository, FilesRepository>();
            services.AddScoped<IRecipeService, RecipeService>();
            services.AddScoped<IRecipeRepository, RecipeRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IStockRepository, StockRepository>();


            ConfigureAuth(services);

            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ForkEat.Web", Version = "v1" });
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext db)
        {
            db.Database.Migrate();

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ForkEat.Web v1"));

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(cors =>
            {
                cors.AllowAnyOrigin();
                cors.AllowAnyMethod();
                cors.AllowAnyHeader();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }


        private string GetPostgresConnectionString()
        {
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (databaseUrl is null)
            {
                throw new ArgumentException("Please populate the DATABASE_URL env variable");
            }

            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/')
            };

            return builder.ToString();
        }

        private static void ConfigureAuth(IServiceCollection services)
        {
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                            throw new ArgumentException("JWT_SECRET Env variable is not set");
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            services.AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(auth =>
                {
                    auth.SaveToken = true;
                    auth.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                })
                ;
        }
    }
}