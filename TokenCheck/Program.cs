using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using StudentManagement_Infrastructure.Persistence;
using StudentManagement_Domain.Interface;
using StudentManagement_Infrastructure.Repositories;
using StudentManagement_Application.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "StudentAdministration.API", Version = "v1" });
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

builder.Services.AddTransient<IAuthRepository, AuthRepository>();

builder.Services.AddTransient<IStudentRepository, StudentRepository>();

builder.Services.AddTransient<IClassRepository, ClassRepository>();

builder.Services.AddTransient<ICourseRepository, CourseRepository>();

builder.Services.AddTransient<IEnrollmentRepository, EnrollmentRepository>();

builder.Services.AddTransient<IScheduleRepository, ScheduleRepository>();

builder.Services.AddTransient<IGradeRepository, GradeRepository>();

builder.Services.AddTransient<IExamRepository, ExamRepository>();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddScoped<LecturerRepository>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();

builder.Services.AddScoped<IClaimRepository, ClaimRepository>();

builder.Services.AddTransient<ITuitionFeeRepository, TuitionFeeRepository>();



builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<EnrollmentService>();
builder.Services.AddTransient<GradeService>();
builder.Services.AddTransient<EmailService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DB") ??
    throw new InvalidOperationException("ConnectionString Not Found")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
    options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
})
                     .AddJwtBearer(IdentityConstants.BearerScheme, options =>
                     {
                         //options.Authority = "localhost";

                         options.TokenValidationParameters = new TokenValidationParameters
                         {
                             ValidIssuer = "localhost",
                             ValidateAudience = false,
                             IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("CJKFOGk-9E0aI8Gv09mD-8utzSyLQx_yrJKi1fXc6Y7CeYszLzcmMA2C0_Ej3K7BQdsCW9zoqW3a-5L1ZNRytFC0BeA6dZLsCjoTrFoI9guwvEmJ0gbN9yHQ0fDYbkwGUyJbP6eNEzKbWHMarSx7RWGKaGsxy0qguEMSO3OUWU8"))
                         };
                     });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageStudentsPolicy", policy =>
        policy.RequireClaim("permission", "CanManageStudents"));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageSchedulePolicy", policy =>
        policy.RequireClaim("permission", "CanManageSchedules"));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageExam_GradePolicy", policy =>
        policy.RequireClaim("permission", "CanManageExam_Grade"));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageUserPolicy", policy =>
        policy.RequireClaim("permission", "CanManageUsers"));
              
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageTP_ClassPolicy", policy =>
        policy.RequireClaim("permission", "CanManageTP_Classes"));

});

var app = builder.Build();

app.UseCors("AllowAllOrigins");

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

app.Run();
