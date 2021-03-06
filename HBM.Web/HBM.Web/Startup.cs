﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;
using HBM.Web.Contexts;
using HBM.Web.Managers;
using System.Data.Entity.Migrations;
using HBM.Web.Migrations;
using System.Configuration;
using System.Linq;

[assembly: OwinStartup(typeof(HBM.Web.Startup))]

namespace HBM.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/User/Login")
            });
            
            app.CreatePerOwinContext(UserDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888

            if (bool.Parse(ConfigurationManager.AppSettings["MigrateDatabaseToLatestVersion"]))
            {
                //var configuration = new Migrations.Configuration();
                //var migrator = new DbMigrator(configuration);
                //if (migrator.GetPendingMigrations().Any())
                //    migrator.Update();
            }
        }
    }
}
