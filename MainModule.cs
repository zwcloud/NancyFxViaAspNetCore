using Nancy;
using System;
using System.Globalization;

namespace WebApp
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["Hello"] = (context, p) => "Hello World!";

            Get["Topics"] = (context, p) => Text("nancyfx, aspnet, aspnetcore");

            Get["/"] = (context, p) =>
            {
                var viewData = GetViewData(context);
                viewData["Title"] = "NancyFx via ASP.NET Core 3.1";

                return View["Main", context, viewData];
            };

            Get["/DateTime/", true] = async (context, p) =>
            {
                var viewData = GetViewData(context);
                viewData["DateTime"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                return await View["Home/DateTime", context, viewData, true];
            };

            Get["/Projects"] = (context, p) =>
            {
                var projects = DBL.GetProjects();
                return Json(projects);
            };

            Get["Project/{id}"] = (context, p) =>
            {
                int projectId = p.id;
                var project = DBL.GetProject(projectId);
                return Json(project);
            };
        }
    }
}
