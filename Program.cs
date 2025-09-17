using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "TestUser",
            ValidAudience = "Admin",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("yourSuperDuperExtraSecretKey123456789123456789"))   // Must be very long or silently fails?
        };
    });

// You'd replace this with an actual SQL database connection, if you wanted to progress further.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TestDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

// I guess UseAuthentication() is needed despite our custom authentication middleware, since [AllowAnonymous]
// doesn't work otherwise and we can't get a valid token.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();