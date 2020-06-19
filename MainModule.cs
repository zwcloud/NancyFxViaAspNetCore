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
            
            Get["Rss"] = (context, p) => Text(
@"<?xml version=""1.0"" encoding=""UTF-8"" ?>
<rss version=""2.0"">
<channel>
  <title>Home Page</title>
  <link>https://github.com/zwcloud</link>
  <description>zwcloud's projects</description>
  <item>
    <title>ImGui</title>
    <link>https://github.com/zwcloud/ImGui</link>
    <description>Immediate Mode GUI for C#</description>
  </item>
  <item>
    <title>Mesh Terrain Editor</title>
    <link>https://assetstore.unity.com/packages/tools/terrain/mesh-terrain-editor-pro-57515</link>
    <description>a mesh based terrain creator, converter and editor</description>
  </item>
</channel>
</rss>", "application/xml");

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

            Get["File/{id}"] = (context, p) =>
            {
                int fileId = p.id;
                var filePath = $"/wwwroot/files/{fileId}.txt";
                return File(filePath);
            };
        }
    }
}
