
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieTicketBooking.Authorization;
using MovieTicketBooking.Data;
using MovieTicketBooking.Middleware;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Register controllers
builder.Services.AddControllers();

// 2. Register database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// 3. Configure ASP.NET Core Identity
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 4. Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];

if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "JWT key must contain at least 32 characters. Configure it using User Secrets."
    );
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],

                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)
                ),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
    });

// 5. Configure Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BookingOwnerPolicy", policy =>
    {
        policy.Requirements.Add(new BookingOwnerRequirement());
    });
});

// Register custom authorization handler
builder.Services.AddScoped<IAuthorizationHandler, BookingOwnerHandler>();

// 6. Register Movie repository and service
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();

// 7. Register Theatre repository and service
builder.Services.AddScoped<ITheatreRepository, TheatreRepository>();
builder.Services.AddScoped<ITheatreService, TheatreService>();

// 8. Register Screen repository and service
builder.Services.AddScoped<IScreenRepository, ScreenRepository>();
builder.Services.AddScoped<IScreenService, ScreenService>();

// 9. Register Seat repository and service
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<ISeatService, SeatService>();

// 10. Register Show repository and service
builder.Services.AddScoped<IShowRepository, ShowRepository>();
builder.Services.AddScoped<IShowService, ShowService>();

// 11. Register ShowSeat repository and service
builder.Services.AddScoped<IShowSeatRepository, ShowSeatRepository>();
builder.Services.AddScoped<IShowSeatService, ShowSeatService>();

// 12. Register Booking repository and service
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();

// 13. Register Authentication service
builder.Services.AddScoped<IAuthService, AuthService>();

// 14. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 15. Build application
var app = builder.Build();

// 16. HTTPS Redirection
app.UseHttpsRedirection();

// 17. Routing
// Explicit routing is used so endpoint metadata is available
// to the diagnostic middleware.
app.UseRouting();

// 18. Endpoint Metadata Diagnostics
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<EndpointMetadataMiddleware>();
}

// 19. CORS
app.UseCors("AllowAngular");

// 20. Authentication
// Validates JWT and populates HttpContext.User
app.UseAuthentication();

// 21. Authentication and Authorization Diagnostics
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<AuthenticationDiagnosticsMiddleware>();

    app.UseMiddleware<AuthorizationDiagnosticsMiddleware>();
}

// 22. Authorization
// Checks endpoint access permissions
app.UseAuthorization();

// 23. Seed Identity roles and Admin account
await DbSeeder.SeedAsync(app.Services, app.Configuration);

// 24. Map controllers
app.MapControllers();

// 25. Run application
app.Run();
