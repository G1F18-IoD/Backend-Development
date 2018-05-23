using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Server_backend.utility;
using Server_backend.Database;
using Server_backend.FlightplanNS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Server_backend.RPiConnectionNS;
using Microsoft.AspNetCore.Identity;

// p-23&G!bn?-sCK

namespace Server_backend
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

            // DI Services
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<ICommandService, CommandService>();
            services.AddTransient<IFlightplanService, FlightplanService>();
            services.AddTransient<IRPiConnectionService, RPiConnectionService>();
            services.AddTransient<IAuthenticationDatabaseService, DatabaseService>();
            services.AddTransient<ICommandDatabaseService, DatabaseService>();
            services.AddTransient<IFlightplanDatabaseService, DatabaseService>();
            services.AddTransient<IRPiConnectionDatabaseService, DatabaseService>();
            services.AddTransient<INpgSqlConnection, DatabaseConnection>();
            services.AddTransient<ISendHttpService, SendHttpService>();

            //Filters
            services.AddScoped<SaveAuthenticationHeader>();
            services.AddScoped<JwtAuthenticationAttribute>();
            services.AddScoped<ValidateToken>();


            services.AddMvc(options =>
            {
                
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
    builder.WithOrigins("http://localhost:4200")
           .AllowAnyHeader()
           .AllowAnyMethod()
    );

            app.UseMvc();

            //this.InitializeRoles(roleManager);
            
        }

        private string[] roles = new[] { "User", "Manager", "Administrator" };
        private async Task InitializeRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var newRole = new IdentityRole(role);
                    await roleManager.CreateAsync(newRole);
                    // In the real world, there might be claims associated with roles
                    // _roleManager.AddClaimAsync(newRole, new )
                }
            }
        }
    }
}
