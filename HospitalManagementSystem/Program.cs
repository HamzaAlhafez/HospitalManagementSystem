using System.Reflection;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HospitalManagementSystem.Services.Interfaces.Auth;
using HospitalManagementSystem.Services.Auth;
using HospitalManagementSystem.Repositories.Auth;
using HospitalManagementSystem.Repositories.Interfaces.Auth;
using HospitalManagementSystem.Services.Auth.Token;
using HospitalManagementSystem.Services.Interfaces.Auth.Token;
using HospitalManagementSystem.Repositories.Interfaces.DoctorManagemment;
using HospitalManagementSystem.Repositories.DoctorManagemment;
using HospitalManagementSystem.Services.Interfaces.DoctorManagemment;
using HospitalManagementSystem.Services.DoctorManagemment;
using HospitalManagementSystem.Services.Interfaces.UserManagement;
using HospitalManagementSystem.Services.UserManagement;
using HospitalManagementSystem.Repositories.Interfaces.UserManagement;
using HospitalManagementSystem.Repositories.UserManagement;
using HospitalManagementSystem.Repositories.Interfaces.AdminManagement;
using HospitalManagementSystem.Repositories.AdminManagement;
using HospitalManagementSystem.Services.Interfaces.AdminManagement;
using HospitalManagementSystem.Services.AdminManagement;
using HospitalManagementSystem.Repositories.Interfaces.PatientManagement;
using HospitalManagementSystem.Repositories.Interfaces.AppointmentManagement;
using HospitalManagementSystem.Services.PatientManagement;
using HospitalManagementSystem.Repositories.PatientManagement;
using HospitalManagementSystem.Services.Interfaces.PatientManagement;
using HospitalManagementSystem.Services.Interfaces.AppointmentManagement;
using HospitalManagementSystem.Services.AppointmentManagement;
using HospitalManagementSystem.Repositories.AppointmentManagement;
using HospitalManagementSystem.Repositories.Interfaces.ReviewManagement;
using HospitalManagementSystem.Repositories.ReviewManagement;
using HospitalManagementSystem.Services.Interfaces.ReviewManagement;
using HospitalManagementSystem.Services.ReviewManagement;
using HospitalManagementSystem.Services.Interfaces.MailingManagement;
using HospitalManagementSystem.Services.MailingManagement;
using Serilog;
using HospitalManagementSystem.Services.Interfaces.StatsManagement;
using HospitalManagementSystem.Services.StatsManagement;
using HospitalManagementSystem.Repositories.Interfaces.StatsManagement;
using HospitalManagementSystem.Repositories.StatsManagement; 

var builder = WebApplication.CreateBuilder(args);

// ================ Serilog  ================ //
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    Log.Information("Start initializing the application");
    // ================  Serilog ================ //

    // Add services to the container.
    builder.Services.AddDbContext<ApplicationDbContext>(cfg => cfg.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // use Serilog  Logger 
    builder.Host.UseSerilog((context, services, config) =>
        config.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<IAuthRespository, AuthRespository>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IDoctorManagemmentRespository, DoctorManagemmentRespository>();
    builder.Services.AddScoped<IDoctorMangementService, DoctorMangementService>();
    builder.Services.AddScoped<IUserManagementRespository, UserManagementRespository>();
    builder.Services.AddScoped<IUserManagementService, UserManagementService>();
    builder.Services.AddScoped<IAdminManagementRespository, AdminManagementRespository>();
    builder.Services.AddScoped<IAdminManagementService, AdminManagementService>();
    builder.Services.AddScoped<IPatientManagementRespository, PatientManagementRespository>();
    builder.Services.AddScoped<IPatientManagementService, PatientManagementService>();
    builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
    builder.Services.AddScoped<IAppointmentService, AppointmentService>();
    builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
    builder.Services.AddScoped<IReviewService, ReviewService>();
    builder.Services.AddTransient<IMailingService, MailingService>();
    builder.Services.AddScoped<IStatsRepository,StatsRepository>();
    builder.Services.AddScoped<IStatsService,StatsService>();

    builder.Services.AddHttpContextAccessor();
    var jwtoptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
    builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
    builder.Services.AddAuthentication(
        options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.RequireHttpsMetadata = false;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtoptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtoptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtoptions.SigningKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

    var app = builder.Build();

    // √÷› middleware · ”ÃÌ· «·ÿ·»« 
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication(); 
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("The application has been initialized successfully. Running...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A fatal error occurred during startup");
}
finally
{
    Log.CloseAndFlush();
}