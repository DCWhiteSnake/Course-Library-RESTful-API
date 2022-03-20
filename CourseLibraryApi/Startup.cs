using AutoMapper;
using CourseLibraryApi.DbContexts;
using CourseLibraryApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CourseLibraryApi
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
            //Todo: Implement CORS

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllers(
                setupAction => setupAction.ReturnHttpNotAcceptable = true)
                 .AddXmlDataContractSerializerFormatters()
                 .ConfigureApiBehaviorOptions(setupAction =>
                 {
                     setupAction.InvalidModelStateResponseFactory = context =>
                     {
                         // Create a problems details object
                         var problemDetailsFactory = context.HttpContext.RequestServices
                         .GetRequiredService<ProblemDetailsFactory>();

                         var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                             context.HttpContext,
                             context.ModelState);

                         // add info not added by default
                         problemDetails.Detail = "See the error field for details";
                         problemDetails.Instance = context.HttpContext.Request.Path;

                         // find out which status codes to use
                         var actionExecutingContext =
                         context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                         // if there are model state errors and all arguments are correctly found/parsed
                         // then we are dealing with validation errors
                         if ((context.ModelState.ErrorCount > 0) &&
                         (actionExecutingContext?.ActionArguments.Count ==
                         context.ActionDescriptor.Parameters.Count))
                         {
                             problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
                             problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                             problemDetails.Title = "One or more validation errors occured.";

                             return new UnprocessableEntityObjectResult(problemDetails)
                             {
                                 ContentTypes = { "application/problem+json" }
                             };
                         }
                         // if one of the arguments wasn't correctly found / couldn't be parsed
                         // we're dealing with null/unparseable input
                         problemDetails.Status = StatusCodes.Status400BadRequest;
                         problemDetails.Title = "One or more errors on input occurred.";
                         return new BadRequestObjectResult(problemDetails)
                         {
                             ContentTypes = { "application/problem+json" }
                         };
                     };
                 });

            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened, please" +
                            "try again");
                    });
                });
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}