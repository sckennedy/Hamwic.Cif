﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hamwic.Cif.Core.Data;
using Hamwic.Cif.Core.Entities;
using Hamwic.Cif.Core.Events;
using Hamwic.Cif.Core.Implementation.Events;
using Hamwic.Cif.Core.Implementation.Services;
using Hamwic.Cif.Core.Services;
using Hamwic.Cif.Web.Framework;
using Hamwic.Core.Commands;
using Hamwic.Core.Events;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

namespace Hamwic.Cif.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //AddIdentity adds cookie based authentication
            //also adds scoped classed for UserManager, SignInManager, PasswordHashers etc.
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Stores.MaxLengthForKeys = 128;
                    options.User.RequireUniqueEmail = true;
                })
                //This adds the UserStore and RoleStore that the RoleManager, UserManager etc need
                .AddEntityFrameworkStores<ApplicationDbContext>()
                //Adds a provider that generates unique keys and hashes for things like
                //forgot password links, phone number verification codes etc.
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.HttpOnly = true; 
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            services.AddMvc(config =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddControllersAsServices();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(FindConnectionString()));

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //this utilities class works with the ServiceBasedControllerActivator middleware to populate the 
            //properties on the ControllerBase object
            services.AddScoped<IIocUtilities, NetCoreIocUtilities>();
            //this implementation enables property injection in to the controllers, in particular the controllerbase
            services.AddScoped<IControllerActivator, Framework.ServiceBasedControllerActivator>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddSingleton<ICommandProcessor, CommandProcessor>();
            services.AddSingleton<IEventDispatcher, DomainEventDispatcher>();

            //add the command handlers as scoped so we get a new one for each request.


            //add the event handlers as scoped so we get a new one for each request
            services.AddScoped<IDomainEventHandler<UserLoggedInEvent>, UserLoggedInEventHandler>();
            var domainEvents = GetIDomainEventTypes(Assembly.GetAssembly(typeof(UserLoggedInEvent)));

            foreach (var domainEvent in domainEvents)
            {
                //services.AddScoped<IDomainEventHandler<typeof(domainEvent)>, UserLoggedInEventHandler>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });
        }

        /// <summary>
        /// Method that combines the machine name with the solution to try to find the connection
        /// string for the executing machine with a fallback to the main connection
        /// string if machine specific one is not found.
        /// </summary>
        /// <returns>string: connectionstring</returns>
        private string FindConnectionString()
        {
            var connectionStringNames = new[]
            {
                $"HamwicCifDb_{Environment.MachineName}",
                $"HamwicCifDb"
            };

            string connectionString = null;
            foreach (var connectionStringName in connectionStringNames)
            {
                connectionString = Configuration.GetConnectionString(connectionStringName);
                if (connectionString != null)
                    break;

                Console.WriteLine("WARNING: Unable to find a connection string named {0}", connectionStringName);
            }

            if (connectionString == null)
            {
                Log.Error("Unable to find a connection string with any of the names " +
                          string.Join(", ", connectionStringNames));
                throw new Exception("Unable to find connection string name in appsettings");
            }

            Log.Information($"Using connection string {connectionString}");
            return connectionString;
        }

        private IEnumerable<Type> GetIDomainEventTypes(Assembly asm)
        {
            var it = typeof(IDomainEvent);
            return asm.GetLoadableTypes().Where(it.IsAssignableFrom).ToList();
        }
    }
}
