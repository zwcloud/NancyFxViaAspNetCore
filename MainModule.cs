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

            Get["/"] = (context, p) =>
            {
                var viewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(), new ModelStateDictionary());
                viewData["Title"] = "NancyFx via ASP.NET Core 3.1";

                return View["Main", context, viewData];
            };

            Get["/Home/", true] = async (context, p) =>
            {
                var viewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(), new ModelStateDictionary());
                viewData["DateTime"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                return await View["Home/Index", context, viewData, true];
            };
        }
    }
}