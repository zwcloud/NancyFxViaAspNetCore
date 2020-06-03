using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nancy;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationExpanders.Add(new CustomViewLocator());
        });

        services.AddMvc()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            }
        );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseStaticFiles();//default: wwwroot/

        app.UseRouting();

        app.UseAuthorization();

        foreach (var type in Modules)
        {
            var module = Activator.CreateInstance(type) as NancyModule;
            module?.Configure(app, env);
        }
    }
    
    protected virtual IEnumerable<Type> Modules
    {
        get
        {
            if (this.modules != null)
            {
                return this.modules;
            }
            
            var derivedTypes = new List<Type>();
            foreach (var domainAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var assemblyTypes = domainAssembly.GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(NancyModule)) && !type.IsAbstract);

                derivedTypes.AddRange(assemblyTypes);
            }
            modules = derivedTypes.ToArray();
            return this.modules;
        }
    }

    private IEnumerable<Type> modules;
}