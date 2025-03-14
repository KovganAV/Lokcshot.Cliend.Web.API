using Lokcshot.Cliend.Web.API;
using Lokcshot.Cliend.Web.API.Core.Interfaces;
using Lokcshot.Cliend.Web.API.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Proof_of_Concept.Hubs;
using Refit;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using UserActivity.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Use scheme for authorization header: Bearer {token}",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.MaximumReceiveMessageSize = 1024 * 1024 * 5;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "UserUI",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:3000", "http://192.168.180.13:3000", "http://192.168.17.57:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("JwtSettings:jwtSecret").Value)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
        };
    });

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<MicroservicesSettings>(builder.Configuration.GetSection("MicroservicesSettings"));

builder.Services.RegisterApiServices(builder.Configuration);

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseCors("UserUI");

app.UseAuthentication();


app.UseAuthorization();

app.MapControllers();

app.MapHub<ClientHub>("/clientHub");
app.MapHub<ChatHub>("/chat");
app.MapHub<VoiceChatHub>("/voicechat");

app.Run();
