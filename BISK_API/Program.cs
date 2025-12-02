using System.Net;
using System.Net.Http.Headers;
using System.Text;
using gardnerAPIs.Common;
//using gardnerAPIs.Data;
using GardeningAPI.Data;
using gardnerAPIs.Services;
using GardnerAPI.BusinessLogic;
using BISK_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using GardeningAPI.Services;
using GardeningAPI.Application.Interfaces;
using GardeningAPI.Helper;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WEBAPI",
        Version = "v1",
        Description = "Gardening API (Masters & Posting via SAP B1 Service Layer)"
    });

    c.EnableAnnotations();

   
    var jwtScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter JWT only (no 'Bearer ' prefix).",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtScheme, Array.Empty<string>() } });
});

builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("corsapp", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});



builder.Services.AddTransient<IConfig, Configuration>();
builder.Services.AddSingleton<IUserRepository, AuthService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<OdbcClient>();
builder.Services.AddSingleton<IDatabase, DatabaseService>();
builder.Services.AddSingleton<IPostingBusinessLogic, PostingBusinessLogic>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IItemsService, ItemsService>();
builder.Services.AddScoped<HelperService>();
builder.Services.AddScoped<IBusinessPartnerService, BusinessPartnerService>();
//builder.Services.AddScoped<OtpService>();

builder.Services.Configure<ServiceLayerOptions>(builder.Configuration.GetSection("ServiceLayer"));

builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Auth"));

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IServiceLayerClient, ServiceLayerClient>((sp, http) =>
{
    var opts = sp.GetRequiredService<IOptions<ServiceLayerOptions>>().Value;
    http.BaseAddress = new Uri(opts.Url.TrimEnd('/') + "/"); // e.g. .../b1s/v2/
    http.Timeout = TimeSpan.FromSeconds(60);
    http.DefaultRequestHeaders.Accept.Clear();
    http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    if (!http.DefaultRequestHeaders.Contains("Prefer"))
        http.DefaultRequestHeaders.Add("Prefer", "return=representation");
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    UseCookies = true,
    CookieContainer = new CookieContainer(),
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
    PooledConnectionLifetime = TimeSpan.FromMinutes(10)
});


var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Jwt:Key is missing in configuration.");

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"]; 

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
            ValidIssuer = issuer,
            ValidateAudience = !string.IsNullOrWhiteSpace(audience),
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WEBAPI v1");
    c.DocumentTitle = "WEBAPI • BISK";
});

app.UseRouting();
app.UseCors("corsapp");

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapControllers();

ConfigManager.Instance.init();

app.Run();
