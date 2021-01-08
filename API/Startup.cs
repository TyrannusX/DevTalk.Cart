using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using API.Auth;
using API.Consumers;
using API.Middleware;
using Dapper;
using Domain.Contracts;
using Domain.Entities;
using GreenPipes.Configurators;
using Infrastructure.DbContexts;
using Infrastructure.Queries;
using Infrastructure.SqlMappings;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace API
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
            services.AddControllers();

            //DB
            services.AddDbContext<CartDbContext>(x => x.UseSqlServer(Configuration["CartsConnectionString"], y => y.MigrationsAssembly("API")));
            services.AddScoped<IDbContext, CartDbContext>();

            //Dapper queries
            services.AddScoped<IQueries<Cart>, CartQueries>();
            SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));

            //MediatR
            services.AddMediatR(typeof(Startup));

            //Auth0
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = "https://dev-3y955wo3.us.auth0.com/";
                options.Audience = "DevTalk";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Cart", policy => policy.Requirements.Add(new HasScopeRequirement("Cart", "https://dev-3y955wo3.us.auth0.com/")));
            });

            //Mass Transit
            services.AddMassTransit(x =>
            {
                //add consumers
                x.AddConsumer<UserCreatedEventConsumer>();
                x.AddConsumer<PriceUpdatedEventConsumer>();

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.SelectBasicTier();
                    cfg.Host("Endpoint=sb://reyes-devtalk.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Wf1TqXpzwL+i34UvcdU2yGwqicIXYKUWrzDBEIqQ5Zo=");

                    //subscribe to user created
                    cfg.ReceiveEndpoint("usercreated", ep =>
                    {
                        ep.SelectBasicTier();
                        ep.PrefetchCount = 1;
                        ep.ConfigureConsumer<UserCreatedEventConsumer>(context);
                    });

                    //subscribe to price updated
                    cfg.ReceiveEndpoint("priceupdated", ep =>
                    {
                        ep.SelectBasicTier();
                        ep.PrefetchCount = 1;
                        ep.ConfigureConsumer<PriceUpdatedEventConsumer>(context);
                    });
                });
            });

            services.AddMassTransitHostedService();

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Cart API",
                    Description = "Manages carts for shopping app"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<RequestMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customers API"));

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
