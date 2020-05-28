using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

public class CustomViewLocator : IViewLocationExpander
{
    public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations)
    {
        return new List<string>{$"/Pages/{context.ViewName}{RazorViewEngine.ViewExtension}"};
    }

    public void PopulateValues(ViewLocationExpanderContext context)
    {
    }
}