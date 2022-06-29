using System;
using Fasetto.Word.Web.Server.Data;
using Fasetto.Word.Web.Server.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Fasetto.Word.Web.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // Share the configuration
            IocContainer.Configuration = configuration;
        }

        //public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //Add ApplicationDBCOntext to DI
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(IocContainer.Configuration.GetConnectionString("DefaultConnection")));

            //AddIdentity adds cookie-based Authentication
            //Adds scoped classes for things like UserManager, SignInManager, PasswordHashers etc.
            services.AddIdentity<ApplicationUser, IdentityRole>()

                //Add UserStore And RoleStore from this context
                //that are consumed by the UserManager and RoleManager
                .AddEntityFrameworkStores<ApplicationDbContext>()

                //Adds a provider that generates unique keys and hashes for things like
                //forgot password links, phone number verfication codes etc.
                .AddDefaultTokenProviders();

            services.AddAuthentication().AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = IocContainer.Configuration["Jwt:Issuer"],
                        ValidAudience = IocContainer.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(IocContainer.Configuration["Jwt:SecretKey"]))
                    };
                });

            //change password policy/validations
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            });

            //Change login url from default & alter cookie timeout
            services.ConfigureApplicationCookie(options =>
            {
                //redirect to login
                options.LoginPath = "/login";//when private is unauthorized it redirects to this configured path

                //change cookie timeout to 15 seconds
                options.ExpireTimeSpan = TimeSpan.FromSeconds(1500);
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            //Store instance of the di service provider so its accessible to our app globally
            IocContainer.Provider = serviceProvider;
            var userStore = IocContainer.Provider.GetService<UserStore<ApplicationUser>>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //Setup Identity
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
