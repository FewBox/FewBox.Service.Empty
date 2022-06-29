using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FewBox.Core.Web.Extension;
using System.Collections.Generic;
using FewBox.Core.Utility.Net;

namespace FewBox.Service.Empty
{
    public class Startup
    {
        private IList<ApiVersionDocument> ApiVersionDocuments = new List<ApiVersionDocument> {
                new ApiVersionDocument{
                    ApiVersion = new ApiVersion(1, 0, "alpha1"),
                    IsDefault = true
                }
            };
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
            services.AddFewBox(this.ApiVersionDocuments, FewBoxDBType.None, FewBoxAuthType.Payload);
            RestfulUtility.IsCertificateNeedValidate = false;
            RestfulUtility.IsEnsureSuccessStatusCode = false;
            HttpUtility.IsCertificateNeedValidate = false;
            HttpUtility.IsEnsureSuccessStatusCode = false;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseFewBox(this.ApiVersionDocuments);
        }
    }
}