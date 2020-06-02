﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Nancy;
using System;
using System.Globalization;
using WebApp;

namespace Nancy
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["Hello"] = (context, p) => "Hello World!";

            Get["Topics"] = (context, p) => Text("nancyfx, aspnet, aspnetcore");

            Get["/"] = (context, p) =>
            {
                var viewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(), new ModelStateDictionary());
                viewData["Title"] = "NancyFx via ASP.NET Core 3.1";

                return View["Main", context, viewData];
            };

            Get["/DateTime/", true] = async (context, p) =>
            {
                var viewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(), new ModelStateDictionary());
                viewData["DateTime"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                return await View["Home/DateTime", context, viewData, true];
            };

            Get["/Projects"] = (context, p) =>
            {
                var projects = DBL.GetProjects();
                return Json(projects);
            };
        }
    }
}