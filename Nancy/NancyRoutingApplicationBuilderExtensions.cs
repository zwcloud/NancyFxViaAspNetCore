using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Nancy
{
    public static class NancyRoutingApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNancyRouting(this IApplicationBuilder builder,
            IWebHostEnvironment env)
        {
            foreach (var type in Modules)
            {
                var module = Activator.CreateInstance(type) as NancyModule;
                module?.Configure(builder, env);
            }

            return builder;
        }

        private static IEnumerable<Type> Modules
        {
            get
            {
                if (modules != null)
                {
                    return modules;
                }
            
                var derivedTypes = new List<Type>();
                foreach (var domainAssembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var assemblyTypes = domainAssembly.GetTypes()
                        .Where(type => type.IsSubclassOf(typeof(NancyModule)) && !type.IsAbstract);

                    derivedTypes.AddRange(assemblyTypes);
                }
                modules = derivedTypes.ToArray();
                return modules;
            }
        }

        private static IEnumerable<Type> modules;
    }
}