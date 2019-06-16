﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseHelperBasicVersion.Models;
using CourseHelperBasicVersion.Models.Data;
using CourseHelperBasicVersion.Models.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseHelperBasicVersion
{
    public class Startup { 

        public IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Adds identity database connection to services
            services.AddDbContext<AppIdentityDBContext>(options => options.UseSqlServer(Configuration["Data:CourseHelperIdentity:ConnectionString"]));
            // Adds content database connection to services
            services.AddDbContext<CourseHelperDBContext>(options => options.UseSqlServer(Configuration["Data:CourseHelper:ConnectionString"]));

            // Seting custom class for identity & roles
            services.AddIdentity<CourseHelperUser, IdentityRole>(options => {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            })
                .AddEntityFrameworkStores<AppIdentityDBContext>()
                .AddDefaultTokenProviders();
            
            // Setting lifetime of database table access
            services.AddTransient<ICourseDatabase, EFCourseDatabase>();
            services.AddTransient<IStudentDatabase, EFStudentDatabase>();
            services.AddTransient<ICourseStudentDatabase, EFCourseStudentDatabase>();
            services.AddTransient<IReviewDatabase, EFReviewDatabase>();

            // Setting default authorization routes
            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/Login";
                options.LogoutPath = "/Logout";
                options.AccessDeniedPath = "/AccessDenied";
            });
            services.AddMvc();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "",
                    template: "Student/ReviewList",
                    defaults: new { controller = "Review", action = "List", area = "Student" }
                    );
                routes.MapRoute(
                    name: "",
                    template: "Student/CreateReview/{courseCode}",
                    defaults: new { controller = "Review", action = "Create", area = "Student" }
                    );
                routes.MapRoute(
                    name: "",
                    template: "Student/UnEnroll/{courseCode}",
                    defaults: new { controller = "Student", action = "UnEnroll", area = "Student" }
                    );
                routes.MapRoute(
                    name: "",
                    template: "Student/Enroll/{courseCode}",
                    defaults: new { controller = "Student", action = "Enroll", area = "Student" }
                    );
                routes.MapRoute(
                    name: "",
                    template: "Faculty/Manage/{courseCode}",
                    defaults: new { controller = "Course", action = "Manage", area = "Faculty" }
                    );
                routes.MapRoute(
                    name: "",
                    template: "Faculty/Edit/{courseCode}",
                    defaults: new { controller = "Course", action = "Edit", area = "Faculty" }
                    );
                routes.MapRoute(
                    name: "",
                    template: "Student/{action=Index}",
                    defaults: new { controller = "Student", area = "Student" }
                    );
                routes.MapRoute(
                     name: "",
                     template: "Account/{action=Index}",
                    defaults: new { controller = "Account", area = "Admin" }
                     );
                routes.MapRoute(
                    name: "",
                    template: "Admin/{action=Index}",
                    defaults: new { controller = "Account", area = "Admin" }
                    );
                routes.MapRoute(
                    name: "",
                    template: "Faculty/{action=Index}",
                    defaults: new { controller = "Course", area = "Faculty" }
                    );
                routes.MapRoute(
                    name: "Register",
                 template: "Register",
                    defaults: new { area = "Admin", controller = "Account", action = "Create" }
                    );
                routes.MapRoute(
                    name: "Login",
                    template: "Login",
                    defaults: new { area = "Admin", controller = "Account", action = "Login" }
                    );
                routes.MapRoute(
                    name: "Logout",
                    template: "Logout",
                    defaults: new { area = "Admin", controller = "Account", action = "Logout" }
                    );
                routes.MapRoute(
                    name: "AccessDenied",
                    template: "AccessDenied",
                    defaults: new { area = "Admin", controller = "Account", action = "AccessDenied" }
                    );
                routes.MapRoute(
                    name:"default",
                    template:"{controller=Guest}/{action=Index}",
                    defaults: new {area="Guest"}
                    );
            });
            SeedData.Populate(app);
            IdentitySeedData.Populate(app);
        }
    }
}
