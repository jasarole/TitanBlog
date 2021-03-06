using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBlog.Data;
using TitanBlog.Models;
using TitanBlog.Services;
using TitanBlog.Services.Interfaces;
using TitanBlog.Classes;
using System.IO;
using Microsoft.OpenApi.Models;

namespace TitanBlog
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Connection.GetConnectionString(Configuration)));

            services.AddCors(options =>
            {
                options.AddPolicy("DefaultPolicy",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddScoped<BasicSlugService>();
            services.AddTransient<BasicSeedService>();
            services.AddTransient<SearchService>();

            //Register BasicImageService to be used when the interface IImageService is used
            services.AddTransient<IImageService, BasicImageService>();

            //Register GmailSmtpService concrete class to be used when IEmailSender is used
            services.AddTransient<IEmailSender, GmailSmtpService>();

            //Add in Swagger as a registered service
            services.AddSwaggerGen(s =>
            {
                //Configure Swagger to use xml comments
                var xmlDocPath = $"{Directory.GetCurrentDirectory()}/wwwroot/TitanBlog.xml";
                s.IncludeXmlComments(xmlDocPath, true);

                //Configure descriptive information for the Help page
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Blog API",
                    Version = "v1.0",
                    Description = "Serving up Blog Post data...",
                    Contact = new OpenApiContact
                    {
                        Name = "Jason Lynn",
                        Email = "contact@jasondevs.com",
                        Url = new Uri("https://www.linkedin.com/in/JasonLynn")
                    }
                });

            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Use the referenced CORS Policy
            app.UseCors("DefaultPolicy");

            //Configure Swagger
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {

                s.InjectStylesheet("/swagger/swaggerCustom.css");
                s.InjectJavascript("/swagger/swaggerCustom.js");
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Titan Blog API");
                s.DocumentTitle = "Titan Blog API";
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //Adding custom slug route
                endpoints.MapControllerRoute(
                    name: "SlugRoute",
                    pattern: "BlogPosts/UrlFriendly/{slug}",
                    defaults: new { controller = "Posts", action = "Details"});

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });
        }
    }
}
