using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server_backend.utility;
using Server_backend.Database;
using Server_backend.FlightplanNS;
using Server_backend.RPiConnectionNS;

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
        /**
         * This method gets called by the runtime container.
         * Use this method to add services that needs to be able to be dependency injected by the runtime container.
         */
        public void ConfigureServices(IServiceCollection services)
        {
            // Allow preflight calls. See app.UserCors under the method Configure.
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
                builder.WithOrigins("http://localhost:4200") // Which IP's to allow passing through preflight. This IP is what Angular is usually hosted on.
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );

            app.UseMvc();

        }
    }
}
