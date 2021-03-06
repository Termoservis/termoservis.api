﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using termoservis.api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using termoservis.api.Models;
using AutoMapper;
using termoservis.api.Domain.Abstract;
using termoservis.api.Domain.Places;
using termoservis.api.Domain.Countries;
using termoservis.api.Services;

namespace termoservis.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy(
                "AllowAll",
                p => p
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()));

            services.AddDbContext<ITermoservisContext, TermoservisContext>(options => options
                    .UseNpgsql(Configuration.GetConnectionString("DefaultPgsqlServerConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<TermoservisContext>()
                    .AddDefaultTokenProviders();

            services.AddMvcCore().AddVersionedApiExplorer(o => {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddApiVersioning();
            services.AddSwaggerGen(
                    options =>
                    {
                        var provider = services.BuildServiceProvider()
                                               .GetRequiredService<IApiVersionDescriptionProvider>();

                        foreach (var description in provider.ApiVersionDescriptions)
                        {
                            options.SwaggerDoc(
                                description.GroupName,
                                new Info()
                                {
                                    Title = $"Termoservis API {description.ApiVersion}",
                                    Version = description.ApiVersion.ToString()
                                });
                        }
                    });
            services.AddAutoMapper(config => {
                config.AddProfile<PlaceProfile>();
            });

            services.AddTransient<IQueryDispatcher, QueryDispatcher>();
            services.AddTransient<ICommandDispatcher, CommandDispatcher>();
            services.AddTransient<QueryPlacesFiltered>();
            services.AddTransient<CommandCountryCreate>();
            services.AddTransient<CommandPlaceCreate>();

            services.AddTransient<IPlacesRepository, PlacesRepository>();
            services.AddTransient<IPlacesApiService, PlacesApiService>();
            services.AddTransient<ICountriesRepository, CountriesRepository>();
            services.AddTransient<ICountriesApiService, CountriesApiService>();
        }

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("AllowAll");
            app.AddSentryContext();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(
                    options =>
                    {
                        foreach (var description in provider.ApiVersionDescriptions)
                        {
                            options.SwaggerEndpoint(
                                $"/swagger/{description.GroupName}/swagger.json",
                                description.GroupName.ToUpperInvariant());
                        }
                    });
            app.UseMvc();
        }
    }
}
