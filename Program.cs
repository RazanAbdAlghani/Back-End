
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using secondVersionFlowSync.Data;
using secondVersionFlowSync.Errors;
using secondVersionFlowSync.Models;
using secondVersionFlowSync.services;
using Serilog;
using secondVersionFlowSync.services.EmailService;

namespace secondVersionFlowSync
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           
            // Add services to the container.

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // ????? ??? Identity ?? ???????

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {

                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;


                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                //options.User.RequireUniqueEmail = true;       
            }).AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Add Cors
            builder.Services.AddCors(
               options =>
               {
                   options.AddPolicy("AllowAll",
                   builder =>
                   {
                       builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();


                   });

               });


            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            ////Email Confirmation Service
            //builder.Services.Configure<IdentityOptions>(options =>
            //{
            //    options.SignIn.RequireConfirmedEmail = true;
            //});

            // Outloock
            builder.Services.AddScoped<IEmailService, EmailService>();

            // Serilog
            builder.Host.UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration);
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.UseExceptionHandler();
            app.Run();
        }
    }
}
