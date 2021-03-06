﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DualAuthCore.Data;
using DualAuthCore.Models;
using DualAuthCore.Services;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;

namespace DualAuthCore
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
      services.AddDbContext<DualAuthContext>(options =>
          options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

      services.AddTransient<DataSeeder>();

      services.AddIdentity<ApplicationUser, IdentityRole>()
          .AddEntityFrameworkStores<DualAuthContext>()
          .AddDefaultTokenProviders();

      // Enable Dual Authentication 
      services.AddAuthentication()
        .AddCookie(cfg => cfg.SlidingExpiration = true)
        .AddJwtBearer(cfg =>
        {
          cfg.RequireHttpsMetadata = false;
          cfg.SaveToken = true;
          
          cfg.TokenValidationParameters = new TokenValidationParameters()
          {
            ValidIssuer = Configuration["Tokens:Issuer"],
            ValidAudience = Configuration["Tokens:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
          };

        });

      // Add application services.
      services.AddTransient<IEmailSender, EmailSender>();

      services.AddControllersWithViews();
      services.AddRazorPages();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataSeeder seeder)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(c =>
      {
        c.MapRazorPages();
        c.MapDefaultControllerRoute();
      });

      seeder.SeedAsync().Wait();
    }
  }
}
