using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using middleware.Context;
using middleware.Contracts;
using middleware.Helpers;
using middleware.Models;
using middleware.Respositories;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace middleware_autorization_authentication_auditing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _key = Encoding.Default.GetBytes(configuration.GetSection("secret").Value);
            _cookieName = configuration.GetSection("cookieName").Value;
        }

        public IConfiguration Configuration { get; }
        private readonly byte[] _key;
        private readonly String _cookieName;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Autenticação, autorização e auditoria API",
                    Description = "Documentação do middleware autenticação, autorização e auditoria.",
                    TermsOfService = new Uri("https://github.com/gabriel2mm/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Gabriel Maia",
                        Email = "gabriel_more@hotmail.com",
                        Url = new Uri("https://github.com/gabriel2mm/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use MTI License",
                        Url = new Uri("https://github.com/gabriel2mm/"),
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddSwaggerGenNewtonsoftSupport();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<Context>(options => options.UseSqlServer(Configuration.GetConnectionString("principalConnection")), ServiceLifetime.Scoped);
            services.AddIdentity<User, IdentityRole>()
             .AddEntityFrameworkStores<Context>()
             .AddDefaultTokenProviders();

            services.AddScoped<IRepository<Order>, OrderRepository>();
            services.AddScoped<IRepository<Request>, RequestRepository>();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;

                }).AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
                        {
                            if (expires != null)
                                if (DateTime.UtcNow < expires)
                                    return true;

                            return false;
                        },
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[_cookieName];
                        return Task.CompletedTask;
                    }
                    };
                });
            services.AddAuthorization(option => Policies.CreatePolicies(option));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });
            loggerFactory.AddFile("Logs/myapp-{Date}.log");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(options => options.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
