using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Globalization;

namespace Nancy
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["Hello"] = (context, p) => "Hello World!";

            Get["/", true] = async (context, p) =>
            {
                var viewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(), new ModelStateDictionary());
                viewData["CurrentDateTime"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                viewData["Title"] = "NancyFx via ASP.NET Core 3.1";

                return View["Main", context, viewData];
            };

            Get["/Home/", true] = async (context, p) =>
            {
                var viewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(), new ModelStateDictionary());
                viewData["Time"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                return View["Home/Index", context, viewData];
            };
        }
    }
}