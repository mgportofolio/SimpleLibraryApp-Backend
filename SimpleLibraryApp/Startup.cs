using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleLibrary.Services.Helper;
using SimpleLibrary.Services.Helper.Auth;
using SimpleLibrary.Services.Services.Auth;
using SimpleLibrary.Services.Services.Book;
using SimpleLibrary.Services.Services.Rent;
using SimpleLibrary.Services.Services.User;
using SimpleLibraryDbContext.Entities;

namespace SimpleLibraryApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddDbContext<LibraryDb_DevContext>(options =>
            {
                options.UseSqlServer
                (Configuration.GetConnectionString("ConnString"),
                   strategy =>
                   {
                       strategy.EnableRetryOnFailure();
                   }
                );
            });
            services.AddControllers();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<BookService>();
            services.AddScoped<RentService>();
            services.AddScoped<UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();
            app.UseCors(x => x
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
            app.UseRouting();
            app.UseMiddleware<JwtMiddleware>();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
