using cms.Api.Authorization;
using cms.Api.Extensions;
using cms.Api.Filters;
using cms.Api.Services;
using cms.Core.ConfigOptions;
using cms.Core.Identity;
using cms.Core.Models.Content;
using cms.Core.Repositories;
using cms.Core.SeedWorks;
using cms.Core.Services;
using cms.Data;
using cms.Data.Repositories;
using cms.Data.SeedWorks;
using cms.Data.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

var CMSCorsPolicy = "CMSCorsPolicy";
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddCors(o => o.AddPolicy(CMSCorsPolicy, builder =>
{
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins(configuration["AllowedOrigins"]?.Split(";"))
        .AllowCredentials();
}));

// Start. Config DB Context and ASP.NET Core Identity
builder.Services.AddDbContext<CMSDbContext>(options =>
                options.UseSqlServer(connectionString));

builder.Services.AddIdentity<AppUser, AppRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<CMSDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = false;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
}); // End. Config DB Context and ASP.NET Core Identity

//Auto mapper
builder.Services.AddAutoMapper(typeof(PostInListDto));

//Authen and author
builder.Services.Configure<JwtTokenSettings>(configuration.GetSection("JwtTokenSettings"));

builder.Services.AddScoped<SignInManager<AppUser>, SignInManager<AppUser>>();
builder.Services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();
builder.Services.AddScoped<IRoyaltyService, RoyaltyService>();

// Add services to the container.
builder.Services.AddScoped(typeof(IRepository<,>), typeof(RepositoryBase<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddTransient<IPostRepository, PostRepository>();
builder.Services.AddTransient<IPostCategoryRepository, PostCategoryRepository>();


// Default config for ASP.NET core
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomOperationIds(apiDesc =>
    {
        return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
    });
    c.SwaggerDoc("AdminAPI", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "API for Administrators",
        Description = "API for CMS core domain. This domain keeps track of campaigns, campaign rules, and campaign execution."
    });
    c.ParameterFilter<SwaggerNullableParameterFilter>();
});

// Add BearerToken
builder.Services.AddAuthentication(o => { 
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;

    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = configuration["JwtTokenSettings:Issuer"],
        ValidAudience = configuration["JwtTokenSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtTokenSettings:Key"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("AdminAPI/swagger.json", "Admin API");
        c.DisplayOperationId();
        c.DisplayRequestDuration();
    });
}

app.UseCors(CMSCorsPolicy);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seeding data
app.MigrateDatabase();

app.Run();
