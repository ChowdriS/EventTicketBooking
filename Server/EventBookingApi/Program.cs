using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using EventBookingApi.Context;
using EventBookingApi.Interface;
using EventBookingApi.Misc;
using EventBookingApi.Model;
using EventBookingApi.Repository;
using EventBookingApi.Service;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("serilog.json", optional: false, reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddSwaggerGen(
opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "EventBookingApi", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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
}
);

builder.Services.AddControllers().AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    opts.JsonSerializerOptions.WriteIndented = true;
                });;

builder.Services.AddDbContext<EventContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSignalR();

#region Repository
builder.Services.AddTransient<IRepository<Guid, User>, UserRepository>();
builder.Services.AddTransient<IRepository<Guid, Event>, EventRepository>();
builder.Services.AddTransient<IRepository<Guid, Ticket>, TicketRepository>();
builder.Services.AddTransient<IRepository<Guid, TicketType>, TicketTypeRepository>();
builder.Services.AddTransient<IRepository<Guid, Payment>, PaymentRepository>();
builder.Services.AddTransient<IRepository<Guid, BookedSeat>, BookedSeatRepository>();
#endregion

#region Services
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IEncryptionService, EncryptionService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IEventService, EventService>();
builder.Services.AddTransient<ITicketService, TicketService>();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddTransient<ITicketTypeService, TicketTypeService>();
builder.Services.AddTransient<IAnalyticsService,AnalyticsService>();
#endregion

#region Mics
builder.Services.AddTransient<IOtherFunctionalities, OtherFunctionalities>();
builder.Services.AddTransient<ObjectMapper>();
#endregion

#region Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 5;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});
#endregion

#region AuthenticationFilter
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Keys:JwtTokenKey"]??""))
                    };
                });
#endregion


#region CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
#endregion



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Logging
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();

    string requestBody = "";
    if (context.Request.ContentLength > 0 && context.Request.ContentType?.Contains("application/json") == true)
    {
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        requestBody = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        requestBody = MaskSensitiveFields(requestBody);
    }

    var originalBodyStream = context.Response.Body;
    using var responseBody = new MemoryStream();
    context.Response.Body = responseBody;

    await next();

    context.Response.Body.Seek(0, SeekOrigin.Begin);
    var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
    context.Response.Body.Seek(0, SeekOrigin.Begin);

    Log.Information("HTTP {Method} {Path} | RequestBody: {RequestBody} | Status: {StatusCode}",
        context.Request.Method,
        context.Request.Path,
        requestBody,
        context.Response.StatusCode);

    await responseBody.CopyToAsync(originalBodyStream);

    string MaskSensitiveFields(string body)
    {
        var sensitiveKeys = new[] { "password", "confirmPassword", "token", "accessToken", "refreshToken" };

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            var masked = new Dictionary<string, object>();
            foreach (var prop in root.EnumerateObject())
            {
                if (sensitiveKeys.Contains(prop.Name, StringComparer.OrdinalIgnoreCase))
                {
                    masked[prop.Name] = "***";
                }
                else
                {
                    masked[prop.Name] = prop.Value.ValueKind == JsonValueKind.String ? prop.Value.GetString()! : prop.Value.ToString();
                }
            }

            return JsonSerializer.Serialize(masked);
        }
        catch
        {
            return body;
        }
    }
});
#endregion

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers().RequireRateLimiting("fixed");
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
