using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentManagementAPI.Data;
using StudentManagementAPI.Models;
using StudentManagementAPI.Services;
using System.Text;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// --- SERVICES ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Just paste the token (no 'Bearer' prefix needed).",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Swashbuckle v10 / OpenApi v2: use delegate-based AddSecurityRequirement
    c.AddSecurityRequirement(document =>
    {
        return new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        };
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 1. Identity Services (with Role support for your assignment)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 2. JWT Authentication
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// 3. Force JWT Bearer as the default (overrides Identity's cookie defaults)
builder.Services.Configure<AuthenticationOptions>(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
});

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<ITokenService, TokenService>();
/* builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
}); */
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

// 3. Role Seeding (Required for the Job Portal Assignment)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Employer", "Student", "Admin" };
    foreach (var role in roles)
    {
        if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
        {
            roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
        }
    }
}

// --- PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// 4. Important: Authentication first, then Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();