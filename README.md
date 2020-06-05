# NancyFx via ASP.NET Core
An ASP.NET Core 3.1 template project that uses [NancyFx 1.4.4 routing](https://github.com/NancyFx/Nancy/wiki/Defining-routes) instead of regular MVC.

# Why
I'd grown to like routing in NancyFx before v1.4.4:

```C#
public class SampleModule : Nancy.NancyModule
{
    public SampleModule()
    {
        Get["/"] = _ => "Hello World!";
        Get["/home/"]= _ =>
        {
            return View["Home/Index"];
        };
        Get["/project/{name}"]= p =>
        {
            return View["Project/" + p.name];
        };
        Get["blog/{id}"]= p =>
        {
            Blog blog = GetBlog(p.id);
            return Response.AsJson(blog);
        };
    }
}
```

But since [NancyFx has been discontinued](https://github.com/NancyFx/Nancy/issues/3010), I decided to implement similar routing inside ASP.NET Core instead.

# Run

```bat
git clone https://github.com/zwcloud/NancyFxViaAspNetCore.git
cd .\NancyFxViaAspNetCore
dotnet run
```

Then in your webbrowser, input `http://localhost:5000` and enter.

# Usage

![Usage](https://raw.githubusercontent.com/wiki/zwcloud/NancyFxViaAspNetCore/showcase.png)

