﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing.Template;

namespace Nancy
{
    public class ViewRenderer
    {
        public ContentResult this[string viewName, HttpContext context, ViewDataDictionary viewDictionary]
        {
            get
            {
                using var stringWriter = new StringWriter();

                var actionContext =
                    new ActionContext(context, new RouteData(), new ActionDescriptor());
                var razorViewEngine = context.RequestServices.GetService<IRazorViewEngine>();

                ViewEngineResult viewEngineResult =
                    razorViewEngine.FindView(actionContext, viewName, true);
                if (viewEngineResult.View == null)
                {
                    throw new InvalidOperationException($"No view<{viewName}> exists");
                }

                var viewContext = new ViewContext(actionContext, viewEngineResult.View, viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext,
                        context.RequestServices.GetService<ITempDataProvider>()),
                    stringWriter, new HtmlHelperOptions());

                viewEngineResult.View.RenderAsync(viewContext).Wait();

                var contentResult = new ContentResult();
                contentResult.Content = stringWriter.ToString();
                contentResult.ContentType = "text/html";
                return contentResult;
            }
        }
    }

    /// <summary>
    /// Helper class for configuring a route handler in a module.
    /// </summary>
    public class RouteCollecter : IHideObjectMembers
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteBuilder"/> class.
        /// </summary>
        /// <param name="method">The HTTP request method that the route should be available for.</param>
        /// <param name="module"></param>
        public RouteCollecter(string method, NancyModule module)
        {
            this.method = method;
            this.module = module;
        }

        /// <summary>
        /// Defines a route for the specified <paramref name="path"/>.
        /// </summary>
        /// <value>A delegate that is used to invoke the route.</value>
        public Func<HttpContext, dynamic, dynamic> this[string path]
        {
            set
            {
                module.Routes.Add((string.Empty, path, value));
            }
        }

        /// <summary>
        /// Defines a route for the specified <paramref name="path"/>.
        /// </summary>
        /// <value>A delegate that is used to invoke the route.</value>
        public Func<HttpContext, dynamic, Task<dynamic>> this[string path, bool runAsync]
        {
            set
            {
                module.AsyncRoutes.Add((string.Empty, path, value));
            }
        }

        private readonly string method;
        private readonly NancyModule module;
    }


    public class NancyModule
    {
        public RouteCollecter Get { get; }
        public RouteCollecter Post { get; }

        public NancyModule()
        {
            Routes =
                new List<(
                    string name,
                    string path,
                    Func<HttpContext, DynamicDictionary, dynamic> func)
                >(16);
            AsyncRoutes =
                new List<(
                    string name,
                    string path,
                    Func<HttpContext, DynamicDictionary, Task<dynamic>> func
                    )>(16);
            Post = new RouteCollecter("POST", this);
            Get = new RouteCollecter("GET", this);
            View = new ViewRenderer();
        }

        public static Task WriteActionResult<TResult>(HttpContext context, TResult result) where TResult : IActionResult
        {
            var executor = context.RequestServices.GetRequiredService<IActionResultExecutor<TResult>>();

            if (executor == null)
            {
                throw new InvalidOperationException($"No action result executor for {typeof(TResult).FullName} registered.");
            }

            var routeData = context.GetRouteData() ?? new RouteData();
            var actionContext = new ActionContext(context, routeData, new ActionDescriptor());

            return executor.ExecuteAsync(actionContext, result);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.env = env;

            app.UseEndpoints((endpoints) =>
            {
                foreach (var route in Routes)
                {
                    endpoints.Map(route.path, context =>
                    {
                        //create dic from actual URL params according to route.path
                        var dic = new DynamicDictionary();
                        var routeTemplate = TemplateParser.Parse(route.path);
                        var matcher = new TemplateMatcher(routeTemplate, null);
                        var values = new RouteValueDictionary();
                        if (matcher.TryMatch(context.Request.Path, values))
                        {
                            foreach (var item in values)
                            {
                                //Console.WriteLine("{0}: {1}", item.Key, item.Value);
                                dic.Add(item.Key, item.Value);
                            }
                        }

                        var result = route.func.Invoke(context, dic);
                        return WriteActionResult(context, result);
                    });
                }
                foreach (var route in AsyncRoutes)
                {
                    endpoints.Map(route.path, async context =>
                    {
                        //create dic from actual URL params according to route.path
                        var dic = new DynamicDictionary();
                        var routeTemplate = TemplateParser.Parse(route.path);
                        var matcher = new TemplateMatcher(routeTemplate, null);
                        var values = new RouteValueDictionary();
                        if (matcher.TryMatch(context.Request.Path, values))
                        {
                            foreach (var item in values)
                            {
                                //Console.WriteLine("{0}: {1}", item.Key, item.Value);
                                dic.Add(item.Key, item.Value);
                            }
                        }

                        var result = await route.func.Invoke(context, dic);
                        await WriteActionResult(context, result);
                    });
                }
            });
        }

        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-3.1#routing-basics

        public ViewRenderer View { get; }

        public ViewDataDictionary ViewData { get; }
            = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

        public string MapPath(string relativePath)
        {
            return Path.Combine(env.ContentRootPath, relativePath);
        }

        public JsonResult Json(object data)
        {
            return new JsonResult(data);
        }

        public ContentResult Text(string text, string mimeType)
        {
            var content = new ContentResult();
            content.Content = text;
            content.ContentType = mimeType;
            return content;
        }

        private IWebHostEnvironment env;
        public List<(string name, string path, Func<HttpContext, DynamicDictionary, dynamic> func)> Routes { get; }
        public List<(string name, string path, Func<HttpContext, DynamicDictionary, Task<dynamic>> func)> AsyncRoutes { get; }
    }
}