using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.Processors.Security;
using NSwag.Generation.AspNetCore;
using FewBox.Core.Web.Config;

namespace FewBox.Service.Empty
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.Configuration = configuration;
            this.Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true); // Lowercase the urls.
            services.AddMvc()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddCors(
                options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder.SetIsOriginAllowedToAllowWildcardSubdomains().WithOrigins("https://fewbox.com").AllowAnyMethod().AllowAnyHeader();
                        });
                    options.AddPolicy("all",
                        builder =>
                        {
                            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                        });
                }); // Cors.
            services.AddMemoryCache(); // Memory cache.
            services.AddSingleton(new HealthyConfig{ Version = "Empty" });
            // Used for Swagger Open Api Document.
            services.AddOpenApiDocument(config =>
            {
                this.InitAspNetCoreOpenApiDocumentGeneratorSettings(config, "v1", new[] { "1-alpha", "1-beta", "1" }, "v1");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOpenApi();
            app.UseStaticFiles();
            app.UseCors("all");
            if (env.IsDevelopment())
            {
                app.UseSwaggerUi3();
                app.UseDeveloperExceptionPage();
            }
            if (env.IsStaging())
            {
                app.UseSwaggerUi3();
                app.UseDeveloperExceptionPage();
            }
            if (env.IsProduction())
            {
                app.UseReDoc();
                app.UseHsts();
            }
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void InitAspNetCoreOpenApiDocumentGeneratorSettings(AspNetCoreOpenApiDocumentGeneratorSettings config, string documentName, string[] apiGroupNames, string documentVersion)
        {
            config.DocumentName = documentName;
            //config.ApiGroupNames = apiGroupNames;
            config.PostProcess = document =>
            {
                this.InitDocumentInfo(document, documentVersion);
            };
            config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
            config.DocumentProcessors.Add(
                new SecurityDefinitionAppender("JWT", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    Description = "Bearer [Token]",
                    In = OpenApiSecurityApiKeyLocation.Header
                })
            );
        }

        private void InitDocumentInfo(OpenApiDocument document, string version)
        {
            document.Info.Version = version;
            document.Info.Title = "FewBox Empty Api";
            document.Info.Description = "FewBox shipping, for more information please visit the 'https://fewbox.com'";
            document.Info.TermsOfService = "https://fewbox.com/terms";
            document.Info.Contact = new OpenApiContact
            {
                Name = "FewBox",
                Email = "support@fewbox.com",
                Url = "https://fewbox.com/support"
            };
            document.Info.License = new OpenApiLicense
            {
                Name = "Use under license",
                Url = "https://fewbox.com/license"
            };
        }
    }
}