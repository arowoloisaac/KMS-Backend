
using Key_Management_System.Configuration;
using Key_Management_System.Data;
using Key_Management_System.Models;
using Key_Management_System.Services.AssignKeyService;
using Key_Management_System.Services.AuthenticationService;
using Key_Management_System.Services.KeyService;
using Key_Management_System.Services.RequestKeyService;
using Key_Management_System.Services.ThirdPartyService;
using Key_Management_System.Services.UserServices.CollectorService;
using Key_Management_System.Services.UserServices.SharedService;
using Key_Management_System.Services.UserServices.TokenService;
using Key_Management_System.Services.UserServices.TokenService.TokenGenerator;
using Key_Management_System.Services.UserServices.WorkerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace Key_Management_System
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //DotNetEnv.Env.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAutoMapper(typeof(AutoMapperConfiguration));

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IkeyService, KeyService>();
            builder.Services.AddScoped<ISharedService, SharedService>();
            builder.Services.AddScoped<IWorkerService, WorkerService>();
            builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
            builder.Services.AddScoped<ICollectorService, CollectorService>();
            builder.Services.AddScoped<IAssignKeyService, AssignKeyService>();
            builder.Services.AddScoped<IThirdPartyService, ThirdPartyService>();
            builder.Services.AddScoped<IRequestKeyService, RequestKeyService>();
            builder.Services.AddScoped<ITokenStorageService, TokenDbStorageService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(ApplicationRoleNames.Admin, new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimTypes.Role, ApplicationRoleNames.Admin)
                    .RequireRole(ApplicationRoleNames.Admin).
                    RequireAuthenticatedUser()
                    .Build());

                options.AddPolicy(ApplicationRoleNames.Collector, new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimTypes.Role, ApplicationRoleNames.Collector)
                    .RequireRole(ApplicationRoleNames.Collector).
                    RequireAuthenticatedUser()
                    .Build());

                options.AddPolicy(ApplicationRoleNames.Manager, new AuthorizationPolicyBuilder()
                    .RequireClaim(ClaimTypes.Role, ApplicationRoleNames.Manager)
                    .RequireRole(ApplicationRoleNames.Manager).
                    RequireAuthenticatedUser()
                    .Build());


                options.AddPolicy("User",
                    new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
            });

            builder.Services.AddCors();


            var jwtSection = builder.Configuration.GetSection("JwtBearerTokenSettings");
            builder.Services.Configure<JwtBearerTokenSettings>(jwtSection);

            var jwtConfiguration = jwtSection.Get<JwtBearerTokenSettings>();
            var key = Encoding.ASCII.GetBytes(jwtConfiguration.SecretKey);

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwtConfiguration.Audience,
                    ValidIssuer = jwtConfiguration.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                };

            });

            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.EnableAnnotations();
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
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
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            var app = builder.Build();

            using var serviceScope = app.Services.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.Migrate();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors(builder => 
            builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();


            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                var roles = new[] { ApplicationRoleNames.Admin, ApplicationRoleNames.Manager, ApplicationRoleNames.Collector };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new Role { Name = role } );
                    }
                }
            }

            using (var scope = app.Services.CreateScope())
            {
                var usermanager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                var config = app.Configuration.GetSection("Credentials");

                var checkAdminExistence = await usermanager.FindByEmailAsync(config["Email"]);

                if (checkAdminExistence == null)
                {
                    var AdminUser = new Worker
                    {
                        FirstName = "Administrator",
                        Email = config["Email"],
                        UserName = config["UserName"],
                        Password = config["Password"]
                    };

                    var result = await usermanager.CreateAsync(AdminUser, config["Password"]);


                    if (!result.Succeeded)
                    {
                        throw new Exception("Unable to create user admin");
                    }

                    checkAdminExistence = await usermanager.FindByEmailAsync(config["Email"]);
                }

                if(!await usermanager.IsInRoleAsync(checkAdminExistence, ApplicationRoleNames.Admin))
                {
                    await usermanager.AddToRoleAsync(checkAdminExistence, "Admin");
                }


            }


            app.Run();
        }
    }
}
